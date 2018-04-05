using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.FileServer
{
    public class LocalStorage : IStorage
	{
		/// <summary>
		/// Salva o arquivo no formato byte array.
		/// </summary>
		/// <param name="file">Byte array do arquivo.</param>
		/// <param name="nameFile">Nome do arquivo.</param>
		/// <param name="contentType">ContentType do arquivo</param>
		/// <param name="entityFile">Entity File do arquivo salvo</param>
		public EntityFile Save(byte[] file, string nameFile, string contentType, string folder, string VirtualDirectory, string PhysicalDirectory, out EntityFile entityFile)
		{
			entityFile = new EntityFile();

			var path = PhysicalDirectory;

			if (!string.IsNullOrEmpty(folder))
			{
				path = Path.Combine(path, folder);
			}	

            string filePath = Path.Combine(path, nameFile);

			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["UserName"]))
			{
				using (new Impersonator(ConfigurationManager.AppSettings["UserName"], ConfigurationManager.AppSettings["UserDomain"], ConfigurationManager.AppSettings["UserPassword"]))
				{
					try
					{
						if (!Directory.Exists(path))
							Directory.CreateDirectory(path);

                        System.IO.File.WriteAllBytes(filePath, file);

						entityFile.Name = nameFile;
						entityFile.ContentType = contentType;
                        entityFile.Path = Path.Combine(VirtualDirectory, folder, nameFile).Replace('\\', '/');
					}
					catch (Exception e)
					{
						if (Directory.Exists(path))
							Directory.Delete(path, true);

						entityFile.Validate.IsValid = false;
						entityFile.Validate.Type = ValidateType.error.ToString();
						entityFile.Validate.Code = 500;
						entityFile.Validate.Message = "Erro ao salvar arquivo.";

						throw new Exception(string.Format("{0} - {1}", WindowsIdentity.GetCurrent().Name, e.Message), e);
					}
				}
			}
			else
			{
				try
				{
					if (!Directory.Exists(path))
						Directory.CreateDirectory(path);

                    System.IO.File.WriteAllBytes(filePath, file);

					entityFile.Name = nameFile;
					entityFile.ContentType = contentType;
					entityFile.Path = Path.Combine(VirtualDirectory, folder, nameFile).Replace('\\', '/');
				}
				catch (Exception e)
				{
					if (Directory.Exists(path))
						Directory.Delete(path, true);

					entityFile.Validate.IsValid = false;
					entityFile.Validate.Type = ValidateType.error.ToString();
					entityFile.Validate.Code = 500;
					entityFile.Validate.Message = "Erro ao salvar arquivo.";

					throw new Exception(string.Format("{0} - {1}", WindowsIdentity.GetCurrent().Name, e.Message), e);
				}
			}


			return entityFile;
		}

        public EntityFile Delete(string nameFile)
        {
            return Delete(nameFile, null);
        }

        public EntityFile Delete(string nameFile, string path)
        {
            return Delete(nameFile, path, null);
        }

        public EntityFile Delete(string nameFile, string path, string physicalPath)
		{
			EntityFile entityFile = new EntityFile();

			path = !string.IsNullOrEmpty(physicalPath) ? physicalPath : string.Concat(Constants.StorageFilePath, path);

			try
			{
				if (Directory.Exists(path) && !string.IsNullOrEmpty(nameFile))
				{
					if (System.IO.File.Exists(Path.Combine(path, nameFile)))
					{
						System.IO.File.Delete(Path.Combine(path, nameFile));
					}
				}
				else
				{
					if(System.IO.File.Exists(path))
					{
						System.IO.File.Delete(path);
					}
				}
			}
			catch
			{
				entityFile.Validate.IsValid = false;
				entityFile.Validate.Type = ValidateType.error.ToString();
				entityFile.Validate.Code = 500;
				entityFile.Validate.Message = "Erro ao deletar arquivo.";
			}

			return entityFile;
		}

        public EntityFile DeleteByPath(string path)
        {
            EntityFile entityFile = new EntityFile();
            
            try
            {

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                
            }
            catch
            {
                entityFile.Validate.IsValid = false;
                entityFile.Validate.Type = ValidateType.error.ToString();
                entityFile.Validate.Code = 500;
                entityFile.Validate.Message = "Erro ao deletar arquivo.";
            }
            return entityFile;
        }

        public EntityFile SaveZip(string zipFileName, string folder, IEnumerable<ZipFileInfo> files, string physicalDirectory)
		{
			EntityFile entityFile = new EntityFile();

			try
			{
				string path = physicalDirectory;
				path = Path.Combine(path, folder);

				entityFile.Path = path;

				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);

				string completePath = Path.Combine(path, zipFileName);
				if (!string.IsNullOrEmpty(completePath) && System.IO.File.Exists(completePath))
					System.IO.File.Delete(completePath);

				ZipFileCreator.CreateZipFile(completePath, files);
			}
			catch
			{
				if (Directory.Exists(entityFile.Path))
					Directory.Delete(entityFile.Path, true);

				entityFile.Validate.IsValid = false;
				entityFile.Validate.Type = ValidateType.error.ToString();
				entityFile.Validate.Code = 500;
				entityFile.Validate.Message = "Erro ao salvar arquivo.";
			}

			return entityFile;
		}

		public string SaveBitmap(Bitmap bmp, string virtualDirectory, string physicalDirectory, string fileName, string folder)
		{
			var path = physicalDirectory;

			if (!string.IsNullOrEmpty(folder))
			{
				path = Path.Combine(path, folder);
			}

            string filePath = Path.Combine(path, fileName);

            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                byte[] data = ImageToByte2(bmp);
                System.IO.File.WriteAllBytes(filePath, data);

				return string.Concat(virtualDirectory, Path.Combine(filePath.Replace(physicalDirectory, string.Empty)).Replace('\\', '/'));
            }
            catch
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, true);

                return null;
            }
		}

		private byte[] ImageToByte2(Image img)
		{
			byte[] byteArray = new byte[0];
			using (MemoryStream stream = new MemoryStream())
			{
				img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
				stream.Close();

				byteArray = stream.ToArray();
			}
			return byteArray;
		}

        public void ClearFolder(string folder)
        {
            var path = Constants.StorageFilePath;

            if (!string.IsNullOrEmpty(folder))
            {
                path = Path.Combine(path, folder);
            }

            if (Directory.Exists(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);

                foreach (FileInfo fi in dir.GetFiles())
                {
                    fi.IsReadOnly = false;
                    fi.Delete();
                }

                foreach (DirectoryInfo di in dir.GetDirectories())
                {
                    ClearFolder(di.FullName);
                    di.Delete();
                }
            }
        }

        public string GetDirectorySize(string path)
        {
            return GetDirectorySize(path, false);
        }

        public string GetDirectorySize(string path, bool recursive)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                if (dir != null)
                {
                    long sizeInBytes = dir.EnumerateFiles("*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Sum(x => x.Length);
                    return FormatBytes(sizeInBytes);
                }
            }

            return " - ";
        }

        private string FormatBytes(long bytes)
        {
            if (bytes >= 0x1000000000000000) { return ((double)(bytes >> 50) / 1024).ToString("0.### EB"); }
            if (bytes >= 0x4000000000000) { return ((double)(bytes >> 40) / 1024).ToString("0.### PB"); }
            if (bytes >= 0x10000000000) { return ((double)(bytes >> 30) / 1024).ToString("0.### TB"); }
            if (bytes >= 0x40000000) { return ((double)(bytes >> 20) / 1024).ToString("0.### GB"); }
            if (bytes >= 0x100000) { return ((double)(bytes >> 10) / 1024).ToString("0.### MB"); }
            if (bytes >= 0x400) { return ((double)(bytes) / 1024).ToString("0.###") + " KB"; }
            return bytes.ToString("0 Bytes");
        }
    }
}
