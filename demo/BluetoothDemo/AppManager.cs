using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SKII.BluetoothAuto
{
    public class AppManager
    {
        private static AppManager instance = null;

        private static object singleLock = new object(); //锁同步

        /// <summary>
        /// 创建单例
        /// </summary>
        /// <returns>返回单例对象</returns>
        public static AppManager CreateInstance()
        {
            lock (singleLock)
            {
                if (instance == null)
                {
                    instance = new AppManager();
                }
            }
            return instance;
        }

        public List<CmdData> CheckCases = new List<CmdData>();//要测试的指令集

        public int WaitTime = 2;

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version
        {
            get;
            set;
        }

        public void Init()
        {
            if (File.Exists("Config/ItemConfig.xml"))
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load("Config/ItemConfig.xml");
                    XmlNode chkNode = xmlDoc.SelectSingleNode("Configuration/CheckCases");

                    foreach (XmlNode subNode in chkNode.ChildNodes)
                    {
                        CmdData cmdData = new CmdData();
                        cmdData.NeedAnswerSum = int.Parse(subNode.Attributes["rtnNum"].Value.Trim());
                        cmdData.CmdStr = subNode.Attributes["cmdstr"].Value.Trim();
                        cmdData.CmdDesc = subNode.Attributes["cmddesc"].Value.Trim();
                        string cmdStrs = subNode.Attributes["cmd"].Value.Trim();

                        cmdData.NeedReBuild = bool.Parse(subNode.Attributes["rebuild"].Value.Trim());

                        cmdData.CmdBytes.AddRange(HexStr2Bytes(cmdStrs));
                        CheckCases.Add(cmdData);
                    }

                    XmlNode waitNode = xmlDoc.SelectSingleNode("Configuration/WaitTime");
                    WaitTime = int.Parse(waitNode.Attributes["value"].Value.Trim());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("文件不存在!");
            }
        }

        public byte[] HexStr2Bytes(string dataStr)
        {
            if(string.IsNullOrEmpty(dataStr))
            {
                return new byte[] { };
            }
            List<byte> lStr = new List<byte>();
            List<Byte> lData = new List<byte>();
            string[] sData1 = dataStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            return StrToByte(sData1);
        }

        private byte[] StrToByte(string[] d1)
        {
            if (d1 == null || d1.Length == 0)
            {
                return new byte[] { };
            }

            List<byte> lDa = new List<byte>();

            foreach (string str in d1)
            {
                lDa.Add(byte.Parse(str, System.Globalization.NumberStyles.HexNumber));
            }

            return lDa.ToArray();
        }
    }
}
