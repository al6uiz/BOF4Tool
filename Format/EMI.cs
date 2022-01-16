using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BOF4Tool.Format
{
    public class EMI
    {
        public class Entry
        {
            public uint Size { get; set; }
            public FileType Type { get; set; }
            public uint FirstBody { get; set; }
            public ushort Unknown { get; set; }
            public ushort Fill { get; set; }
            public int Index { get; internal set; }

        }

        public uint NumberOfFile { get; set; }

        public uint Unknown { get; set; }

        public byte[] MagicNumber { get; set; }

        public List<Entry> Files { get; } = new List<Entry>();

        public override string ToString()
        {
            return $"{NumberOfFile}";
        }

        public static EMI Read(string path)
        {
            var emi = new EMI();
            using (var fs = new FileStream(path, FileMode.Open))
            {
                emi.NumberOfFile = fs.ReadUInt32();
                emi.Unknown = fs.ReadUInt32();

                emi.MagicNumber = fs.ReadBytes(8);

                for (int i = 0; i < emi.NumberOfFile; i++)
                {
                    var entry = new Entry();
                    entry.Size = fs.ReadUInt32();
                    entry.Type = (FileType)fs.ReadUInt32();
                    entry.FirstBody = fs.ReadUInt32();
                    entry.Unknown = fs.ReadUInt16();
                    entry.Fill = fs.ReadUInt16();
                    entry.Index = i;

                    emi.Files.Add(entry);
                }
                fs.Position = GetNextSection(fs);
                var location = Path.GetDirectoryName(path);
                var extractLocation = Path.Combine(location, Path.GetFileNameWithoutExtension(path));
                FileUtility.PrepareFolder(extractLocation);

                foreach (var entry in emi.Files)
                {
                    var raw = fs.ReadBytes((int)entry.Size);
                    //if (entry.FirstBody != BitConverter.ToUInt32(raw, 0))
                    //{

                    //}

                    var next = GetNextSection(fs);
                    var remain = next - fs.Position;
                    var fill = fs.ReadBytes((int)remain);
                    if (!fill.All(x => x == 0x5F))
                    {

                    }
                    var filePath = Path.Combine(extractLocation, $"{entry.Index:000}.{(uint)entry.Type:X8}");
                    File.WriteAllBytes(filePath, raw);
                }
            }

            return emi;
        }

        private static int GetNextSection(FileStream fs)
        {
            return (int)(Math.Ceiling((double)fs.Position / 0x800) * 0x800);
        }
    }


    public enum FileType : uint
    {
        Text = 0x80010000,
    }
}
