using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Network
{
    public class Session
    {
        public TcpClient client;
        public NetworkStream netstream;

        // db port 3306 33060

        public Session(TcpClient tcpClient)
        {
            client = tcpClient;
            netstream = client.GetStream();
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
                Console.WriteLine("Failed to Send message : {0}", e.Message);
            }
        }

        public async Task StartAsync()
        {
            var buffer = new byte[4096];
            Console.WriteLine("[Server] Reading from client");
            while (true)
            {
                try
                {
                    var byteCount = await netstream.ReadAsync(buffer, 0, buffer.Length);
                    var request = Encoding.UTF8.GetString(buffer, 0, byteCount);

                    HandleIncoming(request);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to AsyncRead : {0}", ex.Message);
                    break;
                }
            }
        }

        protected virtual void HandleIncoming(string message) {}

        public async Task SendValidation(Guid toValidate, string errMessage = "")
        {
            await SendAsync(Utils.ToJson(new PD_Validate(toValidate, errMessage)));
        }

        public async Task SendValidation(Guid toValidate, string errMessage = "")
        {
            await SendAsync(PacketData.ToJson(new PD_Validate(toValidate, errMessage)));
        }
    }



}
