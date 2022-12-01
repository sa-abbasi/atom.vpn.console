using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atom.VPN.CC.Messaging
{
    public enum MessageType : int
    {
        Invalid = 1,
        Connect = 2,
        Disconnect = 3,
        GetCountryList = 4,
        GetProtocols = 5,
        GetCities = 6

    }
}
