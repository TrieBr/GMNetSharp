using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace GMNetSharp
{
    //Handshake status
    enum GMTCPHandshakeStage
    {
        Unknown,
        AwaitingConnectAck,
        Complete
    };
    /*
     * Client used to interact with GameMaker (GameMaker packet headers/connection handshake)
     * */
    public class GMTCPClient : AbstractClient
    {
        //Status of the GM Handshake
        private GMTCPHandshakeStage handshakeStatus = GMTCPHandshakeStage.Unknown;
        //Constructor
        public GMTCPClient(Socket sock, AbstractServer parent)
            : base(sock, parent)
        {
            PacketBuffer handshakePacket = new PacketBuffer();
            Logger.Trace("Connection Accepted. Beginning GM Handshake...");
            handshakePacket.WriteString("GM:Studio-Connect",true);
            base.SendPacket(handshakePacket);
            handshakeStatus = GMTCPHandshakeStage.AwaitingConnectAck;
        }
        //Send packet
        override public void SendPacket(PacketBuffer data)
        {
            byte[] newBuf = new byte[data.GetLength() + 12];
            Array.Copy(BitConverter.GetBytes((UInt32)0xdeadc0de), 0, newBuf, 0, 4); //Magic number
            Array.Copy(BitConverter.GetBytes((UInt32)12), 0, newBuf, 4, 4); //Header length
            Array.Copy(BitConverter.GetBytes((UInt32)data.GetLength()), 0, newBuf, 8, 4); //Payload length
            Array.Copy(data.GetData(), 0, newBuf, 12, data.GetLength()); //Actual payload
            try
            {
                GetSocket().Send(newBuf, 0, data.GetLength() + 12, 0);
            }
            catch (SocketException e)
            {
                Logger.Warn("Socket Exception: " + e.Message);
            }
        }
        //Data has been received
        override protected void OnDataReceived(byte[] data, int len)
        {
            if (handshakeStatus == GMTCPHandshakeStage.Complete)
            {
                int bytesReceived = 0;
                //Receive packets until all the data on the wire has been processed
                while (bytesReceived < len)
                {
                    UInt32 packetIdentifier = BitConverter.ToUInt32(data, bytesReceived);
                    if (packetIdentifier != 0xdeadc0de)
                    {
                        Logger.Trace("Packet dropped. Invalid identifier/magic number.");
                        break;
                    }
                    else
                    {
                        //Length of the packet data
                        UInt32 payloadLength = BitConverter.ToUInt32(data, bytesReceived+8);
                        PacketBuffer p = new PacketBuffer(data, bytesReceived+12, (int)payloadLength);
                        OnPacketReceived(p); //Process the packet
                        bytesReceived += (int)payloadLength + 12;
                    }
                }
            
            }
            else if (handshakeStatus == GMTCPHandshakeStage.AwaitingConnectAck)
            {
                uint magicNumber = BitConverter.ToUInt32(data, 0);
                if (magicNumber == 0xcafebabe)
                {   //If the magic number matches
                    Logger.Trace("Connect Acknowledged Successfully, Finalizing Handshake...");
                    PacketBuffer handshakePacket = new PacketBuffer();
                    handshakePacket.WriteUInt32(0xdeafbead);
                    handshakePacket.WriteUInt32(0xf00dbeeb);
                    handshakePacket.WriteUInt32(0x0000000c);
                    base.SendPacket(handshakePacket);
                    Logger.Trace("Handshake Finalized.");
                    handshakeStatus = GMTCPHandshakeStage.Complete;
                } 
            }

        }
    }
}
