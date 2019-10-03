using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace FlightCommander.SerialCommunication
{
    public sealed class SerialBoss
    {
        private SerialBoss()
        {
            synchronizedStream = Stream.Synchronized(memoryStream);
        }

        /// <summary>
        /// 扫描当前可用串口。
        /// 注册 <code>SerialPortScanFinished</code> 事件以获取扫描结果。
        /// </summary>
        public void StartScanAsync()
        {
            FrameParser = new FrameParser();
            var handler = SerialPortScanFinished;
            Thread scanner = new Thread(() => {
                handler?.Invoke(this, new SerialPortScanFinishedEventArgs(SerialPort.GetPortNames()));
            });
            scanner.Start();
        }

        public void OpenPort(string name, int baud)
        {
            // defaulting to 8N1
            BytesSent = 0;
            BytesReceived = 0;
            activeSerialPort = new SerialPort(name, baud, Parity.None, 8, StopBits.One);
            activeSerialPort.DataReceived += (sender, data) =>
            {
                var bytes = new byte[activeSerialPort.ReadBufferSize];
                activeSerialPort.Read(bytes, 0, bytes.Length);
                SerialPortByteReceived?.Invoke(this, new SerialPortIncomingByteEventArgs(bytes));
                BytesReceived += bytes.Length;
            };
            activeSerialPort.ErrorReceived += (sender, ev) =>
            {
                // handle error here
                ClosePort();
            };
            blastingThread = new Thread(() =>
            {
                while (true)
                {
                    continueSignal.WaitOne();
                    byte[] buffer = new byte[16];
                    while (synchronizedStream.Length > 0)
                    {
                        int count = synchronizedStream.Read(buffer, 0, 16);
                        if (count > 0)
                        {
                            activeSerialPort.Write(buffer, 0, count);
                        }
                        BytesSent += count;
                    }
                }
            });
            blastingThread.IsBackground = true;
            blastingThread.Start();
            try
            {
                activeSerialPort.Open();
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("请以管理员身份运行此程序", "权限不足", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public void ClosePort()
        {
            activeSerialPort?.Close();
            blastingThread?.Abort();
        }

        public void SendByte(byte b)
        {
            synchronizedStream.WriteByte(b);
            continueSignal.Set();
        }

        public void SendBytes(byte[] bytes)
        {
            synchronizedStream.WriteAsync(bytes, 0, bytes.Length);
            continueSignal.Set();
        }

        public class SerialPortScanFinishedEventArgs : EventArgs
        {
            public SerialPortScanFinishedEventArgs(string[] list)
            {
                PortNames = new List<string>();
                PortNames.AddRange(list);
            }

            public List<string> PortNames;
        }

        public class SerialPortIncomingByteEventArgs : EventArgs
        {
            public SerialPortIncomingByteEventArgs(byte[] bytes)
            {
                Bytes = bytes;
            }

            public byte[] Bytes;
        }

        public int BytesSent { get; set; } = 0;

        public int BytesReceived { get; set; } = 0;

        public FrameParser FrameParser { get; private set; }

        public event EventHandler<SerialPortScanFinishedEventArgs> SerialPortScanFinished;
        public event EventHandler<SerialPortIncomingByteEventArgs> SerialPortByteReceived;
        public static SerialBoss Instance {
            get
            {
                if (me == null)
                {
                    me = new SerialBoss();
                }
                return me;
            }
        }
        private static SerialBoss me;
        private SerialPort activeSerialPort;
        private readonly MemoryStream memoryStream = new MemoryStream(128);
        private Stream synchronizedStream;
        private AutoResetEvent continueSignal = new AutoResetEvent(true);
        private Thread blastingThread;
    }
}
