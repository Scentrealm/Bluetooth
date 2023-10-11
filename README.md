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

## 协议总体结构说明

### 0.1 系统构成

设备终端模块通过 RS232/RS485/TCP 或者 4G/WIFI 等无线连接到 PC/手机平板或者 云服务端，协议在应用层

![System](img/system.png)

### 0.2 通信参数约定

使用串口通信时，通信参数：19200,N,8,1

### 0.3 数据帧结构

| 项名称 | 包头 | 源地址 | 目标地址 | 指令码 | 数据长度 | 数据 | 校验 | 包尾 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 字节数 | 1 | 2 | 2 | 1 | 1 | N<256 | 2 | 1 |
| 示例 | 0xF5 | 0x00,0x00 | 0x02,0x03 | 0x01 | 0x01 | 0x00 | 0x07 | 0x55 |

### 0.4 帧结构说明

| 名称 | 说明 |  |
| --- | --- | --- |
| 帧头 | 固定数据0xF5 |  |
| 源地址 | 2字节，从0到0xFFFF，其中地址0xFFFF表示所有设备 |  |
| 目标地址 | 2字节，从0到0xFFFF，其中地址0xFFFF表示所有设备，广播数据包 |  |
| 指令码 | 1字节，详细下面章节说明 |  |
| 数据长度 | 1字节，表示帧数据段的数据长度，最大255 |  |
| 数据 | N(N<256)字节,表示指令参数 |  |
| 校验 | 2字节，使用CRC校验，校验方法见附录 | 计算包括从源地址到数据段 |
| 帧尾 | 固定数据0x55 |  |

参数化控制气味播放器播放，多气路循环、交替控制、轮流播放，CRC校验去包头和包尾及校验4字节除外。

### 0.5 设备类型与地址

| 服务端设备类型 | 代码(单个16进制) | 下端设备类型 | 代码(单个16进制) |
| --- | --- | --- | --- |
| PC上位机端 | 0x01 | 脖戴式气味播放器 | 0x01 |
| 同步器 | 0x02 | 气味钢琴 | 0x02 |
| 手机端(BLE) | 0x03 | 数字香水 | 0x03 |
| 阿里云部署服务 | 0x04 | 车载模块 | 0x04 |
| IOT | 0x08 |  |  |

在发送数据地址中，为了方便业务区分，增加标识，如本机类型PC，下发到车载模块，合成本机设备地址高位为 0x14, 如果上位机是IOT系统，则本机设备地址高位为 0x84。本机设备低地址为序号(如IOT向车载设备发送指令，IOT序号为0x01则本机设备地址为0x84,0x01)，未使用到可自行定义本机设备地址。

# 常用指令编码

## 1. 停止播放/重启与初始化

描述

停止当前播放或者重启，或者初始化参数

主机请求

| 字节序号 | 含义 | 示例 |
| --- | --- | --- |
| 0 | 包头 | 0xf5 |
| 1 | 本机设备地址高8位 | 0x84 IOT -> 车载模块(08->04) |
| 2 | 本机设备地址低8位 | 0x00 |
| 3 | 目标设备地址高8位 | 0x00 |
| 4 | 目标设备地址低8位 | 0x01 |
| 5 | 功能码 | 0x00 |
| 6 | 数据长度 | 0x01 |
| 7 | 功能标志 | 0x00:停止;0x01:初始化;0x02:重启 |
| 8 | 校验字高8位 | 0x00 |
| 9 | 校验字低8位 | 0x05 |
| 10 | 包尾 | 0x55 |

> 注：其中序号7功能标志段中占一字节；
值 0x00: 停止播放；0x01: 初始化；0x02：重启
>

从站响应

| 字节序号 | 含义 | 示例 |
| --- | --- | --- |
| 0 | 包头 | 0xF5 |
| 1 | 本机设备地址高8位 | 0x48 |
| 2 | 本机设备地址低8位 | 0x00 |
| 3 | 目标设备地址高8位 | 0x00 |
| 4 | 目标设备地址低8位 | 0x01 |
| 5 | 功能码 | 0x00 |
| 6 | 数据长度 | 0x01 |
| 7 | 执行结果标志 | 0x00执行成功;大于0x01执行失败 |
| 8 | 校验字高8位 | 0x00 |
| 9 | 校验字低8位 | 0x05 |
| 10 | 包尾 | 0x55 |

