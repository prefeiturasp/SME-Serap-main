using GestaoAvaliacao.Util;
using System.Collections.Generic;
using System.Drawing;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.IFileServer
{
    public interface IStorage
    {
        /// <summary>
        /// Salva o arquivo no formato byte array
        /// </summary>
        /// <param name="file">Byte array do arquivo</param>
        /// <param name="nameFile">Nome do arquivo</param>
        /// <param name="contentType">ContentType do arquivo</param>
        /// <param name="entityFile">Entity File do arquivo salvo </param>
		EntityFile Save(byte[] file, string nameFile, string contentType, string folder, string VirtualDirectory, string PhysicalDirectory, out EntityFile entityFile);

        EntityFile Delete(string nameFile);

        EntityFile Delete(string nameFile, string path);
        EntityFile Delete(string nameFile, string path, string physicalPath);
        EntityFile DeleteByPath(string path);

        EntityFile SaveZip(string zipFileName, string folder, IEnumerable<ZipFileInfo> files, string physicalDirectory);

        string SaveBitmap(Bitmap bmp, string virtualDirectory, string physicalDirectory, string fileName, string folder);

        void ClearFolder(string folderName);
        string GetDirectorySize(string path);
        string GetDirectorySize(string path, bool recursive);
    }
}
