using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HalconDotNet;
using HaspDemo.Core.Entities;
using HaspDemo.Core.Interfaces;

namespace HaspDemo.Vision
{
    // 只有一个类：既是 IDetectAlgorithm（接口要求），又是 IDisposable（释放模型）
    public class HalconWasherInspector : IDetectAlgorithm, IDisposable
    {
        private HTuple _modelID;

        // ===== 可调参数 =====
        public double MinMatchScore { get; set; } = 0.2;
        public int ExpectedWashers { get; set; } = 4;
        public double OuterDiaMin { get; set; } = 660;
        public double OuterDiaMax { get; set; } = 680;
        public double InnerDiaMin { get; set; } = 380;
        public double InnerDiaMax { get; set; } = 400;
        public double ErosionRadius { get; set; } = 15;
        public int MeanWindowSize { get; set; } = 151;
        public double ScratchSensitivity { get; set; } = 18;
        public double ScratchMinArea { get; set; } = 150;
        public double ScratchMinAnisometry { get; set; } = 3;

        private const string ModelFileName = "washer_model.shm";

        // 加载视觉模型
        public HalconWasherInspector()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                       "Models", ModelFileName);
            if (!File.Exists(path))
                throw new FileNotFoundException(
                    $"找不到形状模型文件：{path}\n请把 HDevelop 生成的 {ModelFileName} 放到该目录");
            HOperatorSet.ReadShapeModel(path, out _modelID);
        }

        // ===== 接口实现：DetectAsync =====
        public async Task<InspectionResult> DetectAsync(ImageData image, CancellationToken ct)
        {
            HObject hImg = HalconImageHelper.ToHObject(image);
            try
            {
                return await Task.Run(() => Inspect(hImg), ct);
            }
            finally { hImg?.Dispose(); }
        }

        // ===== 原有检测逻辑（原封不动，只是方法在同一个类里了）=====
        public InspectionResult Inspect(HObject grayImage)
        {
            // ...你原来的 ①模板匹配 ②Blob ③尺寸 ④划痕 代码原样保留...
            // 注意改成 result.ElapsedMs（不是 ElapseMs）
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var result = new InspectionResult();

            // ① 模板匹配定位
            HOperatorSet.FindShapeModel(grayImage, _modelID,
                -Math.PI, 2 * Math.PI, MinMatchScore, 0, 0.5,
                "least_squares", 0, 0.7,
                out HTuple rows, out HTuple cols, out HTuple angles, out HTuple scores);

            string matchValue = scores.Length > 0
    ? $"找到 {scores.Length} 片, 最高分: {scores.TupleMax().D.ToString("F2")}"
    : "未找到任何工件";

            result.Items.Add(new CheckItem
            {
                Name = "模板匹配",
                Value = matchValue,
                IsOk = scores.Length >= ExpectedWashers
            });
            for (int i = 0; i < scores.Length; i++)
                result.Matches.Add(new double[] { rows[i].D, cols[i].D, angles[i].D });

            // ② Blob 分割
            HOperatorSet.Threshold(grayImage, out HObject regions, 11, 243);
            HOperatorSet.Connection(regions, out HObject conn);
            HOperatorSet.SelectShape(conn, out HObject washers,
                "area", "and", 340100, 500000);
            HOperatorSet.CountObj(washers, out HTuple nWashers);

            // ③ 尺寸测量（逐片测）
            bool sizeOk = nWashers.I > 0;
            double outerMeas = 0, innerMeas = 0;
            for (int i = 1; i <= nWashers.I; i++)
            {
                HOperatorSet.SelectObj(washers, out HObject one, i);
                HOperatorSet.FillUp(one, out HObject fillUp);
                HOperatorSet.Difference(fillUp, one, out HObject holeRaw);
                HOperatorSet.Connection(holeRaw, out HObject holeConn);
                HOperatorSet.SelectShape(holeConn, out HObject hole,
                    "area", "and", 115527, 147514);
                HOperatorSet.CountObj(hole, out HTuple nHole);

                HOperatorSet.AreaCenter(fillUp, out HTuple aOut, out _, out _);

                
                HTuple aIn = new HTuple();
                if (nHole.I > 0)
                    HOperatorSet.AreaCenter(hole, out aIn, out _, out _);

                if (aOut.Length > 0 && aIn.Length > 0)
                {
                    outerMeas = 2 * Math.Sqrt(aOut[0].D / Math.PI);
                    innerMeas = 2 * Math.Sqrt(aIn[0].D / Math.PI);
                    if (outerMeas < OuterDiaMin || outerMeas > OuterDiaMax ||
                        innerMeas < InnerDiaMin || innerMeas > InnerDiaMax)
                        sizeOk = false;
                }
                else sizeOk = false;

                fillUp.Dispose(); holeRaw.Dispose(); holeConn.Dispose();
                hole.Dispose(); one.Dispose();
            }

            result.Items.Add(new CheckItem
            {
                Name = "尺寸测量",
                Value = nWashers.I > 0
                    ? $"外径 {outerMeas:F1} / 内径 {innerMeas:F1} px"
                    : "未找到工件",
                IsOk = sizeOk
            });

            // ④ 划痕检测（与HDevelop定稿脚本一一对应）
            // 合并所有垫圈区域 → 腐蚀掉边缘，得到有效检测区
            HOperatorSet.Union1(washers, out HObject ring);
            HOperatorSet.ErosionCircle(ring, out HObject ringInner, ErosionRadius);

            // 大窗口均值作为背景估计
            HOperatorSet.MeanImage(grayImage, out HObject mean,
                MeanWindowSize, MeanWindowSize);

            // 动态阈值抓暗偏差
            HOperatorSet.DynThreshold(grayImage, mean, out HObject darkRegions,
                ScratchSensitivity, "dark");

            // 闭运算：把断成几段的划痕连成整体
            HOperatorSet.ClosingCircle(darkRegions, out HObject darkClosed, 3.5);

            // 限定在工件表面内
            HOperatorSet.Intersection(darkClosed, ringInner, out HObject cand);

            // 连通域
            HOperatorSet.Connection(cand, out HObject candConn);

            // 双特征过滤：面积 + 细长度（anisometry，不是elongation！）
            HOperatorSet.SelectShape(candConn, out HObject scratches,
                new HTuple("area", "anisometry"), "and",
                new HTuple(ScratchMinArea, ScratchMinAnisometry),
                new HTuple(999999, 100));
            HOperatorSet.CountObj(scratches, out HTuple nScratch);

            // 记录结果
            result.Items.Add(new CheckItem
            {
                Name = "划痕检测",
                Value = nScratch.I == 0 ? "无" : $"发现 {nScratch.I} 处",
                IsOk = nScratch.I == 0
            });
            for (int i = 1; i <= nScratch.I; i++)
            {
                HOperatorSet.SelectObj(scratches, out HObject s, i);
                HOperatorSet.AreaCenter(s, out _, out HTuple sr, out HTuple sc);
                result.ScratchPoints.Add(new double[] { sr.D, sc.D });
                s.Dispose();
            }

            // 释放本段创建的HALCON对象
            ring.Dispose(); ringInner.Dispose(); mean.Dispose();
            darkRegions.Dispose(); darkClosed.Dispose(); cand.Dispose();
            candConn.Dispose(); scratches.Dispose();


            sw.Stop();
            result.ElapseMs= sw.ElapsedMilliseconds;
            return result;
        
        }

        // ===== 保存图像 =====
        public string SaveImage(HObject gray, int productId)
        {
            string folder = @"E:\Images";
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder,
                $"{productId}_{DateTime.Now:yyyyMMdd_HHmmss_fff}.bmp");
            HOperatorSet.WriteImage(gray, "bmp", 0, path);
            return path;    // ★ 不在这里 Dispose gray，释放归调用方
        }

        // ===== 释放模型 =====
        public void Dispose()
        {
            if (_modelID != null)
            {
                HOperatorSet.ClearShapeModel(_modelID);
                _modelID = null;
            }
        }
    }
}