## 2. 播放气味高级（多路轮播循环）

描述

参数化控制气味播放器播放，多气路循环、交替控制、轮流播放，累加去包头和包尾及校验4字节除外。

主机请求

| 字节序号 | 含义 | 示例 |
| --- | --- | --- |
| 1 | 包头 | 0xf5 |
| 2 | 本机设备地址高8位 | 0x84 |
| 3 | 本机设备地址低8位 | 0x01 |
| 4 | 目标设备地址高8位 | 0x00 |
| 5 | 目标设备地址低8位 | 0x00 |
| 6 | 功能码 | 0x01 |
| 7 | 数据长度 |  |
| 8 | 有效数据 气味强度 | 0x00 |
| 9 |  | 0x01（1-3） |
| 10 | 播放气路数（N） | 0x01（1-255） |
| 11 | 气味编号1 | 0x00 |
| 12 | 气味编号1 | 0x01（1-12） |
| 13 | 气味编号2 | 0x00 |
| 14 | ……（根据气路数列表） | 0x01（1-12） |
| 15 | 播放时间总长（单位：ms） | 0x00 |
| 16 |  | 0x00 |
| 17 |  | 0x00 |
| 18 |  | 0x0a（10ms） |
| 19 | 预热时长（单位：ms） | 0x00 |
| 20 |  | 0x00 |
| 21 |  | 0x00 |
| 22 |  | 0x0a（10ms） |
| 23 | 每次播放时长（单位：ms） | 0x00 |
| 24 |  | 0x00 |
| 25 |  | 0x00 |
| 26 |  | 0x0a（10ms） |
| 27 | 每次间隔时长（单位：ms） | 0x00 |
| 28 |  | 0x00 |
| 29 |  | 0x00 |
| 30 |  | 0x0a（10ms） |
| 31 | 校验字高8位 | 0x00 |
| 32 | 校验字低8位 | 0x05 |
| 33 | 包尾 | 0x55 |

从站响应

| 字节序号 | 含义 | 示例 |
| --- | --- | --- |
| 1 | 包头 | 0xf5 |
| 2 | 本机设备地址高8位 | 0x84 |
| 3 | 本机设备地址低8位 | 0x00 |
| 4 | 目标设备地址高8位 | 0x00 |
| 5 | 目标设备地址低8位 | 0x00 |
| 6 | 功能码 | 0x01 |
| 7 | 数据长度 | 0x01 |
| 8 | 有效数据  操作结果 | 0x00 :执行成功; 0x01错误 |
| 9 | 校验字高8位 | 0x00 |
| 10 | 校验字低8位 | 0x05 |
| 11 | 包尾 | 0x55 |

## 3. 播放单路气味

描述

模块单路播放，当气味编号大于256时，目标地址低8位作为扩展气味编号高位。
注：在324路使用时，气味编号 = 分组个数(自定义 小于255) * 组数(目标地址低8位) + 气味编号字段数据；例如：气味编号217，分组个数108，则217=108*2+1(编码为目标地址低8位为0x02，气味编号字段为0x01)；

主机请求

| 字节序号 | 含义 | 示例 |
| --- | --- | --- |
| 0 | 包头 | 0xf5 |
| 1 | 本机设备地址高8位 | 0x84 |
| 2 | 本机设备地址低8位 | 0x01 |
| 3 | 目标设备地址高8位 | 0x00 |
| 4 | 目标设备地址低8位 | 0x00 当气路数>256 |
| 5 | 功能码 | 0x02 |
| 6 | 数据长度 | 0x05 |
| 7 | 气味编号 | 0x01 |
| 8 | 持续时间 | 0x00 |
| 9 | 持续时间 | 0x00 |
| 10 | 持续时间 | 0x00 |
| 11 | 持续时间 | 0x0A ms |
|  | 播放脚本编号 | 0x01 |
| 12 | 校验字高8位 | 0x00 |
| 13 | 校验字低8位 | 0x05 |
| 14 | 包尾 | 0x55 |

从站响应

