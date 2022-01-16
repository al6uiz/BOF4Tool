using System.Collections.Generic;
using System.IO;
using System.Linq;

using BOF4Tool.Format;

namespace BOF4Tool
{
    public class Program
    {
        private static string ROOT = @"D:\LocalProjects\BOF4\J\BIN";

        static void Main(string[] args)
        {
            //ExtractAllEMI();

            ExtractTextFile("WORLD");
        }

        private static void ExtractTextFile(string location)
        {
            var list = new List<TextFile>();

            foreach (var path in Directory.GetFiles(Path.Combine(ROOT, location), "*.80010000", SearchOption.AllDirectories))
            {
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    var file = new TextFile
                    {
                        Name = Path.GetFileName(Path.GetDirectoryName(path)) + ".EMI"
                    };

                    var first = fs.ReadUInt16();
                    file.Entries.Add(new TextFile.TextEntry
                    {
                        Offset = first
                    });

                    while (fs.Position < first)
                    {
                        var p = fs.ReadUInt16();


                        if (file.Entries.LastOrDefault()?.Offset == p)
                        {
                            break;
                        }
                        file.Entries.Add(new TextFile.TextEntry
                        {
                            Offset = p
                        });
                    }

                    // offset이 정렬되지 않은 경우가 있음
                    file.Entries.Sort((x, y) => x.Offset.CompareTo(y.Offset));

                    for (int i = 0; i < file.Entries.Count - 1; i++)
                    {
                        fs.Position = file.Entries[i].Offset;
                        var size = file.Entries[i + 1].Offset - file.Entries[i].Offset;
                        file.Entries[i].Data = fs.ReadBytes(size);
                    }
                    file.Entries.RemoveAt(file.Entries.Count - 1);

                    list.Add(file);
                }

                SerializationHelper.Save(list, "world.xml");
            }
        }

        private static void ExtractAllEMI()
        {
            foreach (var path in Directory.GetFiles(ROOT, "*.EMI", SearchOption.AllDirectories))
            {
                EMI.Read(path);
            }
        }
    }
}