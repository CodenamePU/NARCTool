using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARC
{
    public class NARC_FileExtensions
    {
        private Dictionary<uint, string> extensions = new Dictionary<uint, string>
        {
            // BMD0, Models
            {0x424D4430, ".bmd0"},
            {0x30444D42, ".bmd0"},

            // BTX0, Textures
            {0x42545830, ".btx0"},
            {0x30585442, ".btx0"},

            // NCSR, Nitro Character Screen
            {0x4E435352, ".ncsr"},
            {0x5253434E, ".ncsr"},

            // NCLR, Nitro Character Color Palette
            {0x4E434C52, ".nclr"},
            {0x524C434E, ".nclr"},

            // NCGR, Nitro Character Graphics
            {0x4E434752, ".ncgr"},
            {0x5247434E, ".ncgr"},

            // NANR, Nitro Character Animation
            {0x4E414E52, ".nanr"},
            {0x524E414E, ".nanr"},

            // NMAR, Nitro Character Multicell Animation
            {0x4E4D4152, ".nmar"},
            {0x52414D4E, ".nmar"},

            // NMCR, Nitro Character Multicells
            {0x4E4D4352, ".nmcr"},
            {0x52434D4E, ".nmcr"},

            // NCER, Nitro Character Cells
            {0x4E434552, ".ncer"},
            {0x5245434E, ".ncer"},

            // LZSS, LZ-Compressed files
            {0x11, ".lzss"}
        };

        public string GetFileExtension(uint num)
        {
            return extensions[num];
        }
    }
}