| 字节序号 | 含义 | 示例 |
| --- | --- | --- |
| 0 | 包头 | 0xf5 |
| 1 | 本机设备地址高8位 | 0x84 |
| 2 | 本机设备地址低8位 | 0x00 |
| 3 | 目标设备地址高8位 | 0x00 |
| 4 | 目标设备地址低8位 | 0x00 |
| 5 | 功能码 | 0x02 |
| 6 | 数据长度 | 0x01 |
| 7 | 有效数据  操作结果 | 0x00 :成功;0x01~ :其它 |
| 8 | 校验字高8位 | 0x00 |
| 9 | 校验字高8位 | 0x05 |
| 10 | 包尾 | 0x55 |

## 4. 查询设备工作状态

描述

查询气味播放器的当前状态

主机请求

| 字节序号 | 含义 | 示例 |
| --- | --- | --- |
| 0 | 包头 | 0xf5 |
| 1 | 本机设备地址高8位 | 0x13 |
| 2 | 本机设备地址低8位 | 0x01 |
| 3 | 目标设备地址高8位 | 0x00 |
| 4 | 目标设备地址低8位 | 0x01 |
| 5 | 功能码 | 0x03 (查询状态) |
| 6 | 数据长度 | 0x00 |
| 7 | 校验字高8位 | 0x00 |
| 8 | 校验字低8位 | 0x05 |
| 9 | 包尾 | 0x55 |

从站响应

气味播放器响应中有效数据为1字节，包含设备状态（1字节）

| 设备状态 | 对应代码 |
| --- | --- |
| 待机 | 0x00 |
| 播放气味（高级） | 0x07 |
| 料芯故障 | 0x09 |
| 其他错误 | 0xff |
| 脚本不存在 | 0xfe |

| 字节 | 含义 | 示例 |
| --- | --- | --- |
| 0 | 包头 | 0xf5 |
| 1 | 本机设备地址高8位 | 0x13 |
| 2 | 本机设备地址低8位 | 0x01 |
| 3 | 目标设备地址高8位 | 0x00 |
| 4 | 目标设备地址低8位 | 0x01 |
| 5 | 功能码 | 0x03（查询状态） |
| 6 | 数据长度 | 3 |
| 7 | 有效数据   设备状态(参见设备状态表) | 0x00 |
| 8 | 气味编号 高字节 | 0x00 |
| 9 | 气味编号 低字节 | 0x01 |
| 10 | 已播放脚本时长(s) 高字节 | 0x00 |
| 11 | 已播放脚本时长(s) 低字节 | 0x01 |
| 12 | 脚本总时长(s) 高字节 | 0x01 |
| 13 | 脚本总时长(s) 低字节 | 0x05 |
| 14 | 校验字高 8 位 | 0x00 |
| 15 | 校验字低 8 位 | 0x05 |
| 16 | 包尾 | 0x55 |


## 5. 获取设备信息 0x16

描述

获取设备版本号

主机请求

| 字节序号 | 含义 | 示例 |
| --- | --- | --- |
| 0 | 包头 | 0xf5 |
| 1 | 本机设备地址高8位 | 0x18 |
| 2 | 本机设备地址低8位 | 0x00 |
| 3 | 目标设备地址高8位 | 0x00 |
| 4 | 目标设备地址低8位 | 0x01 |
| 5 | 功能码 | 0x16 |
| 6 | 数据长度 | 0x00 |
| 7 | 校验字高8位 | 0x00 |
| 8 | 校验字低8位 | 0x00 |
| 9 | 包尾 | 0x55 |

从站响应

| 字节 | 含义 | 示例 |
| --- | --- | --- |
| 0 | 包头 | 0xf5 |
| 1 | 本机设备地址高8位 | 0x81 |
| 2 | 本机设备地址低8位 | 0x01 |
| 3 | 目标设备地址高8位 | 0xff |
| 4 | 目标设备地址低8位 | 0xff |
| 5 | 功能码 | 0x16 |
| 6 | 数据长度 | 0x01 |
| 7 | 版本号 | 0x01 |
| 8 | 版本号 | 0x01 |
| 9 | 版本号 | 0x01 |
| 10 | 校验字高 8 位 | 0x00 |
| 11 | 校验字低 8 位 | 0x05 |
| 12 | 包尾 | 0x55 |

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
