using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace FlightCommander.SerialCommunication
{
    public class Frame
    {
        /// <summary>
        /// 创建接收帧
        /// </summary>
        /// <param name="size"></param>
        /// <param name="cmdName"></param>
        /// <param name="data"></param>
        /// <param name="checksum"></param>
        public Frame(byte size, byte cmdName, byte[] data, byte checksum)
        {
            this.size = size;
            this.command = cmdName;
            this.data = data;
            this.checksum = checksum;
        }
        
        /// <summary>
        /// 创建发送帧
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        public Frame(byte command, byte[] data)
        {
            this.size = (byte) data.Length;
            this.command = command;
            this.data = data;
            this.checksum = (byte) (size ^ command ^ data.Aggregate((byte) 0, (acc, e) => acc ^= e));
        }

        /// <summary>
        /// 创建发送帧
        /// </summary>
        /// <param name="command"></param>
        public Frame(byte command)
        {
            this.size = 0;
            this.command = command;
            this.checksum = (byte) (size ^ command);
        }

        public bool IsValid
        {
            get
            {
                return checksum == (size ^ command ^ data.Aggregate((byte) 0, (acc, e) => acc ^= e));
            }
        }

        private byte size;
        private byte command;
        private byte[] data;
        private byte checksum;
    }

    public enum FrameState
    {
        IDLE,
        HEADER,
        DIRECTION,
        LENGTH,
        COMMAND,
        DATA,
        CHECKSUM,
        ERROR,
    }

    public class FrameParser
    {
        public FrameParser()
        {
            syncStream = Stream.Synchronized(memoryStream);
            SerialBoss.Instance.SerialPortByteReceived += (sender, serial) =>
            {
                syncStream.Write(serial.Bytes, 0, serial.Bytes.Length);
            };
            Thread emitterThread = new Thread(() =>
            {
                continueFlag.WaitOne();
                int currentByte = syncStream.ReadByte();
                if (currentByte != -1)
                {
                    switch (state)
                    {
                        case FrameState.ERROR:
                            ++errorCount;
                            goto case FrameState.IDLE; // C# quirks/features
                        case FrameState.IDLE:
                            // requires to be '$'
                            if (currentByte == '$')
                            {
                                state = FrameState.HEADER;
                            }
                            else
                            {
                                state = FrameState.ERROR;
                            }
                            break;
                        case FrameState.HEADER:
                            // requires to be 'M'
                            if (currentByte == 'M')
                            {
                                state = FrameState.DIRECTION;
                            }
                            else
                            {
                                state = FrameState.ERROR;
                            }
                            break;
                        case FrameState.DIRECTION:
                            // requires to be '>'
                            if (currentByte == '>')
                            {
                                state = FrameState.LENGTH;
                            }
                            else
                            {
                                state = FrameState.ERROR;
                            }
                            break;
                        case FrameState.LENGTH:
                            totalBytes = currentByte;
                            remainingBytes = currentByte;
                            if (remainingBytes > 0)
                            {
                                data = new byte[remainingBytes];
                            }
                            else
                            {
                                data = null;
                            }
                            state = FrameState.COMMAND;
                            break;
                        case FrameState.COMMAND:
                            command = currentByte;
                            state = FrameState.DATA;
                            break;
                        case FrameState.DATA:
                            if (remainingBytes <= 0)
                            {
                                goto case FrameState.CHECKSUM;
                            }
                            data[data.Length - remainingBytes] = (byte) currentByte;
                            --remainingBytes;
                            break;
                        case FrameState.CHECKSUM:
                            var frame = new Frame((byte)totalBytes, (byte)command, data, (byte)currentByte);
                            if (frame.IsValid)
                            {
                                ++frameCount;
                                FrameReady?.Invoke(this, new FrameReadyEventArgs(frame));
                                state = FrameState.IDLE;
                            }
                            else
                            {
                                state = FrameState.ERROR;
                            }
                            break;
                    }
                }
            });
        }

        public class FrameReadyEventArgs : EventArgs
        {
            public FrameReadyEventArgs(Frame frame)
            {
                Frame = frame;
            }

            public Frame Frame { get; }
        }

        public int ErrorCount
        {
            get
            {
                return errorCount;
            }
        }

        public int FrameCount
        {
            get
            {
                return frameCount;
            }
        }

        public void DiscardFrame()
        {
            state = FrameState.ERROR;
        }

        public event EventHandler<FrameReadyEventArgs> FrameReady;
        private volatile FrameState state = FrameState.IDLE;
        private int totalBytes = 0, remainingBytes = 0;
        private int command = 0;
        private byte[] data = null;
        private MemoryStream memoryStream = new MemoryStream();
        private Stream syncStream;
        private AutoResetEvent continueFlag = new AutoResetEvent(true);
        private int errorCount = 0;
        private int frameCount = 0;
    }
}
