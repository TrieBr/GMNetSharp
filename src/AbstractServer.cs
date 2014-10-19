using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
namespace GMNetSharp
{
    public abstract class AbstractServer
    {
        //Socket for listening for incoming connections
        private Socket listenSocketTCP;
        //Handle for the thread this server is running on
        private Thread threadHandle;
        //Factory for making clients
        private AbstractClientFactory clientFactory;
        //List of clients connected
        private List<AbstractClient> clientList = new List<AbstractClient>();
        //Flag to stop the server/thread
        private bool stopFlag = false;
        //Start the Server
        public void Start(AbstractClientFactory factory)
        {
            clientFactory = factory;
            threadHandle = new Thread(new ThreadStart(this.Run));
            threadHandle.Start();
        }
        //Stop the Server
        public void Stop()
        {
            stopFlag = true;
        }
        //Disconnect (remove client from client list)
        public void ClientDisconnect(AbstractClient c)
        {
            clientList.Remove(c);
        }
        //Run method that is called on the thread
        private void Run()
        {
            Logger.Trace("Server Thread Started.");
            BeginListen(1337);  //Begin listening on port 1337
            Logger.Trace("Listening on port 1337");
            while (!stopFlag) Thread.Sleep(1000);
            Logger.Trace("Server Thread Stopping.");
        }
        //Begin listening on specified port
        private void BeginListen(short portNum)
        {
            listenSocketTCP = new Socket(SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, portNum);
            listenSocketTCP.Bind(ep);
            listenSocketTCP.Listen(100);
            //Asyncrounously accept new incoming connections
            listenSocketTCP.BeginAccept(
                new AsyncCallback(AcceptCallback),
                listenSocketTCP);
        }
        //Async callback for accepting new connections
        private void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            Socket newSocket = listener.EndAccept(ar);
            if (clientFactory != null)
            {
                Logger.Trace("New Client Accepted.");
                AbstractClient newClient = clientFactory.CreateClient(newSocket, this);
                clientList.Add(newClient);             
            }
            listenSocketTCP.BeginAccept(
                new AsyncCallback(AcceptCallback),
                listenSocketTCP);
        }


    }
}
