using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mitac.Core.Utilities
{
    public class HandleFile
    {
        /// <summary>
        /// 解壓
        /// </summary>
        /// <param name="zipFilePath">解壓文件路徑</param>
        /// <returns></returns>
        public static List<FileInfo> UnZipFile(string zipFilePath)
        {
            ZipConstants.DefaultCodePage = Encoding.GetEncoding("utf-8").CodePage;

            var files = new List<FileInfo>();
            var zipFile = new FileInfo(zipFilePath);
            if (!File.Exists(zipFilePath))
            {
                return files;
            }
            using (var zipInputStream = new ZipInputStream(File.OpenRead(zipFilePath)))
            {
                ZipEntry theEntry;
                while ((theEntry = zipInputStream.GetNextEntry()) != null)
                {
                    if (zipFilePath != null)
                    {
                        string dir = Path.GetDirectoryName(zipFilePath);
                        if (dir != null)
                        {
                            string fileName = Path.GetFileName(theEntry.Name);
                            if (!string.IsNullOrEmpty(fileName))
                            {
                                string filePath = Path.Combine(dir, theEntry.Name.Replace(Path.GetExtension(theEntry.Name), "") + Path.GetExtension(theEntry.Name));
                                using (FileStream streamWriter = File.Create(filePath))
                                {
                                    var data = new byte[2048];
                                    while (true)
                                    {
                                        int size = zipInputStream.Read(data, 0, data.Length);
                                        if (size > 0)
                                        {
                                            streamWriter.Write(data, 0, size);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                files.Add(new FileInfo(filePath));
                            }
                        }
                    }
                }
            }
            return files;
        }
    }
}
