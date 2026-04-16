using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Wes.Utils.Extension
{
    public static class FileExtension
    {
        public static void WriteToFile(this string path, string fileName, string fileContent)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText($"{path}/{fileName}", fileContent);
        }
    }
}
