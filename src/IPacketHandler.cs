using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMNetSharp
{
    public interface IPacketHandler
    {
        void Handle(PacketBuffer p);
    }
}
