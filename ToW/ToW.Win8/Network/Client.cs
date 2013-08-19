using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace ToW.Win8.Network
{
    public class Client
    {
        StreamSocket Socket;
        OperationMode OperationMode;

        bool c = false;

        string ip;
        int port;

        public Client(StreamSocket soc)
        {
            Socket = soc;
            OperationMode = Network.OperationMode.AtServer;
            this.Listen();
        }

        public Client(string Ip, int Port)
        {
            this.OperationMode = Network.OperationMode.AtClient;
            ip = Ip;
            port = Port;    
        }

        public async void Connect(Game1 g)
        {
            Socket = new StreamSocket();
            Windows.Networking.HostName hn = new Windows.Networking.HostName(ip);
            await Socket.ConnectAsync(hn, port.ToString());
            g.c = true;
            this.Listen();
        }

        private async void Listen()
        {
            var dr = new DataReader(Socket.InputStream);
            //dr.InputStreamOptions = InputStreamOptions.Partial;
            var stringHeader = await dr.LoadAsync(4);

            if (stringHeader == 0)
            {
                //LogMessage(string.Format("Disconnected (from {0})", Socket.Information.RemoteHostName.DisplayName));
                return;
            }

            int strLength = dr.ReadInt32();

            uint numStrBytes = await dr.LoadAsync((uint)strLength);
            string msg = dr.ReadString(numStrBytes);

            Listen();
        }


        public void Send(Byte[] sendBytes)
        {
            Windows.Storage.Streams.DataWriter dw = new DataWriter(Socket.OutputStream);
            dw.WriteBytes(sendBytes);
        }

        public event TypedEventHandler<Client, IBuffer> OnMessageReceived;
    }


    public enum OperationMode
    {
        AtServer,
        AtClient,
    }
}
