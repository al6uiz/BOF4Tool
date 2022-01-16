using System.IO;

namespace BOF4Tool
{
    class FileUtility
    {
        public static void DeleteFileIfExists(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void PrepareFolderFile(string path)
        {
            var location = Path.GetDirectoryName(path);

            if (!Directory.Exists(location))
            {
                Directory.CreateDirectory(location);
            }
        }

        public static void PrepareFolder(string location)
        {
            if (!Directory.Exists(location))
            {
                Directory.CreateDirectory(location);
            }
        }
    }
}