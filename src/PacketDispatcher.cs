using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMNetSharp
{
    public class PacketDispatcher
    {
        //Message handler instances
        private Dictionary<int, IPacketHandler> handlers = new Dictionary<int, IPacketHandler>();
        //Register a new Message Handler
        public void RegisterMessageHandler(IPacketHandler handler, int messageID)
        {
            if (handlers.ContainsKey(messageID))
            {
                Logger.Warn("Message Handler for ID (" + messageID + ") already exists. Overwriting!");
                handlers.Remove(messageID);
            }
            handlers.Add(messageID, handler);
        }
        //Handle Packet (Dispatches to appropriate handler)
        public void HandlePacket(PacketBuffer p)
        {
            int messageID = (int)p.ReadUInt8(); //Read the message ID;
            IPacketHandler handler;
            if (handlers.TryGetValue(messageID,out handler))
            {
                handler.Handle(p);
            }
            else
            {
                Logger.Warn("Message Dispatcher: No Handler for ID (" + messageID + ")");
            }
        }
    }
}
