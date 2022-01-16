using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BOF4Tool
{
    public class SerializationHelper
    {
        public static void Save<T>(T contents, string fileName) where T : class
        {
            SerializationHelper<T>.Save(contents, fileName);
        }
    }

    public class SerializationHelper<T> where T : class
    {
        private static XmlSerializer _serializer;

        static SerializationHelper()
        {
            try
            {
                _serializer = new XmlSerializer(typeof(T));
            }
            catch
            {

            }
        }

        public static T Read(string fileName)
        {
            using (var reader = XmlReader.Create(File.OpenText(fileName)))
            {
                return _serializer.Deserialize(reader) as T;
            }
        }
          
        public static void Save(T contents, string fileName)
        {
            var location = Path.GetDirectoryName(Path.GetFullPath(fileName));

            if (!Directory.Exists(location))
            {
                Directory.CreateDirectory(location);
            }


            using (var writer = new XmlTextWriter(new FileStream(fileName, FileMode.Create), Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                _serializer.Serialize(writer, contents);
            }
        }
    }
}