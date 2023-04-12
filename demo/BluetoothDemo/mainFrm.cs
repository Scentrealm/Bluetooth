using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Linq;

namespace SKII.BluetoothAuto
{
    public partial class mainFrm : Form
    {
        BluetoothCore bleCore = null;// new BluetoothCore();//

        List<BluetoothLEDevice> DeviceList = new List<BluetoothLEDevice>();

        List<GattCharacteristic> GattCharacteristics = new List<GattCharacteristic>();

        List<GattDeviceService> GattDeviceServices = new List<GattDeviceService>();

        private CheckState chkState = CheckState.None;

        private bool searching = false;//正在搜索？

        private bool chkStart = false;

        private Thread thCheck = null;

        private bool chkflag = false;
        private int tryTimes = 0;
        private string currentCmd = string.Empty;
        private int currentChl = 1;//当前通道
        private string cmdStr = "";

        private Dictionary<string, string> DictData = new Dictionary<string, string>();

        int devCnt = 0;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern Int32 SendMessage(IntPtr hwnd, Int32 wMsg, Int32 wParam, Int32 lParam);

        const int LVM_FIRST = 0x1000;
        const int LVM_SETICONSPACING = LVM_FIRST + 53;

        /// <summary>
        /// 设置图标间隔
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetSpacing(Int16 x, Int16 y)
        {
            SendMessage(this.lvDevices.Handle, LVM_SETICONSPACING, x, y);
            //SendMessage(this.lvZhuType.Handle, LVM_SETICONSPACING, 0, x * 65536 + y);
            this.lvDevices.Refresh();
        }

        public mainFrm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            //SetSpacing(15, 15);
        }

        public static void RunAsyn(Action action)
        {
            ((Action)(delegate ()
            {
                action.Invoke();
            })).BeginInvoke(null, null);
        }

