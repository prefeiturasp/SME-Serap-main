using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web;

namespace GestaoAvaliacao.Util
{
    public class ZipFileInfo
    {
        public Validate Validate { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public byte[] Data { get; set; }
        public int Length { get; set; }
        public string[] ExtensionsAllowed { get; set; }
    }

    public static class ZipFileCreator
    {
        public static bool CreateZipFile(string fileName, IEnumerable<ZipFileInfo> files)
        {
            bool ret = false;

            ZipArchive zip = null;
            try
            {
                zip = ZipFile.Open(fileName, ZipArchiveMode.Create);

                foreach (ZipFileInfo file in files)
                {
                    if (System.IO.File.Exists(file.Path))
                    {
                        zip.CreateEntryFromFile(file.Path, file.Name, CompressionLevel.Optimal);
                        ret = true;
                    }
                }
            }
            catch
            {
                ret = false;
            }
            finally
            {
                if (zip != null)
                {
                    zip.Dispose();
                }
            }

            return ret;
        }

        public static bool CreateZipFileFromDirectory(string fileName, string directoryName)
        {
            bool ret = false;

            if (System.IO.Directory.Exists(directoryName))
            {
                ZipFile.CreateFromDirectory(directoryName, fileName, CompressionLevel.Optimal, false);
                ret = true;
            }

            return ret;
        }

        public static List<ZipFileInfo> ExtractZipFile(ZipFileInfo file, out Validate validate)
        {
            validate = new Validate();
            List<ZipFileInfo> ret = null;
            ZipArchive zip = null;

            try
            {
                if (System.IO.File.Exists(file.Name))
                {
                    int codePage = 850;
                    zip = ZipFile.Open(file.Name, ZipArchiveMode.Read, Encoding.GetEncoding(codePage));
                    ret = new List<ZipFileInfo>();
                    bool fileAllowed = true;

                    foreach (ZipArchiveEntry entry in zip.Entries)
                    {
                        if (entry.Length > 0)
                        {
                            using (var stream = entry.Open())
                            {
                                using (var ms = new MemoryStream())
                                {
                                    if (file.ExtensionsAllowed != null)
                                    {
                                        string contentType = MimeMapping.GetMimeMapping(entry.Name);
                                        fileAllowed = StringHelper.ValidateValuesAllowed(file.ExtensionsAllowed, contentType);
                                    }

                                    string extension = Path.GetExtension(entry.Name);
                                    string fileName = StringHelper.NormalizeFileName(entry.Name.Replace(extension, string.Empty));

                                    if (fileAllowed && !string.IsNullOrEmpty(fileName))
                                    {
                                        stream.CopyTo(ms);

                                        ret.Add(new ZipFileInfo
                                        {
                                            Data = ms.ToArray(),
                                            Name = fileName + extension,
                                            Length = Convert.ToInt32(entry.Length)
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    validate.IsValid = false;
                    validate.Message = "Arquivo compactado não encontrado.";
                }
            }
            catch (InvalidDataException)
            {
                validate.IsValid = false;
                validate.Message = "Arquivo compactado corrompido.";
            }
            catch (Exception)
            {
                ret = null;
                throw;
            }
            finally
            {
                if (zip != null)
                {
                    zip.Dispose();
                }
            }

            return ret;
        }
    }
}



