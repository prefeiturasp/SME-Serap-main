using GestaoAvaliacao.Util;
using System;
using System.Configuration;
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
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["FileManagerUserName"]))
                    SaveFileWithCredentials(file, filePath, path);
                else
                    SaveFile(file, filePath, path);
            }
            catch (Exception ex)
            {
                throw new Exception($"Não foi possível salvar o arquivo na pasta de destino. {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        private void SaveFileWithCredentials(byte[] file, string filePath, string path)
        {
            using (new Impersonator(ConfigurationManager.AppSettings["FileManagerUserName"], 
                ConfigurationManager.AppSettings["FileManagerDomain"], 
                ConfigurationManager.AppSettings["FileManagerPassword"]))
            {
                SaveFile(file, filePath, path);
            }
        }

        private void SaveFile(byte[] file, string filePath, string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            File.WriteAllBytes(filePath, file);
        }
    }
}