        private void mainFrm_Load(object sender, EventArgs e)
        {
            AppManager.CreateInstance().Init();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (this.btnSearch.Text == "搜索设备")
            {
                try
                {
                    if (bleCore != null)
                    {
                        bleCore.Dispose();
                        bleCore = null;
                        GC.Collect();
                    }
                    bleCore = new BluetoothCore();
                    bleCore.MessageChanged += BleCore_MessageChanged;
                    bleCore.DeviceWatcherChanged += BleCore_DeviceWatcherChanged;
                    bleCore.GattDeviceServiceAdded += BleCore_GattDeviceServiceAdded;
                    bleCore.CharacteristicAdded += BleCore_CharacteristicAdded;
                    bleCore.OnDataReceive += BleCore_OnDataReceive;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                InitShow();
                if (thCheck != null)
                {
                    thCheck.Abort();
                    thCheck = null;
                }
                btnAutoCheck.Text = "连接设备";
                this.bleCore.StartBleDeviceWatcher();
                searching = true;
                this.btnSearch.Text = "停止搜索";
            }
            else
            {
                try
                {
                    this.bleCore.StopBleDeviceWatcher();
                    searching = false;
                    this.btnSearch.Text = "搜索设备";
                }
                catch
                {
                }
            }
        }

        #region BleCore方法

        private void BleCore_OnDataReceive(StructProtocol protocol)
        {
            bool bTrueData = false;
            lock(DictData)
            {
                if (DictData.ContainsKey(protocol.CmdString))
                {
                    DictData[protocol.CmdString] = protocol.CmdString;
                }
                else
                {
                    DictData.Add(protocol.CmdString, protocol.CmdString);
                }
            }
            switch (protocol.CmdString)
            {
                case "11":
                    cmdStr = "11";
                    currentCmd = "11";
                    bTrueData = true;
                    this.Invoke((EventHandler)delegate
                    {
                        try
                        {
                            if (protocol.PayLoad.Count > 0)
                            {
                                double fPower = protocol.PayLoad[0] * 0.02;
                            }
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    });
                    break;
                case "0E":
                    cmdStr = "0E";
                    currentCmd = "0E";
                    bTrueData = true;
                    //try
                    //{
                    //    string hexstr = string.Join("", protocol.PayLoad.Select(x=>x.ToString("X2")));
                    //    Console.WriteLine("OE: " + hexstr);
                    //    if(hexstr.Length >= 18)
                    //    {
                    //        for(int i=0;i<6;i++)
                    //        {
                    //            string hexScentid = hexstr.Substring(i * 3, 3);
                    //            int iScentId = int.Parse(hexScentid, System.Globalization.NumberStyles.HexNumber);
                    //            SetChlShow(i, iScentId);
                    //        }
                    //    }
                    //}
                    //catch
                    //{
                    //}
                    break;
                case "02":
                    cmdStr = "02";
                    currentCmd = "02";
                    bTrueData = true;
                    break;
                case "0B":
                    cmdStr = "0B";
                    currentCmd = "0B";

                    try
                    {
                        if (protocol.PayLoad != null && protocol.PayLoad.Count > 0)
                        {
                            if(protocol.PayLoad.Count >= 0x07) //判断
                            {
                                Console.WriteLine("读取时钟");
                                CmdData cmddata = AppManager.CreateInstance().CheckCases[cmdIndex];
                                int week = (byte)DateTime.Now.DayOfWeek;
                                if ((protocol.PayLoad[0] + 2000) == DateTime.Now.Year &&
                                   (protocol.PayLoad[1] == DateTime.Now.Month) &&
                                   protocol.PayLoad[2] == DateTime.Now.Day && protocol.PayLoad[3] == week &&
                                   (protocol.PayLoad[4] == DateTime.Now.Hour || protocol.PayLoad[4] == DateTime.Now.Hour + 1)) //
                                {
                                    bTrueData = true;
                                }
                                else
                                {
                                    Console.WriteLine(protocol.ReceiveString);
                                    protocol.ReceiveString = string.Empty;
                                    bTrueData = false;
                                }
                            }
                            else
                            {
                                Console.WriteLine("写时钟反馈");
                                if (protocol.PayLoad[0] == 0x01 || protocol.PayLoad[0] == 0x00)
                                {
                                    bTrueData = true;
                                }
                                else
                                {
                                    bTrueData = false;
                                }
                            }
                        }
                        else
                        {
                            bTrueData = false;
                        }
                    }
                    catch
                    {
                        bTrueData = false;
                    }
                    break;
                case "00":
                    cmdStr = "00";
                    currentCmd = "00";
                    bTrueData = true;
                    break;

                default:

                    break;
            }
            if(bTrueData)
            {
                if (cmdIndex < AppManager.CreateInstance().CheckCases.Count)
                {
                    AppManager.CreateInstance().CheckCases[cmdIndex].AnswerTotal += 1;
                    AppManager.CreateInstance().CheckCases[cmdIndex].AnswerStr = protocol.ReceiveString;
                    AppManager.CreateInstance().CheckCases[cmdIndex].CheckResult = "PASS";
                }
            }
        }
        private void BleCore_CharacteristicAdded(GattCharacteristic gattCharacteristic)
        {
            RunAsyn(() =>
            {
                this.GattCharacteristics.Add(gattCharacteristic);
            });
        }

        private void BleCore_GattDeviceServiceAdded(GattDeviceService gattDeviceService)
        {
            RunAsyn(() =>
            {
                this.GattDeviceServices.Add(gattDeviceService);
            });
        }

        private void BleCore_DeviceWatcherChanged(MsgType type, BluetoothLEDevice bluetoothLEDevice)
        {
            this.Invoke((EventHandler)delegate
            {
                string devName = bluetoothLEDevice.Name;
                if (devName.Length > 5 && devName.Substring(0, 5) == "scent")
                {
                    var dev = DeviceList.Find(x => x.DeviceId == bluetoothLEDevice.DeviceId);
                    if (dev == null)
                    {
                        ListViewItem viewItem = new ListViewItem();
                        viewItem.ImageIndex = 0;
                        viewItem.Text = bluetoothLEDevice.Name.Replace("scent","S=");
                        viewItem.Tag = bluetoothLEDevice;
                        viewItem.UseItemStyleForSubItems = false;

                        lvDevices.Items.Add(viewItem);
                        devCnt++;
                        DeviceList.Add(bluetoothLEDevice);
                    }
                }
            });
        }

        private void BleCore_MessageChanged(MsgType type, string message, byte[] data = null)
        {
            if (type == MsgType.BleDevice)
            {
                txtOprate.Text = message + "\r\n";
            }
            else if (type == MsgType.ChkState)
            {
                switch (message)
                {
                    case "00":

                        break;
                    case "01": //
                        chkState = CheckState.DevMatched;
                        break;
                    case "02": //获取服务完成
                        chkState = CheckState.EndService;
                        break;
                    case "03":
                        chkState = CheckState.EndFeatures;
                        break;
                    case "40":
                        chkState = CheckState.Connected;
                        break;
                }
            }
            else if (type == MsgType.Test)
            {
                if (txtShow.Text.Length > 800)
                {
                    txtShow.Text = string.Empty;
                }
                txtShow.Text += message + "\r\n";
            }
            else if (type == MsgType.BleData)
            {
                if (txtShow.Text.Length > 800)
                {
                    txtShow.Text = string.Empty;
                }
                txtShow.Text += message + "\r\n";
            }
            else
            {
                if (txtOprate.Text.Length > 500)
                {
                    //tShow.Text = string.Empty;
                }
                txtOprate.Text += message + "\r\n";
            }
        }

        #endregion End


        #region 公共方法
        private void InitShow()
        {
            this.lvDevices.Items.Clear();
            try
            {
                foreach (var item in DeviceList)
                {
                    item.Dispose();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                DeviceList.Clear();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            GattCharacteristics.Clear();
            GattDeviceServices.Clear();
            txtOprate.Text = string.Empty;
            txtShow.Text = string.Empty;
        }

        private void CheckComplete()
        {
            try
            {
                bleCore.LinkClose();
                GattCharacteristics.Clear();
                GattDeviceServices.Clear();

            }
            catch
            {
            }

            chkflag = false;
            tryTimes = 0;
            currentCmd = string.Empty;
            cmdStr = string.Empty;
            currentChl = 1;
            chkStart = false;
            btnAutoCheck.Text = "连接设备";
            cmdIndex = 0;
        }
        /// <summary>
        /// 单次完成
        /// </summary>
        private void ChkSingleComplete()
        {
            try
            {
                GattCharacteristics.Clear();
                GattDeviceServices.Clear();
                bleCore.LinkClose();

                //BleCore_MessageChanged(MsgType.Test, "单个测试完成！");
            }
            catch
            {
            }
            chkflag = false;
            tryTimes = 0;
            currentCmd = string.Empty;
            cmdStr = string.Empty;
            cmdIndex = 0;
        }
        #endregion End

        private void mainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            bleCore?.Dispose();
        }

        private void btnAutoCheck_Click(object sender, EventArgs e)
        {
            if (searching)
            {
                MessageBox.Show("正在搜索中，请停止搜索后进行连接检测。。。");
                return;
            }

            if(chkStart)
            {
                MessageBox.Show("正在连接。。。");
                return;
            }

            if (btnAutoCheck.Text == "连接设备")
            {
                if (thCheck != null)
                {
                    thCheck.Abort();
                    thCheck = null;
                }

                if (lvDevices.SelectedItems.Count > 0)
                {
                    BluetoothLEDevice device = lvDevices.SelectedItems[0].Tag as BluetoothLEDevice;

                    if (device != null)
                    {
                        ChkSingleComplete();

                        txtShow.Text = string.Empty;
                        //chkState = CheckState.None;
                        bleCore.StartMatching(device);

                        chkState = CheckState.DevSelected;

                        if (thCheck != null)
                        {
                            thCheck.Abort();
                            thCheck = null;
                        }
                        btnAutoCheck.Text = "停止连接";
                        chkStart = true;
                        thCheck = new Thread(CheckSingleLink);
                        thCheck.IsBackground = true;
                        thCheck.Name = "checkbluetooth";
                        thCheck.Start();

                    }
                    else
                    {
                        MessageBox.Show("未检测到有效设备！");
                    }
                }
                else
                {
                    MessageBox.Show("请选择连接的设备蓝牙。");
                }
            }
            else
            {
                if (thCheck != null)
                {
                    thCheck.Abort();
                    thCheck = null;
                }

                CheckComplete();
                chkflag = false;
                tryTimes = 0;
                currentCmd = string.Empty;
                cmdStr = string.Empty;
                chkStart = false;
                btnAutoCheck.Text = "连接设备";
            }
        }

        int cmdIndex = 0;//发送了几条指令

        /// <summary>
        /// 连接
        /// </summary>
        private void CheckSingleLink()
        {
            BluetoothLEDevice device = bleCore.CurrentDevice;//先给赋值一个
            if(device == null)
            {
                this.Invoke((EventHandler)delegate
                {
                    BleCore_MessageChanged(MsgType.NotifyTxt, "请选择需要测试的设备！");
                });
                return;
            }

            while (chkStart)
            {
                try
                {
                    switch (chkState)
                    {
                        case CheckState.None:
                            chkflag = false;
                            //chkState = CheckState.DevSelected;
                            break;
                        case CheckState.Searching:
                            if (!chkflag)
                            {
                                this.bleCore.FindServiceByAsync();
                                chkflag = true;//已经启动查找
                            }
                            else
                            {
                                chkflag = false;//已经启动查找
                                chkState = CheckState.DevSelected;
                            }
                            break;
                        case CheckState.Searched:
                            chkflag = false;//已经启动查找
                            chkState = CheckState.DevSelected;
                            break;
                        case CheckState.DevSelected:
                            //手动去选择匹配
                            chkState = CheckState.DevMatched;
                            chkflag = false;
                            break;
                        case CheckState.DevMatched:
                            chkState = CheckState.GetServiceUUID;
                            chkflag = false;
                            break;
                        case CheckState.GetServiceUUID:
                            if (!chkflag)
                            {
                                GattDeviceServices.Clear();
                                this.bleCore.FindServiceByAsync();
                                chkflag = true;
                            }
                            else
                            {
                                if (GattDeviceServices.Count > 0)
                                {
                                    chkState = CheckState.EndService;
                                    tryTimes = 0;
                                }
                                else
                                {
                                    tryTimes++;
                                    Thread.Sleep(1000);
                                    if (tryTimes > 10)
                                    {
                                        //this.Invoke((EventHandler)delegate { });
                                        this.Invoke((EventHandler)delegate
                                        {
                                            BleCore_MessageChanged(MsgType.Test, "未找到模块服务！");

                                            //CheckComplete();
                                            chkState = CheckState.CheckCompleted;
                                        });
                                        //chkStart = false;//跳出测试
                                        Console.WriteLine("No Service!");
                                        tryTimes = 0;
                                    }
                                }
                            }
                            break;
                        case CheckState.EndService:
                            chkState = CheckState.GetFeaturesUUID;
                            chkflag = false;
                            tryTimes = 0;
                            break;
                        case CheckState.GetFeaturesUUID:
                            if (!chkflag)
                            {
                                if (GattDeviceServices.Count > 0)
                                {
                                    Console.WriteLine("进入搜索特征码判断！");
                                    foreach (GattDeviceService gdService in GattDeviceServices)
                                    {
                                        if (gdService.Uuid.ToString().ToLower() == "6E400001-B5A3-F393-E0A9-E50E24DCCA9E".ToLower())
                                        {
                                            GattCharacteristics.Clear();
                                            this.bleCore.FindCharacteristic(gdService);
                                            chkflag = true;
                                            break;
                                        }
                                    }
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    chkState = CheckState.GetServiceUUID;
                                    chkflag = false;
                                }
                            }
                            else
                            {
                                tryTimes++;
                                Thread.Sleep(1000);
                                if (tryTimes > 8)
                                {
                                    this.Invoke((EventHandler)delegate
                                    {
                                        //chkStart = false;
                                        BleCore_MessageChanged(MsgType.Test, "未找到模块读写特征码！");

                                        chkState = CheckState.CheckCompleted;
                                        //CheckComplete();
                                    });
                                    //chkStart = false;//跳出测试
                                    Console.WriteLine("No Character!");
                                    tryTimes = 0;
                                }
                            }
                            break;
                        case CheckState.EndFeatures:
                            chkState = CheckState.Connecting;
                            chkflag = false;
                            tryTimes = 0;
                            break;
                        case CheckState.Connecting:
                            int linkNum = 0;
                            foreach (GattCharacteristic gatt in GattCharacteristics)
                            {
                                this.bleCore.SetOpteron(gatt);
                                Thread.Sleep(500);
                            }
                            //--
                            foreach (GattCharacteristic gatt in GattCharacteristics)
                            {
                                if (gatt.CharacteristicProperties == GattCharacteristicProperties.Notify ||
                                    gatt.CharacteristicProperties == GattCharacteristicProperties.Read ||
                                    gatt.CharacteristicProperties == GattCharacteristicProperties.Write)
                                {
                                    //Console.WriteLine("Uuid:" + gatt.Uuid.ToString());
                                    linkNum++;
                                    continue;
                                }
                            }
                            if (linkNum >= 2)
                            {
                                chkState = CheckState.Connected;
                                chkflag = false;
                                tryTimes = 0;
                                BleCore_MessageChanged(MsgType.Test, "连接成功！");
                            }
                            else
                            {
                                chkState = CheckState.Connected;
                                chkflag = false;
                                tryTimes = 0;
                                BleCore_MessageChanged(MsgType.Test, "连接失败，请重连！");
                            }
                            break;
                        case CheckState.Connected:
                            chkflag = false;
                            tryTimes = 0;
                            chkState = CheckState.CheckCompleted;
                            break;
                        case CheckState.CheckCompleted:

                            //CheckComplete();
                            chkflag = false;
                            tryTimes = 0;
                            currentCmd = string.Empty;
                            cmdStr = string.Empty;
                            currentChl = 1;
                            chkStart = false;
                            this.Invoke((EventHandler)delegate{
                                btnAutoCheck.Text = "连接设备";
                            });

                            cmdIndex = 0;

                            Thread.Sleep(1000);
                            break;
                        default:
                            break;
                    }
                }
                catch
                {

                }

            }
        }

        private void btnSList_Click(object sender, EventArgs e)
        {
            if (searching)
            {
                MessageBox.Show("正在搜索中，请停止搜索后进行查询。。。");
                return;
            }
            string devName = txtSearch.Text.Trim();
            if(string.IsNullOrEmpty(devName))
            {
                lvDevices.Items.Clear();
                foreach(var item in DeviceList)
                {
                    ListViewItem viewItem = new ListViewItem();
                    viewItem.ImageIndex = 0;
                    viewItem.Text = item.Name.Replace("scent", "S=");
                    viewItem.Tag = item;
                    viewItem.UseItemStyleForSubItems = false;

                    lvDevices.Items.Add(viewItem);
                }
                lvDevices.Refresh();
            }
            else
            {
                var devs = DeviceList.FindAll(x => x.Name.Contains(devName));
                if(devs != null && devs.Count > 0)
                {
                    lvDevices.Items.Clear();
                    foreach (var item in devs)
                    {
                        ListViewItem viewItem = new ListViewItem();
                        viewItem.ImageIndex = 0;
                        viewItem.Text = item.Name.Replace("scent", "S=");
                        viewItem.Tag = item;
                        viewItem.UseItemStyleForSubItems = false;

                        lvDevices.Items.Add(viewItem);
                    }
                    lvDevices.Refresh();
                }
                else
                {
                    MessageBox.Show("未搜索到对应设备！");
                }
            }
        }

        private void btnBuildPlay_Click(object sender, EventArgs e)
        {
            int iChl = int.Parse(txtChannel.Text);
            int iTime = int.Parse(txtTime.Text);
            iTime = iTime * 1000;//毫秒
            if (iChl < 1 || iChl > 255)
            {
                MessageBox.Show("气味通道编号大于0且不超过255！");
                return;
            }
            byte bchl = (byte)iChl;

            byte[] tbstime = BitConverter.GetBytes(iTime).Reverse().ToArray();

            List<byte> lCmd = new List<byte>();
            List<byte> lSend = new List<byte>();

            lCmd.Add(0x00);//本机地址
            lCmd.Add(0x00);//本机地址
            lCmd.Add(0x00);//小播地址
            lCmd.Add(0x01);//小播地址

            lCmd.Add(0x02);//指令码

            lCmd.Add(0x05);//数据长度

            lCmd.Add(bchl);//通道

            lCmd.AddRange(tbstime);//播放时间

            lSend.Add(0xF5);
            lSend.AddRange(lCmd);
            lSend.AddRange(Crc16.CalcCrc(lCmd.ToArray()));
            lSend.Add(0x55);

            //Console.WriteLine(string.Join(" ", lSend.Select(x => x.ToString("X2"))));

            txtCmd.Text = string.Join(" ", lSend.Select(x => x.ToString("X2")));

            SendCmd(lSend.ToArray());
        }

        private void btnBuildStop_Click(object sender, EventArgs e)
        {
            string cmdStr = "F5 00 00 00 00 00 01 00 90 1A 55";
            txtCmd.Text = cmdStr;

            byte[] bCmd = AppManager.CreateInstance().HexStr2Bytes(cmdStr);

            if (bCmd == null || bCmd.Length == 0)
            {
                return;
            }

            SendCmd(bCmd);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtCmd.Text.Trim()))
            {
                return;
            }

            byte[] bCmd = AppManager.CreateInstance().HexStr2Bytes(txtCmd.Text.Trim());

            if(bCmd == null || bCmd.Length == 0)
            {
                return;
            }

            SendCmd(bCmd);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void SendCmd(byte[] data)
        {
            try
            {
                if(data == null || data.Length == 0)
                {
                    return;
                }

                if(this.bleCore != null && this.bleCore.CurrentDevice != null && this.bleCore.CurrentDevice.ConnectionStatus == BluetoothConnectionStatus.Connected)
                {
                    _ = this.bleCore.Write(data);
                }
                else
                {
                    MessageBox.Show("未连接或者连接已断开，请重新连接播放！");
                }
            }
            catch
            { }
        }
    }

    /// <summary>
    /// 检测状态
    /// </summary>
    public enum CheckState : int
    {
        None,
        /// <summary>
        /// 正在查找设备
        /// </summary>
        Searching,
        /// <summary>
        /// 完成查找设备
        /// </summary>
        Searched,

        DevSelected,
        /// <summary>
        /// 完成设备匹配
        /// </summary>
        DevMatched,
        /// <summary>
        /// 获取服务UUID
        /// </summary>
        GetServiceUUID,
        /// <summary>
        /// 完成服务特征码搜索
        /// </summary>
        EndService,
        /// <summary>
        /// 搜索特征码
        /// </summary>
        GetFeaturesUUID,
        /// <summary>
        /// 完成特征码
        /// </summary>
        EndFeatures,
        /// <summary>
        /// 正在连接设置与连接
        /// </summary>
        Connecting,
        /// <summary>
        /// 连接完成
        /// </summary>
        Connected,
        /// <summary>
        /// 可读写状态
        /// </summary>
        ReadWrite,
        /// <summary>
        /// 等待回复状态
        /// </summary>
        WaitResponse,
        /// <summary>
        /// 完成测试
        /// </summary>
        CheckCompleted
    }
}
