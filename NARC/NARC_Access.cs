using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NARC
{
    public class NARC_Access
    {
        public void Unpack(string NARC, string outputdir)
        {
            BinaryReader narc = new BinaryReader(File.Open(NARC, FileMode.Open));
            NARC_FileExtensions extension = new NARC_FileExtensions();
            NARC_Structure.Nintendo_Archive.Header narc_header = new NARC_Structure.Nintendo_Archive.Header()
            {
                magic = narc.ReadUInt32(),
                constant = narc.ReadUInt32(),
                fileSize = narc.ReadUInt32(),
                headerSize = narc.ReadUInt16(),
                nSections = narc.ReadUInt16(),
            };

            NARC_Structure.Nintendo_Archive.FATB narc_fatb = new NARC_Structure.Nintendo_Archive.FATB()
            {
                magic = narc.ReadUInt32(),
                sectionSize = narc.ReadUInt32(),
                nFiles = narc.ReadUInt32(),
                startoffsets = new List<uint>(),
                endoffsets = new List<uint>(),
                fntb_name = new List<string>(),
            };

            for (int i = 0; i < narc_fatb.nFiles; i++)
            {
                narc_fatb.startoffsets.Add(narc.ReadUInt32());
                narc_fatb.endoffsets.Add(narc.ReadUInt32());
            }

            NARC_Structure.Nintendo_Archive.FNTB narc_fntb = new NARC_Structure.Nintendo_Archive.FNTB
            {
                magic = narc.ReadUInt32(),
                sectionSize = narc.ReadUInt32(),
                directorystartoffset = narc.ReadUInt32(),
                firstfileposroot = narc.ReadUInt16(),
                nDir = narc.ReadUInt16()
            };

            if (narc_fntb.nDir != 1)
            {
                /*
                for (int i = 0; i < narc_fntb.nDir; i++)
                {
                    narc_fatb.fntb_firstoffsets.Add(narc.ReadUInt32());
                    narc_fatb.fntb_firstfilepos.Add(narc.ReadUInt16());
                    narc_fatb.fntb_parentdir.Add(narc.ReadUInt16());
                    narc_fatb.fntb_sizeName.Add(narc.ReadUInt16());
                    if (narc_fatb.fntb_sizeName[i] == 0)
                        narc_fatb.fntb_name.Add((uint)i);
                    else
                        narc_fatb.fntb_name.Add();
                }
                */
            }
            else
                for (int i = 0; i < narc_fatb.nFiles; i++)
                    narc_fatb.fntb_name.Add(i.ToString());

            var fimg_offset = narc.BaseStream.Position + 0x8;
            NARC_Structure.Nintendo_Archive.FIMG narc_fimg = new NARC_Structure.Nintendo_Archive.FIMG
            {
                magic = narc.ReadUInt32(),
                sectionSize = narc.ReadUInt32()
            };

            string file_ext;
            for (int i = 0; i < narc_fatb.nFiles; i++)
            {
                narc.BaseStream.Position = fimg_offset + narc_fatb.startoffsets[i];
                if (narc.ReadSByte() == 0x11)
                {
                    narc.BaseStream.Position -= 1;
                    file_ext = extension.GetFileExtension(0x11);
                }
                else
                {
                    narc.BaseStream.Position -= 1;
                    try
                    {
                        file_ext = extension.GetFileExtension(narc.ReadUInt32());
                        narc.BaseStream.Position -= 4;
                    }
                    catch (KeyNotFoundException e)
                    {
                        narc.BaseStream.Position -= 4;
                        file_ext = ".bin";
                    }
                }

                Directory.CreateDirectory(outputdir);
                BinaryWriter writer = new BinaryWriter(File.Open(Path.Combine(outputdir, narc_fatb.fntb_name[i] + file_ext), FileMode.Create));
                writer.Write(narc.ReadBytes((int)(narc_fatb.endoffsets[i] - narc_fatb.startoffsets[i])));
            }
            return;
        }

        public void Pack(string dir, string output_narc)
        {
            List<int> file_offsets = new List<int>();
            List<int> file_sizes = new List<int>();
            string[] sorted_files = new string[Directory.GetFiles(dir).Length];
            BinaryWriter writer_fimg = new BinaryWriter(File.Open(dir + "_fimg.bin", FileMode.Create));
            var fimg_end = 0;
            List<byte> fimg_data = new List<byte>();

            foreach (string file in Directory.GetFiles(dir))
                sorted_files[int.Parse(Path.GetFileNameWithoutExtension(file))] = file;
            foreach (string file in sorted_files)
            {
                using (BinaryReader b = new BinaryReader(File.Open(file, FileMode.Open)))
                {
                    file_offsets.Add(fimg_data.Count());
                    fimg_data.AddRange(b.ReadBytes((int)b.BaseStream.Length));
                }
            }
            writer_fimg.Write(0x46494D47);
            writer_fimg.Write(fimg_data.Count() + 8);
            writer_fimg.Write(fimg_data.ToArray<byte>());
            writer_fimg.Close();

            BinaryWriter writer_fntb = new BinaryWriter(File.Open(dir + "_fntb.bin", FileMode.Create));
            writer_fntb.Write((uint)0x464E5442);
            writer_fntb.Write((uint)0x10);
            writer_fntb.Write((uint)0x4);
            writer_fntb.Write((ushort) 0x0);
            writer_fntb.Write((ushort) 0x1);
            writer_fntb.Close();

            BinaryWriter writer_fatb = new BinaryWriter(File.Open(dir + "_fatb.bin", FileMode.Create));
            writer_fatb.Write((uint)0x46415442);
            writer_fatb.Write((uint)(0xC + 0x8 * file_offsets.Count));
            writer_fatb.Write((uint)file_offsets.Count);
            for (int i = 0; i < file_offsets.Count; i++)
            {
                try
                {
                    writer_fatb.Write((uint)file_offsets[i]);
                    writer_fatb.Write((uint)file_offsets[i+1]);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    writer_fatb.Write((uint)fimg_data.Count());
                }
            }
            writer_fatb.Close();

            BinaryWriter writer_narc = new BinaryWriter(File.Open(output_narc, FileMode.Create));
            writer_narc.Write(0x4352414E);
            writer_narc.Write(0x0100FFFE);
            BinaryReader reader_fatb = new BinaryReader(File.Open(dir + "_fatb.bin", FileMode.Open));
            BinaryReader reader_fntb = new BinaryReader(File.Open(dir + "_fntb.bin", FileMode.Open));
            BinaryReader reader_fimg = new BinaryReader(File.Open(dir + "_fimg.bin", FileMode.Open));
            writer_narc.Write((uint)(0x10 + (int)(reader_fatb.BaseStream.Length + reader_fntb.BaseStream.Length + reader_fimg.BaseStream.Length)));
            writer_narc.Write((ushort)0x10);
            writer_narc.Write((ushort)0x3);
            writer_narc.Write(reader_fatb.ReadBytes((int)reader_fatb.BaseStream.Length));
            writer_narc.Write(reader_fntb.ReadBytes((int)reader_fntb.BaseStream.Length));
            writer_narc.Write(reader_fimg.ReadBytes((int)reader_fimg.BaseStream.Length));
            reader_fatb.Close();
            reader_fntb.Close();
            reader_fimg.Close();
            writer_narc.Close();
        }
    }
}
