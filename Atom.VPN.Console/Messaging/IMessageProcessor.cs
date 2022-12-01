using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atom.VPN.Console
{
    public interface IMessageProcessor
    {

        BaseResponse Process(BaseRequest Request);
    }







}
