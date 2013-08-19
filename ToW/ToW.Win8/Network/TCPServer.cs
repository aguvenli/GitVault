using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Windows.Networking.Sockets;
using Windows.Foundation;

namespace ToW.Win8.Network
{
    public class TCPServer
    {
        public TCPServer(int port)
        {
            this.Port = port;
        }
        StreamSocketListener serverSocket;
        StreamSocket clientSocket;

        int counter = 0;
        int Port = 0;
        bool c = false;

        private readonly object padlock = new object();

        
        public async void Start()
        {
            serverSocket = new StreamSocketListener();
            serverSocket.ConnectionReceived += serverSocket_ConnectionReceived;
            await serverSocket.BindServiceNameAsync("8080");
        }
        

        void serverSocket_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            StreamSocket clientSocket = args.Socket;
            Client cli = new Client(clientSocket);
            this.OnClientConnected(this, cli);
        }

        public event TypedEventHandler<TCPServer, Client> OnClientConnected;     
    }
}

