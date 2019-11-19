using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WOC_Core
{
    public class Session
    {
        public Account account;

        public string ServerIP;
        public int ServerPort;
        public Action OnDisconnect;

        private TcpClient client = new TcpClient();
        private NetworkStream netstream;

        private CancellationTokenSource tokenSource;
        private CancellationToken token;
        private bool listening;

        public void Connect(TcpClient client)
        {
            Console.WriteLine("[NETWORK] Trying to setup client.");
            if (listening)
            {
                Console.WriteLine("[NETWORK] Socket already open, closing it.");
                Close();
            }

            try
            {
                this.client = client;
                netstream = client.GetStream();
                Task.Run(() => ListenAsync());
                Console.WriteLine("[NETWORK] Setup completed !");
            }
            catch (Exception e)
            {
                Console.WriteLine("[NETWORK] Failed to connect. {0}", e.Message);
            }
        }

        public void Connect(string ip, int port)
        {
            Console.WriteLine("[NETWORK] Trying to connect to {0}:{1}", ip, port);
            if (listening)
            {
                SendClose();
                Close();
                listening = false;
                Thread.Sleep(500);
            }

            ServerIP = ip;
            ServerPort = port;
            Console.WriteLine("[NETWORK] Connecting...");
            client = new TcpClient();
            try
            {
                client.Connect(ip, port);
                netstream = client.GetStream();
                Task.Run(() => ListenAsync());
                Console.WriteLine("[NETWORK] Connected !");
            }
            catch (Exception e)
            {
                Console.WriteLine("[NETWORK] Failed to connect. {0}", e.Message);
            }
        }

        public void ListenAsync()
        {
            tokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken());
            token = tokenSource.Token;
            listening = true;
            try
            {
                //byte[] buffer = new byte[2048];
                //int byteCount = 1;
                while (!token.IsCancellationRequested/* && byteCount > 0*/)
                {
                    byte[] sizeinfo = new byte[4];

                    //read the size of the message
                    int totalread = 0, currentread = 0;

                    currentread = totalread = netstream.Read(sizeinfo, 0, sizeinfo.Length);

                    while (totalread < sizeinfo.Length && currentread > 0)
                    {
                        currentread = netstream.Read(sizeinfo,
                                  totalread, //offset into the buffer
                                  sizeinfo.Length - totalread); //max amount to read

                        totalread += currentread;
                    }

                    int messagesize = 0;


                    //could optionally call BitConverter.ToInt32(sizeinfo, 0);
                    messagesize |= sizeinfo[0];
                    messagesize |= (((int)sizeinfo[1]) << 8);
                    messagesize |= (((int)sizeinfo[2]) << 16);
                    messagesize |= (((int)sizeinfo[3]) << 24);

                    byte[] data = new byte[messagesize];

                    //read the first chunk of data
                    totalread = 0;
                    currentread = totalread = netstream.Read(data,
                                 totalread, //offset into the buffer
                                data.Length - totalread); //max amount to read

                    //if we didn't get the entire message, read some more until we do
                    while (totalread < messagesize && currentread > 0)
                    {
                        currentread = netstream.Read(data,
                                 totalread, //offset into the buffer
                                data.Length - totalread); //max amount to read
                        totalread += currentread;
                    }

                    var msg = Encoding.UTF8.GetString(data, 0, totalread);

                    try
                    {
                        IPacketData packetData = Serialization.FromJson<IPacketData>(msg);
                        Console.WriteLine("[NETWORK] Received a packet : {0}", packetData);
                        HandleIncomingMessage(packetData);
                    }
                    catch (Exception e)
                    {
                        StackTrace stackTrace = new StackTrace();
                        Console.WriteLine("[NETWORK] Error while handling the message. " + e.Message);
                    }

                    //byteCount = netstream.Read(buffer, 0, buffer.Length);
                    //var msg = Encoding.UTF8.GetString(buffer, 0, byteCount);

                    //Console.WriteLine("{0} : {1}", byteCount, msg);
                    //IPacketData data = null;
                    //try
                    //{
                    //    data = Serialization.FromJson<IPacketData>(msg);
                    //    Console.WriteLine("[NETWORK] Received a packet : {0}", data);
                    //    HandleIncomingMessage(data);
                    //}
                    //catch (Exception e)
                    //{
                    //    StackTrace stackTrace = new StackTrace();
                    //    Console.WriteLine("[NETWORK] Error while handling the message. " + e.Message);
                    //}

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[NETWORK] Lost connection. " + e.Message);
            }
            finally
            {
                client.Close();
                listening = false;
                Console.WriteLine("[NETWORK] TCP socket closed.");
            }
        }

        public void HandleIncomingMessage(IPacketData data)
        {
            switch (data)
            {
                case PD_SessionShutdown sd:
                    Close();
                    break;
                default:
                    HandleAPICall(data);
                    break;
            }
        }

        public virtual void HandleAPICall(IPacketData data) {}

        public void SendClose()
        {
            Console.WriteLine("[NETWORK] Sending shutdown message socket.");
            Send(new PD_SessionShutdown(), true);
        }

        public void Close()
        {
            Console.WriteLine("[NETWORK] Closing socket.");
            tokenSource.Cancel();
            OnDisconnect?.Invoke();
            netstream.Close();
            client.Close();
        }

        public void Send(string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                byte[] sizeInfo = new byte[4];

                //could optionally call BitConverter.GetBytes(data.length);
                sizeInfo[0] = (byte)data.Length;
                sizeInfo[1] = (byte)(data.Length >> 8);
                sizeInfo[2] = (byte)(data.Length >> 16);
                sizeInfo[3] = (byte)(data.Length >> 24);

                netstream.Write(sizeInfo, 0, sizeInfo.Length);
                netstream.Write(data, 0, data.Length);

                //var bytesMessage = Encoding.UTF8.GetBytes(message);
                //netstream.Write(bytesMessage, 0, bytesMessage.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to Send message : {0}", e.Message);
                Close();
            }
        }

        public void Send(IPacketData data, bool force = false)
        {
            if ((account != null && account.connected) || force)
            {
                Console.WriteLine("[NETWORK] Sending a packet : {0}", data);
                Send(Serialization.ToJson(data));
            }
            else
            {
                Console.WriteLine("[NETWORK] Not authorized to send : {0}", data);
            }
        }
        
        public void SendValidation(Guid toValidate, string errMessage = "")
        {
            Send(Serialization.ToJson(new PD_Validation(toValidate, errMessage)));
        }
    }
}
