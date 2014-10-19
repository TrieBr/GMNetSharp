using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
namespace GMNetSharp
{
    public abstract class AbstractClientFactory
    {
        //Create a client
        abstract public AbstractClient CreateClient(Socket sock, AbstractServer server);

    }
}
