using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Atom.VPN.Console
{
    public interface IMessageChannel
    {
        Task Send(BaseResponse message);
    }





}
