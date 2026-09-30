            // ★ 原来是 Applicationsss，拼写错了
using HaspDemo.Applicationsss;
using HaspDemo.Core.Entities;
using HaspDemo.Core.Enums;
using HaspDemo.Core.Interfaces;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HaspDemo.UI
{
    public partial class MainForm : Form
    {
        private readonly DetectionService _detection;
        private readonly ILogService _log;

        // ===== PLC 连接参数：按你的实际PLC改 =====
        private const string PlcIp = "192.168.231.129";
        private const ushort PlcRack = 0;
        private const ushort PlcSlot = 1;

        public MainForm() { InitializeComponent(); }

        public MainForm(DetectionService detection, ILogService log) : this()
        {
            _detection = detection;
            _log = log;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _log.OnLog += AppendLog;
            _detection.Camera.StatusChanged += _ => BeginInvoke(new Action(RefreshUI));
            _detection.PLC.StatusChanged += _ => BeginInvoke(new Action(RefreshUI));
            _detection.Camera.FrameArrived += OnFrameArrived;
            _detection.DetectionCompleted += OnDetectionCompleted;

            RefreshUI();
            _log.Info("系统启动完成");
        }

        private void AppendLog(string msg)
        {
            if (txtLog.InvokeRequired)//表示控件不在主线程
            {
                BeginInvoke(new Action(() => AppendLog(msg)));
                return;
            }
            txtLog.AppendText(msg + Environment.NewLine);
        }

        /// <summary>所有按钮灰/亮的唯一真相源</summary>
        private void RefreshUI()
        {
            bool cam = _detection.CameraStatus == DeviceStatus.Connected;
            bool plc = _detection.PLCStatus == DeviceStatus.Connected;

            lblCameraStatus.Text = "相机：" + (cam ? "已连接" : "未连接");
            lblCameraStatus.ForeColor = cam ? Color.Green : Color.Red;
            lblPLCStatus.Text = "PLC：" + (plc ? "已连接" : "未连接");
            lblPLCStatus.ForeColor = plc ? Color.Green : Color.Red;

            btnConnectCamera.Enabled = !cam;
            btnDisconnectCamera.Enabled = cam;
            btnConnectPLC.Enabled = !plc;                          
            btnDisconnectPLC.Enabled = plc;                        
            btnStart.Enabled = cam && plc && !_detection.IsRunning;
            btnStop.Enabled = _detection.IsRunning;
        }

        /// <summary>相机实时预览（后台线程来，要切UI线程）</summary>
        private void OnFrameArrived(ImageData img)
        {
            BeginInvoke(new Action(() =>
            {
                picPreview.Image?.Dispose();
                picPreview.Image = ToBitmap(img);
            }));
        }

        /// <summary>Mono8 byte[] → Bitmap（UI显示用）</summary>
        private Bitmap ToBitmap(ImageData d)
        {
            var bmp = new Bitmap(d.Width, d.Height, PixelFormat.Format8bppIndexed);
            var palette = bmp.Palette;
            for (int i = 0; i < 256; i++) palette.Entries[i] = Color.FromArgb(i, i, i);
            bmp.Palette = palette;

            var rect = new Rectangle(0, 0, d.Width, d.Height);
            var bd = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            Marshal.Copy(d.Pixels, 0, bd.Scan0, d.Pixels.Length);
            bmp.UnlockBits(bd);
            return bmp;
        }

        /// <summary>每帧检测结果刷新（来自后台线程）</summary>
        private void OnDetectionCompleted(InspectionResult result, ushort productId, string imagePath)
        {
            BeginInvoke(new Action(() =>
            {
                bool ok = result.IsAllOk;
                lbResult.Text = ok ? "OK" : "NG";
                lbResult.ForeColor = ok ? Color.LimeGreen : Color.Red;
                lbTime.Text = $"ProductID: {productId}   耗时: {result.ElapseMs} ms";   // ★ ElapsedMs

                dgvRecord.Rows.Clear();
                foreach (var item in result.Items)
                {
                    int i = dgvRecord.Rows.Add();
                    dgvRecord.Rows[i].Cells[0].Value = item.Name;
                    dgvRecord.Rows[i].Cells[1].Value = item.Value;
                    dgvRecord.Rows[i].Cells[2].Value = item.IsOk ? "√ OK" : "× NG";
                    if (!item.IsOk)
                        dgvRecord.Rows[i].DefaultCellStyle.BackColor = Color.MistyRose;
                }
            }));
        }

        // ==================== 相机 ====================

        private async void btnConnectCamera_Click(object sender, EventArgs e)
        {
            btnConnectCamera.Enabled = false;
            _log.Info($"正在连接相机 ");
            try { await _detection.Camera.ConnectAsync(); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                _log.Error("相机连接失败", ex);
                //MessageBox.Show(ex.Message);
            }
            finally { RefreshUI(); }
        }

        private async void btnDisconnectCamera_Click(object sender, EventArgs e)
        {
            btnDisconnectCamera.Enabled = false;
            await _detection.Camera.DisconnectAsync();
            _log.Info("相机 已断开");
            RefreshUI();
        }

        // ==================== PLC ====================

        private async void btnConnectPLC_Click(object sender, EventArgs e)
        {
            btnConnectPLC.Enabled = false;
            _log.Info($"正在连接PLC {PlcIp} rack={PlcRack} slot={PlcSlot} ...");
            try
            {
                await _detection.PLC.ConnectAsync(PlcIp, PlcRack, PlcSlot);
                _log.Info("PLC 已连接");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "PLC连接失败");
                _log.Error("PLC连接失败", ex);
            }
            finally { RefreshUI(); }
        }

        private async void btnDisconnectPLC_Click(object sender, EventArgs e)
        {
            btnDisconnectPLC.Enabled = false;
            await _detection.PLC.DisconnectAsync();
            _log.Info("PLC 已断开");
            RefreshUI();
        }

        // ==================== 检测流程 ====================

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (_detection.CameraStatus != DeviceStatus.Connected)
            { MessageBox.Show("请先连接相机"); return; }
            if (_detection.PLCStatus != DeviceStatus.Connected)
            { MessageBox.Show("请先连接PLC"); return; }

            _detection.StartPolling(PlcIp, PlcRack, PlcSlot);
            RefreshUI();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _detection.StopPolling();
            RefreshUI();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _detection.StopPolling();
            _ = _detection.Camera.DisconnectAsync();
            _ = _detection.PLC.DisconnectAsync();
        }
    }
}
