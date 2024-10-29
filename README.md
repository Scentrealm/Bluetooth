# Bluetooth

# 气味小播开放能力

蓝牙相关协议

## 功能

- 获取设备信息
- 获取胶囊信息
- 播放、停止播放 气味
- 气味闹钟
- 气味混合
- 睡眠模式

## 目前蓝牙最为普遍使用的有两种规格：

- **蓝牙基础率/增强数据率 (Bluetooth Basic Rate/Enhanced Data Rate, BR/EDR)**: 也称为经典蓝牙。常用在对数据传输带宽有一定要求的场景上，比如需要传输音频数据的蓝牙音箱、蓝牙耳机等；
- **蓝牙低功耗 (Bluetooth Low Energy, BLE)**: 从蓝牙 4.0 起支持的协议，特点就是功耗极低、传输速度更快，常用在对续航要求较高且只需小数据量传输的各种智能电子产品中，比如智能穿戴设备、智能家电、传感器等，应用场景广泛。

*气味小播使用 BLE。*

## 蓝牙协议

[蓝牙协议](./Bluetooth.md)


## 注意事项

每个蓝牙外围设备都有唯一的 `deviceId`来标识。由于部分系统实现的限制，对于同一台蓝牙外围设备，在不同中心设备上扫描获取到的 `deviceId`可能是变化的。因此 `deviceId`不能硬编码到代码中。


### CRC校验函数 使用多项式 0xA001;C#/C++算法如下所示
```c
public static ushort CalcCrc(byte[] data) {
  int len = data.Length;
  if (len > 0) {
    ushort crc = 0xFFFF;
    for (int i = 0; i < len; i++) {
      crc = (ushort)(crc ^ (data[i]));
      for (int j = 0; j < 8; j++) {
        crc = (crc & 1) != 0 ? (ushort)((crc >> 1)^ 0xA001):(ushort)(crc >> 1);
      }
    }

    byte hi = (byte)((crc & 0xFF00) >> 8); //高位置
    byte lo = (byte)(crc & 0x00FF); //低位置
    return (ushort)(hi * 256 + lo);
  }
  return 0;
}
```

### 电量计算（根据锂电池电压对比计算）
```javascript

// 锂电池的电量是根据电压进行换算的
const VOLTAGE_LIST = [
  {
    voltage: "3.00",
    percent: 0,
  },
  {
    voltage: "3.45",
    percent: 0.05,
  },
  {
    voltage: "3.68",
    percent: 0.1,
  },
  {
    voltage: "3.74",
    percent: 0.2,
  },
  {
    voltage: "3.77",
    percent: 0.3,
  },
  {
    voltage: "3.79",
    percent: 0.4,
  },
  {
    voltage: "3.82",
    percent: 0.5,
  },
  {
    voltage: "3.87",
    percent: 0.6,
  },
  {
    voltage: "3.92",
    percent: 0.7,
  },
  {
    voltage: "3.98",
    percent: 0.8,
  },
  {
    voltage: "4.06",
    percent: 0.9,
  },
  {
    voltage: "4.20",
    percent: 1,
  },
];

function toPercent(num) {
  let val = Math.round(Number(num) * 100);
  if (val >= 100) {
    val = 100;
  }
  return val + '%';
};

/*
 * 根据电压计算电量百分比：
 * 1. 低于 3v 显示 0%
 * 2. 超过 4.1 显示100%
 * 3. 刚好返回是这几个固定值之一，显示对应的百分比
 * 4. 在两个值之间，譬如（返回 4.08， 在  4.06 和 4.2 之间）
4.06 到 4.2，对应的电量是0.9到1， 那么每单位电压对应的变化就是  (1-0.9)/(4.2-4.06)，4.08 对应的电量就是  (1-0.9)/(4.2-4.06) * (4.08-4.06) + 0.9
 * 5. 电量四舍五入向上取整
*/
function getVoltage(vol) {
  let list = VOLTAGE_LIST;
  let obj = list.find((ele) => ele.voltage == vol);

  if (obj) {
    return toPercent(obj.percent);
  } else {
    if (vol <= list[0].voltage) {
      //电压小于等于 3v,电量 0
      return '0%';
    } else if (vol >= 4.1) {
      //电压大于4.1 就可以显示 100%
      return '100%';
    } else {
      let result = 0;
      list.map((item, index) => {
        if (vol > item.voltage && vol < list[index + 1].voltage) {
          let vx = Number(list[index + 1].voltage) - Number(item.voltage);
          let vy = Number(list[index + 1].percent) - Number(item.percent);
          result =
            Number(list[index + 1].percent) -
            (vy * (Number(list[index + 1].voltage) - vol)) / vx;
        }
      });
      return toPercent(result);
    }
  }
};
```

