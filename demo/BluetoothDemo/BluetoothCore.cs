using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Security.Cryptography;

namespace SKII.BluetoothAuto
{
    //关于蓝牙服务：其实可以直接过滤uuid使用标准协议uuid的前缀：0000ffff0
    //关于蓝牙特征：也是可以直接过滤uuid使用标准协议uuid的前缀：0000ffff1
    public class BluetoothCore
    {
        private bool asyncLock = false;

        private DeviceWatcher deviceWatcher;

        private bool searching = false;

        /// <summary>
        /// 当前连接的服务
        /// </summary>
        public GattDeviceService CurrentService { get; set; }
        /// <summary>
        /// 当前连接的蓝牙设备
        /// </summary>
        public BluetoothLEDevice CurrentDevice { get; set; }
        /// <summary>
        /// 写特征对象
        /// </summary>
        public GattCharacteristic CurrentWriteCharacteristic 
        { 
            get;
            set;
        }

        public GattCharacteristic CurrentNotifyCharacteristic { get; set; } //CurrentNotifyCharacteristic

        private const GattClientCharacteristicConfigurationDescriptorValue CHARACTERISTIC_NOTIFICATION_TYPE = GattClientCharacteristicConfigurationDescriptorValue.Notify;

        /// &lt;summary&gt;
        /// 存储检测到的设备
        /// &lt;/summary&gt;
        private List<BluetoothLEDevice> DeviceList = new List<BluetoothLEDevice>();

        private BluetoothLEAdvertisementWatcher watcher;

        public delegate void DeviceWatcherChangedEvent(MsgType type, BluetoothLEDevice bluetoothLEDevice);

        public event DeviceWatcherChangedEvent DeviceWatcherChanged;

        public delegate void GattDeviceServiceAddedEvent(GattDeviceService gattDeviceService);

        public event GattDeviceServiceAddedEvent GattDeviceServiceAdded;

        public delegate void CharacteristicAddedEvent(GattCharacteristic gattCharacteristic);

        public event CharacteristicAddedEvent CharacteristicAdded;

        public delegate void MessageChangedEvent(MsgType type, string message, byte[] data = null);

        public event MessageChangedEvent MessageChanged;

        public delegate void OnDataReceiveEvent(StructProtocol protocol);

        public event OnDataReceiveEvent OnDataReceive;


        private string CurrentDeviceMAC { get; set; }

        public BluetoothCore()
        {

        }

        /// &lt;summary&gt;
        /// 搜索蓝牙设备
        /// &lt;/summary&gt;
        public void StartBleDeviceWatcher()
        {
            try
            {
                if(watcher != null)
                {
                    watcher.Received -= Watcher_Received;
                    watcher = null;
                }
            }
            catch
            {
            }

            DeviceList.Clear();
            watcher = new BluetoothLEAdvertisementWatcher();
            watcher.ScanningMode = BluetoothLEScanningMode.Active;
            watcher.SignalStrengthFilter.InRangeThresholdInDBm = -125; //
            watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -125; //
            watcher.Received += Watcher_Received;
            watcher.Stopped += Watcher_Stopped;
            watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(5000);
            watcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(2000);
            watcher.Start();
            searching = true;
            string msgStr = "自动搜索设备中...";
            this.MessageChanged(MsgType.NotifyTxt, msgStr);
        }

        private void Watcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            string msg = "自动发现设备停止";
            this.MessageChanged(MsgType.NotifyTxt, msg);
        }

