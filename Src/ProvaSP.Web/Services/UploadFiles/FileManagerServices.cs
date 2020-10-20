using System;
using System.IO;

namespace ProvaSP.Web.Services.UploadFiles
{
    internal class FileManagerServices : IFileManagerServices
    {
        public void Save(byte[] file, string nameFile, string path)
        {
            var filePath = Path.Combine(path, nameFile);

            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                File.WriteAllBytes(filePath, file);
            }
            catch (Exception ex)
            {
                throw new Exception($"Não foi possível salvar o arquivo na pasta de destino. {ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }
}