### 完整操作 Demo
```python

import asyncio
from bleak import BleakClient, BleakScanner
from bleak.backends.characteristic import BleakGATTCharacteristic
from binascii import unhexlify
from crcmod import crcmod

# 设备的Characteristic UUID
par_notification_characteristic = "6e400003-b5a3-f393-e0a9-e50e24dcca9e"
# 设备的Characteristic UUID（具备写属性Write）
par_write_characteristic = "6e400002-b5a3-f393-e0a9-e50e24dcca9e"
# 设备的MAC地址
par_device_addr = "78:E3:6D:08:B5:AE"

FRAME_HEAD = 'F5'
FRAME_TAIL = '55'


def crc16Add(str_data):
    crc16 = crcmod.mkCrcFun(0x18005, rev=True, initCrc=0xFFFF, xorOut=0x0000)
    data = str_data.replace(" ", "")
    readcrcout = hex(crc16(unhexlify(data))).upper()
    str_list = list(readcrcout)
    if len(str_list) < 6:
        str_list.insert(2, '0'*(6-len(str_list)))  # 位数不足补0
    crc_data = "".join(str_list)
    return crc_data[2:4]+' '+crc_data[4:]


def ten2sixteen(num, length):
    """
    十进制转十六进制
    :param num: 十进制数字
    :param length: 字节长度
    :return:
    """
    data = str(hex(eval(str(num))))[2:]
    data_len = len(data)
    if data_len % 2 == 1:
        data = '0' + data
        data_len += 1

    sixteen_str = "00 " * (length - data_len//2) + data[0:2] + ' ' + data[2:]
    return sixteen_str.strip()


def cmd2bytearray(cmd_str: str):
    verify = crc16Add(cmd_str)
    cmd = FRAME_HEAD + ' ' + cmd_str + ' ' + verify + ' ' + FRAME_TAIL
    print(cmd)
    return bytearray.fromhex(cmd)


def device_capluse():
    """
    获取设备气路胶囊信息
    :return:
    """
    cmd_data = '00 00 00 01 0E 01 06 00 00'
    return cmd2bytearray(cmd_data)


def start_play(scent: int, playtime: int):
    play_cmd = '00 00 00 01 02 05'
    scent_channel = ten2sixteen(scent, 1)
    if playtime == 0:  # 一直播放
        playtime16 = 'FF FF FF FF'
    else:
        playtime16 = ten2sixteen(playtime, 4)
    cmd_data = play_cmd + ' ' + scent_channel + ' ' + playtime16
    return cmd2bytearray(cmd_data)


def stop_play():
    """
    停止播放
    :return:
    """
    stop_cmd = '00 00 00 01 00 01 00'
    return cmd2bytearray(stop_cmd)


def status_check():
    """
    检查工作状态
    :return:
    """
    status_cmd = '00 00 00 01 11 01 00 00 00'
    return cmd2bytearray(status_cmd)


async def scan_devices():
    """
    扫描蓝牙设备
    :return:
    """
    devices = await BleakScanner.discover()
    for d in devices:  # d为类，其属性有：d.name为设备名称，d.address为设备地址
        print(d)


# 监听回调函数，此处为打印消息
def notification_handler(characteristic: BleakGATTCharacteristic, data: bytearray):
    # print("rev data:", data)
    print("rev data bytes2hex:", ' '.join(['%02x' % b for b in data]))


async def main():
    print("starting scan...")
    # 基于MAC地址查找设备
    device = await BleakScanner.find_device_by_address(
        par_device_addr, cb=dict(use_bdaddr=False)
    )
    if device is None:
        print("could not find device with address '%s'" % par_device_addr)
        return

        # 事件定义
    disconnected_event = asyncio.Event()

    # 断开连接事件回调
    def disconnected_callback(client):
        print("Disconnected callback called!")
        disconnected_event.set()

    print("connecting to device...")
    async with BleakClient(device, disconnected_callback=disconnected_callback) as client:
        print("Connected")
        await client.start_notify(par_notification_characteristic, notification_handler)

        await client.write_gatt_char(par_write_characteristic, device_capluse())  # 获取设备气路胶囊信息
        await asyncio.sleep(2.0)

        scent = 5  # 播放气路数
        playtime = 18000  # 播放时长，单位ms
        await client.write_gatt_char(par_write_characteristic, start_play(scent, playtime))  # 发送开始播放指令

        await asyncio.sleep(10.0)
        await client.write_gatt_char(par_write_characteristic, stop_play())  # 发送停止播放指令

        await asyncio.sleep(10.0)
        await client.write_gatt_char(par_write_characteristic, status_check())  # 检查设备工作状态

        await client.stop_notify(par_notification_characteristic)
        await client.disconnect()


asyncio.run(main())

```


### 多路混香 Demo
```python

total_v = 255  # 总体积 255 ML
channel_ratio = [0, 10, 30, 0, 30, 50]  # 表示第一路不播放，第二路10%，第三路30%，第四路不播放，第五路30%，第六路50%

cmd_list = [0, 0, 0, 0, 0, 0]

play_channel_number_total = 0
play_channel_ratio_total = 0

for ratio in channel_ratio:
    if ratio != 0:
        play_channel_number_total += 1
        play_channel_ratio_total += ratio

if play_channel_ratio_total == 0:
    print("请至少选择一路播放")
    exit()

channel_avg_v = 255 / play_channel_ratio_total
for key, ratio in enumerate(channel_ratio):
    if ratio != 0:
        cmd_list[key] = int(ratio * channel_avg_v)

print(cmd_list)

```
