using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atom.VPN.Console
{
    public static class Util
    {
        public static string LeftOf(this string str, int Count)
        {
            if (str.Length <= Count) return str;

            return str.Substring(0, Count);

        }
    }
}
