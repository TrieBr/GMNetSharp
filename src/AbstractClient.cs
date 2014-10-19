using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace GMNetSharp
{
    public class AbstractClient
    {
        // Size of receive buffer.
        private const int BufferSize = 1024;
        // Receive buffer.
        private byte[] buffer = new byte[BufferSize];
        //Socket for this client
        private Socket socket;
        //parentServer
        private AbstractServer parentServer;
        //Message dispatcher to dispatch message ID's to handlers
        private PacketDispatcher messageDispatcher = new PacketDispatcher();
        //Constructor
        public AbstractClient(Socket sock, AbstractServer parent)
        {
            parentServer = parent;
            socket = sock;
            sock.BeginReceive(buffer, 0, BufferSize, 0,
                new AsyncCallback(ReadCallback), this);
        }
        //Get the server controlling this client
        protected AbstractServer GetParentServer()
        {
            return parentServer;
        }
        //Get the socket for this client
        protected Socket GetSocket() {
            return socket;
        }
        //Get the message dispatcher for this client
        protected PacketDispatcher GetMessageDispatcher()
        {
            return messageDispatcher;
        }
        //Read callback for new incoming data
        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                // Read data from the client socket. 
                int bytesRead = socket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    OnDataReceived(buffer, bytesRead);
                }
                socket.BeginReceive(buffer, 0, BufferSize, 0, new AsyncCallback(ReadCallback), this);
            }
            catch (SocketException s)
            {
                if (s.SocketErrorCode == SocketError.ConnectionReset)
                {//Socket was disconnected by client
                    OnDisconnect();
                }
            }
        }
        //Send packet
        virtual public void SendPacket(PacketBuffer data)
        {
            socket.Send(data.GetData(), 0, data.GetLength(), 0);
        }
        //Data has been received
        virtual protected void OnDataReceived(byte[] data, int len)
        {

        }
        //Disconnected
        virtual protected void OnDisconnect()
        {
            GetParentServer().ClientDisconnect(this);
            Logger.Trace("Client disconnected.");
        }
        //Process an incoming packet
        virtual protected void OnPacketReceived(PacketBuffer p)
        {
            messageDispatcher.HandlePacket(p);
        }
    }
}
