using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace BOF4Tool.Format
{
    public class TextFile
    {
        [XmlAttribute]
        public string Name { get; set; }

        public List<TextEntry> Entries { get; } = new List<TextEntry>();

        public class TextEntry
        {
            [XmlAttribute]
            public ushort Offset { get; set; }

            [XmlIgnore]
            public byte[] Data { get; set; }

            [XmlAttribute]
            public string Parsed
            {
                get
                {
                    return TextParser.Parse(Data);
                }
                set { }
            }
        }
    }

    public class TextParser
    {
        static Dictionary<ushort, string> MAP = new Dictionary<ushort, string>();

        static TextParser()
        {
            var lines = File.ReadAllLines("CharMap.txt");
            foreach (var line in lines)
            {
                var tokens = line.Split('\t');

                if (!ushort.TryParse(tokens[0].Trim().Substring(2), NumberStyles.HexNumber, null, out var value))
                {
                }
                MAP[value] = tokens[1].Trim();
            }
        }

        public static string Parse(byte[] data)
        {
            var index = 0;
            var buffer = new StringBuilder();

            while (index < data.Length)
            {
                ushort value = data[index];

                if (value == 0)
                {
                    if (index == data.Length - 1)
                    {
                    }
                    else
                    {
                    }
                }

                switch (value)
                {
                    case 0x00:
                    {
                        buffer.Append("[next/]");
                        break;
                    }
                    case 0x01:
                    {
                        buffer.Append("[1/]");
                        break;
                    }
                    case 0x02:
                    {
                        buffer.Append("[continue/]");
                        break;
                    }
                    case 0x04:
                    {
                        var member = data[++index];
                        buffer.Append($"[name:{member}/]");
                        break;
                    }
                    case 0x05:
                    {
                        var color = data[++index];
                        buffer.Append($"[color:{color}]");

                        break;
                    }
                    case 0x06:
                    {
                        buffer.Append($"[/color]");
                        break;
                    }
                    case 0x0B:
                    {
                        buffer.Append("[p/]");
                        break;
                    }
                    case 0x0C:
                    {
                        index++;
                        break;
                    }
                    case 0x0D:
                    {
                        buffer.Append("[animation]");
                        break;
                    }
                    //case 0x0E:
                    //{
                    //    var f0 = data[++index];
                    //    var type = data[++index];
                    //    buffer.Append($"[/animation:{f0:X2}-{type}]");
                    //    break;
                    //}
                    case 0x17:
                    {
                        index++;
                        index++;
                        break;
                    }
                    case 0x12:
                    case 0x13:
                    {
                        value = (ushort)(value << 8 | data[++index]);
                        if (MAP.TryGetValue(value, out var text))
                        {
                            buffer.Append(text);
                        }
                        else
                        {
                            buffer.Append($"0x{value:X4}");
                        }
                        break;
                    }
                    case 0x14:
                    {
                        index++;
                        index++;
                        index++;
                        break;
                    }
                    case 0x15:
                    {
                        value = (ushort)(value << 8 | data[++index]);
                        if (MAP.TryGetValue(value, out var text))
                        {
                            buffer.Append($"[{text}]");
                        }
                        else
                        {
                        }
                        break;
                    }
                    case 0x18:
                    {
                        index++;
                        index++;
                        break;
                    }
                    default:
                    {
                        if (MAP.TryGetValue(value, out var text))
                        {
                            buffer.Append(text);
                        }
                        else
                        {
                            buffer.Append($"0x{value:X2}");
                        }
                        break;
                    }
                }

                index++;
            }

            return buffer.ToString();
        }
    }
}