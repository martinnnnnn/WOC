using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WOC_Core
{
    public class Session
    {
        public string ServerIP;
        public int ServerPort;
        //public Action<IPacketData> OnMessageReceived;
        public Action OnDisconnect;

        private TcpClient client = new TcpClient();
        private NetworkStream netstream;

        private CancellationTokenSource tokenSource;
        private CancellationToken token;
        private bool listening;

        public void Connect(TcpClient client)
        {
            LOG.Print("[NETWORK] Trying to setup client.");
            if (listening)
            {
                LOG.Print("[NETWORK] Socket already open, closing it.");
                Close();
            }

            try
            {
                this.client = client;
                netstream = client.GetStream();
                Task.Run(() => ListenAsync());
                LOG.Print("[NETWORK] Setup completed !");
            }
            catch (Exception e)
            {
                LOG.Print("[NETWORK] Failed to connect. {0}", e.Message);
            }
        }

        public void Connect(string ip, int port)
        {
            LOG.Print("[NETWORK] Trying to connect to {0}:{1}", ip, port);
            if (listening)
            {
                SendClose().Wait();
                Close();
                listening = false;
                Thread.Sleep(500);
            }

            ServerIP = ip;
            ServerPort = port;
            LOG.Print("[NETWORK] Connecting...");
            client = new TcpClient();
            try
            {
                client.Connect(ip, port);
                netstream = client.GetStream();
                Task.Run(() => ListenAsync());
                LOG.Print("[NETWORK] Connected !");
            }
            catch (Exception e)
            {
                LOG.Print("[NETWORK] Failed to connect. {0}", e.Message);
            }
        }

        public async Task ListenAsync()
        {
            tokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken());
            token = tokenSource.Token;
            listening = true;

            try
            {
                byte[] byteArray = new byte[1024];
                while (!token.IsCancellationRequested)
                {
                    var byteCount = await netstream.ReadAsync(byteArray, 0, byteArray.Length, token);
                    var msg = Encoding.UTF8.GetString(byteArray, 0, byteCount);

                    IPacketData data = Serialization.FromJson<IPacketData>(msg);

                    LOG.Print("[NETWORK] received from server:\n" + data);

                    HandleIncomingMessage(data);
                }
            }
            catch (Exception e)
            {
                LOG.Print("[NETWORK] Lost connection to server. " + e.Message);
            }
            finally
            {
                client.Close();
                listening = false;
                LOG.Print("[NETWORK] TCP socket closed.");
            }
        }

        public virtual void HandleIncomingMessage(IPacketData data)
        {
            switch (data)
            {
                case PD_SessionShutdown sd:
                    Close();
                    break;
            }
        }

        public async Task SendClose()
        {
            LOG.Print("[NETWORK] Sending shutdown message socket.");
            await SendAsync(new PD_SessionShutdown());
        }

        public void Close()
        {
            LOG.Print("[NETWORK] Closing socket.");
            tokenSource.Cancel();
            OnDisconnect?.Invoke();
            netstream.Close();
            client.Close();
        }

        public async Task SendAsync(string message)
        {
            try
            {
                var bytesMessage = Encoding.UTF8.GetBytes(message);
                await netstream.WriteAsync(bytesMessage, 0, bytesMessage.Length);
            }
            catch (Exception e)
            {
                LOG.Print("Failed to Send message : {0}", e.Message);
                Close();
            }
        }

        public async Task SendAsync(IPacketData data)
        {
            await SendAsync(Serialization.ToJson(data));
        }
        
        public async Task SendValidation(Guid toValidate, string errMessage = "")
        {
            await SendAsync(Serialization.ToJson(new PD_Validate(toValidate, errMessage)));
        }
    }
}
