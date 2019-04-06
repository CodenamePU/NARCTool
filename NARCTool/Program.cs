using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARCTool
{
    class Program
    {
        static void Main(string[] args)
        {
            NARC.NARC_Access narc = new NARC.NARC_Access();
            switch (args[0].ToLower())
            {
                case "extract":
                    narc.Unpack(args[1], args[2]);
                    break;

                case "pack":
                    narc.Pack(args[1], args[2]);
                    break;

                default:
                    break;
            }
        }
    }
}