        /// <summary>
        /// 设备查找
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress).Completed = async (asyncInfo, asyncStatus) => 
            { 
                if(asyncStatus == AsyncStatus.Completed)
                {
                    if(asyncInfo.GetResults() == null)
                    {
                        //this.MessAgeChanged(MsgType.NotifyTxt, "没有搜索到结果\r\n");
                        Console.WriteLine("没有搜索到结果\r\n");
                    }
                    else
                    {
                        BluetoothLEDevice currentDevice = asyncInfo.GetResults();
                        bool contain = false;
                        lock(DeviceList)
                        {
                            foreach (BluetoothLEDevice device in DeviceList)
                            {
                                if (device.DeviceId == currentDevice.DeviceId)
                                {
                                    contain = true;
                                }
                            }
                            if (!contain)
                            {
                                byte[] bytes1 = BitConverter.GetBytes(currentDevice.BluetoothAddress);
                                Array.Reverse(bytes1);
                                //string devName = currentDevice.Name;
                                //if (devName.Length > 5 && devName.Substring(0, 5) == "scent")
                                //{
                                //    lock (DeviceList)
                                //    {
                                //        this.DeviceList.Add(currentDevice);
                                //    }
                                //}

                                //这边不做过滤
                                lock (DeviceList)
                                {
                                    this.DeviceList.Add(currentDevice);
                                }

                                if (searching)
                                {
                                    this.MessageChanged(MsgType.NotifyTxt, "发现设备：" + currentDevice.Name + "   Address:" + BitConverter.ToString(bytes1, 2, 6).Replace('-', ':').ToLower() + "\r\n");
                                    this.DeviceWatcherChanged(MsgType.BleDevice, currentDevice);
                                }
                            }
                            else
                            {
                                try
                                {
                                    this.DeviceWatcherChanged(MsgType.BleDevice, currentDevice);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
            };
        }
        /// <summary>
        /// 停止搜索
        /// </summary>
        public void StopBleDeviceWatcher()
        {
            watcher?.Stop();
            try
            {
                if (watcher != null)
                {
                    watcher.Received -= Watcher_Received;
                    watcher = null;
                    GC.Collect(); //需要及时清理，不然重新查找设备时找不到已经查找过的
                }
            }
            catch
            {
            }
            searching = false;
        }

        public void LinkCheck()
        {
            lock(DeviceList)
            {
                try
                {
                    if(CurrentDevice != null)
                    {
                        DeviceList.Remove(CurrentDevice);
                    }
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// 选择设备并匹配
        /// </summary>
        /// <param name="Device"></param>
        public void StartMatching(BluetoothLEDevice Device)
        {
            this.CurrentDevice = Device;
            this.MessageChanged(MsgType.ChkState, "01");//匹配
            this.MessageChanged(MsgType.Test, "匹配完成");
        }

        public async void FindService()
        {
            var GattServices = this.CurrentDevice.GattServices;
            foreach (GattDeviceService ser in GattServices)
            {
                this.GattDeviceServiceAdded(ser);
            }
        }
        /// <summary>
        /// 发现服务
        /// </summary>
        public async void FindServiceByAsync()
        {
            this.CurrentDevice.GetGattServicesAsync().Completed = async (asyncInfo, asyncStatus) =>
            {
                if(asyncStatus == AsyncStatus.Completed)
                {
                    var services = asyncInfo.GetResults().Services;
                    this.MessageChanged(MsgType.Test, "GattServices size=" + services.Count);
                    try
                    {
                        //this.GattDeviceServiceAdded -= this.GattDeviceServiceAdded;
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    foreach (GattDeviceService gattDevice in services)
                    {
                        this.GattDeviceServiceAdded(gattDevice);
                    }
                    if(services.Count> 0)
                    {
                        this.MessageChanged(MsgType.ChkState, "02");//服务特征码
                    }
                    this.MessageChanged(MsgType.Test, "获取服务收集完成");
                }
            };
        }
        /// <summary>
        /// 发现特征码
        /// </summary>
        /// <param name="gattDeviceService"></param>
        public async void FindCharacteristic(GattDeviceService gattDeviceService)
        {

            this.CurrentService = gattDeviceService;
            //var items = gattDeviceService.GetAllCharacteristics();
            //var item = gattDeviceService.GetCharacteristicsAsync();

            this.CurrentService.GetCharacteristicsAsync().Completed = async (asyncInfo,asycnStatus) => 
            { 
                if(asycnStatus == AsyncStatus.Completed)
                {
                    var charactics = asyncInfo.GetResults().Characteristics;
                    try
                    {
                       // this.CharacteristicAdded -= this.CharacteristicAdded;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    foreach (var c in charactics)
                    {
                        this.CharacteristicAdded(c);
                    }
                    if(charactics.Count > 0)
                    {
                        this.MessageChanged(MsgType.ChkState, "03");//通信特征码
                        this.MessageChanged(MsgType.Test, "GattCharacteristic size=" + charactics.Count);
                    }
                    else
                    {
                        try
                        {
                            //lock(gattDeviceService)
                            //{
                            //    var items = gattDeviceService.GetAllCharacteristics();
                            //    foreach (var c in items)
                            //    {
                            //        this.CharacteristicAdded(c);
                            //    }
                            //    this.MessAgeChanged(MsgType.NotifyTxt, "GattCharacteristic Size=" + items.Count);
                            //}
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    this.MessageChanged(MsgType.Test, "获取特码收集完成");
                }
            };
        }
        /// <summary>
        /// 设置连接
        /// </summary>
        /// <param name="gattCharacteristic"></param>
        /// <returns></returns>
        public async Task SetOpteron(GattCharacteristic gattCharacteristic)
        {
            if (gattCharacteristic.CharacteristicProperties == GattCharacteristicProperties.Write)
            {
                this.CurrentWriteCharacteristic = gattCharacteristic;
            }
            if (gattCharacteristic.CharacteristicProperties == GattCharacteristicProperties.Notify)
            {
                this.CurrentNotifyCharacteristic = gattCharacteristic;
            }
            if ((uint)gattCharacteristic.CharacteristicProperties == 26)
            { }

            if(gattCharacteristic.CharacteristicProperties == GattCharacteristicProperties.Write)
            {
                this.CurrentWriteCharacteristic = gattCharacteristic;

            }
            else if (gattCharacteristic.CharacteristicProperties == GattCharacteristicProperties.Notify)
            {
                this.CurrentNotifyCharacteristic = gattCharacteristic;
                this.CurrentNotifyCharacteristic.ProtectionLevel = GattProtectionLevel.Plain;
                try
                {
                    this.CurrentNotifyCharacteristic.ValueChanged -= Characteristic_ValueChanged;
                }
                catch(Exception ex)
                {
                }

                this.CurrentNotifyCharacteristic.ValueChanged += Characteristic_ValueChanged;

                await this.EnableNotifications(CurrentNotifyCharacteristic);
            }

            this.Connect();

            this.MessageChanged(MsgType.NotifyTxt, "完成连接设置！");
        }

        private async Task Connect()
        {
            byte[] _Bytes1 = BitConverter.GetBytes(this.CurrentDevice.BluetoothAddress);
            Array.Reverse(_Bytes1);
            this.CurrentDeviceMAC = BitConverter.ToString(_Bytes1, 2, 6).Replace('-', ':').ToLower();

            string msg = "正在连接设备" + this.CurrentDeviceMAC + ">.." + string.Join(" " ,_Bytes1.Select(x=> x.ToString("X2")));
            this.MessageChanged(MsgType.NotifyTxt, msg);
            this.CurrentDevice.ConnectionStatusChanged += this.CurrentDevice_ConnectionStatusChanged;
        }

        public void LinkClose()
        {
            try
            {
                //return;
                //this.CurrentDevice?.Dispose();
                //this.CurrentDevice = null;
                CurrentDeviceMAC = string.Empty;
                CurrentService?.Dispose();
                CurrentService = null;
                CurrentWriteCharacteristic = null;
                CurrentNotifyCharacteristic = null;
            }
            catch
            {

            }
        }

        private async Task Matching(string Id)
        {
            try
            {
                BluetoothLEDevice.FromIdAsync(Id).Completed = async (asyncInfo, asyncStatus) =>
                {
                    if (asyncStatus == AsyncStatus.Completed)
                    {
                        BluetoothLEDevice bleDevice = asyncInfo.GetResults();
                        lock(this.DeviceList)
                        {
                            this.DeviceList.Add(bleDevice);
                        }
                        this.DeviceWatcherChanged(MsgType.BleDevice, bleDevice);
                    }
                };
            }
            catch (Exception e)
            {
                string msg = "没有发现设备" + e.ToString();
                this.MessageChanged(MsgType.NotifyTxt, msg);
                this.StartBleDeviceWatcher();
            }
        }

        public void Dispose()
        {
            this.CurrentDevice?.Dispose();
            this.CurrentDevice = null;
            CurrentDeviceMAC = string.Empty;
            CurrentService?.Dispose();
            CurrentService = null;
            CurrentWriteCharacteristic = null;
            CurrentNotifyCharacteristic = null;
            //MessAgeChanged(MsgType.NotifyTxt, "主动断开连接");
        }

        private void CurrentDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            if (sender.ConnectionStatus == BluetoothConnectionStatus.Disconnected && CurrentDeviceMAC != null)
            {
                //string msg = "设备已断开,正在重连中...";//,自动重连
                //MessAgeChanged(MsgType.NotifyTxt, msg);
                if (!asyncLock)
                {
                    asyncLock = true;
                    this.CurrentDevice?.Dispose();
                    this.CurrentDevice = null;
                    CurrentService = null;
                    CurrentWriteCharacteristic = null;
                    CurrentNotifyCharacteristic = null;
                    SelectDeviceFromIdAsync(CurrentDeviceMAC);
                }
            }
            else
            {
                //string msg = "设备已连接";
                //MessAgeChanged(MsgType.NotifyTxt, msg);
            }
        }
        /// <summary>
        /// 重新查找新的设备
        /// </summary>
        /// <param name="MAC"></param>
        /// <returns></returns>
        public async Task SelectDeviceFromIdAsync(string MAC)
        {
            CurrentDeviceMAC = MAC;
            CurrentDevice = null;
            BluetoothAdapter.GetDefaultAsync().Completed = async(asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    BluetoothAdapter mBluetoothAdapter = asyncInfo.GetResults();
                    
                    byte[] _Bytes1 = BitConverter.GetBytes(mBluetoothAdapter.BluetoothAddress);//ulong转换为byte数组
                    Array.Reverse(_Bytes1);
                    string macAddress = BitConverter.ToString(_Bytes1, 2, 6).Replace("-", ":").ToLower();
                    string Id = "BluetoothLE#BluetoothLE" + macAddress + "-" + MAC;
                    await Matching(Id);
                }
                else
                {
                    string msg = "查找连接蓝牙设备异常。";
                }
            };
        }
        //当前设备PC已经已经连接或者配对成功的列表，从PC中寻找这个MAC配对，如果PC未配对则无法找到设备
        public async Task SelectDevice(string mac)
        {
            CurrentDeviceMAC = mac;
            CurrentDevice = null;
            DeviceInformation.FindAllAsync(BluetoothLEDevice.GetDeviceSelector()).Completed = async (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    DeviceInformationCollection deviceInformations = asyncInfo.GetResults();
                    foreach(DeviceInformation di in deviceInformations)
                    {
                        await Matching(di.Id);
                    }
                    if(CurrentDevice == null)
                    {
                        string msg = "没有发现设备";
                        StartBleDeviceWatcher();
                    }
                }
            };
        }

        public async Task EnableNotifications(GattCharacteristic characteristic)
        {
            string msg = "收通知对象=" + CurrentDevice.ConnectionStatus;
            this.MessageChanged(MsgType.NotifyTxt, msg);

            characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(CHARACTERISTIC_NOTIFICATION_TYPE).Completed = async (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    GattCommunicationStatus status = asyncInfo.GetResults();
                    if (status == GattCommunicationStatus.Unreachable)
                    {
                        msg = "设备不可用";
                        this.MessageChanged(MsgType.NotifyTxt, msg);
                        if (CurrentNotifyCharacteristic != null && !asyncLock)
                        {
                            await this.EnableNotifications(CurrentNotifyCharacteristic);
                        }
                    }
                    asyncLock = false;
                    msg = "设备连接状态" + status;
                    this.MessageChanged(MsgType.NotifyTxt, msg);
                    return;
                }
            };
        }

        /// <summary>
        /// 收到数据解析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            byte[] data;
            CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out data);
            string str = BitConverter.ToString(data);
            this.MessageChanged(MsgType.BleData, str, data);
            if(data != null && data.Length >= 10)
            {
                if(data[0] == 0xF5)
                {
                    int frLen = data[6] + 9;
                    if(frLen  < data.Length)
                    {
                        if(data[frLen] == 0x55)
                        {
                            if(OnDataReceive != null)
                            {
                                byte[] dest = new byte[data[6]];
                                Array.Copy(data, 7, dest, 0, dest.Length);//

                                StructProtocol protocol = new StructProtocol();
                                protocol.CmdCode = data[5];
                                protocol.CmdString = data[5].ToString("X2");
                                protocol.DataStr = BitConverter.ToString(dest).Replace('-',' ');
                                protocol.PayLoad = new List<byte>();
                                protocol.PayLoad.AddRange(dest);
                                protocol.ReceiveString = str;
                                OnDataReceive.Invoke(protocol);
                            }
                        }
                    }
                }
            }
        }

        /// &lt;summary&gt;
        /// 发送数据接口
        /// &lt;/summary&gt;
        /// &lt;returns&gt;&lt;/returns&gt;
        public async Task Write(byte[] data)
        {
            if (CurrentWriteCharacteristic != null)
            {
                CurrentWriteCharacteristic.WriteValueAsync(CryptographicBuffer.CreateFromByteArray(data), GattWriteOption.WriteWithResponse);
                string str = "发送数据:" + BitConverter.ToString(data);
                this.MessageChanged(MsgType.BleData, str, data);
            }
        }
    }

    public struct StructProtocol
    {
        /// <summary>
        /// 指令字符串
        /// </summary>
        public string CmdString;

        /// <summary>
        /// 指令代码
        /// </summary>
        public byte CmdCode;

        /// <summary>
        /// 数据段
        /// </summary>
        public List<byte> PayLoad;
        /// <summary>
        /// 
        /// </summary>
        public string DataStr;
        /// <summary>
        /// 收到的全部数据
        /// </summary>
        public string ReceiveString;
    }

    public class CmdData
    {
        /// <summary>
        /// 16进制指令字符串
        /// </summary>
        public string CmdStr;
        /// <summary>
        /// 指令描述
        /// </summary>
        public string CmdDesc;
        /// <summary>
        /// 
        /// </summary>
        public byte CmdCode;
        /// <summary>
        /// 数据集
        /// </summary>
        public List<byte> CmdBytes = new List<byte>();
        /// <summary>
        /// 回复的字符串
        /// </summary>
        public string AnswerStr;
        /// <summary>
        /// 需要总条数
        /// </summary>
        public int NeedAnswerSum = 0;
        /// <summary>
        /// 是否需要重构指令
        /// </summary>
        public bool NeedReBuild = false;

        /// <summary>
        /// 回复总条数
        /// </summary>
        public int AnswerTotal = 0;
        /// <summary>
        /// 检测结果
        /// </summary>
        public string CheckResult;
    }

    /// <summary>
    /// 
    /// </summary>
    public enum MsgType
    {
        /// <summary>
        /// 通知
        /// </summary>
        NotifyTxt,

        /// <summary>
        /// 
        /// </summary>
        BleDevice,
        /// <summary>
        /// 数据
        /// </summary>
        BleData,
        /// <summary>
        /// 测试状态
        /// </summary>
        ChkState,
        /// <summary>
        /// 测试
        /// </summary>
        Test
    }
}
