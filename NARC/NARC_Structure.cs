using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARC
{
    public class NARC_Structure
    {
        public struct Nintendo_Archive
        {
            public struct Header
            {
                public uint magic;
                public uint constant;
                public uint fileSize;
                public uint headerSize;
                public uint nSections;
            }
            public struct FATB
            {
                public uint magic;
                public uint sectionSize;
                public uint nFiles;
                public List<uint> startoffsets;
                public List<uint> endoffsets;
                public List<uint> fntb_firstoffsets;
                public List<uint> fntb_firstfilepos;
                public List<uint> fntb_parentdir;
                public List<uint> fntb_sizeName;
                public List<string> fntb_name;
                public List<uint> fntb_dirnum;
            }
            public struct FNTB
            {
                public uint magic;
                public uint sectionSize;
                public uint directorystartoffset;
                public uint firstfileposroot;
                public uint nDir;
            }
            public struct FIMG
            {
                public uint magic;
                public uint sectionSize;
                public byte[] data;
            }
        }
    }
}
