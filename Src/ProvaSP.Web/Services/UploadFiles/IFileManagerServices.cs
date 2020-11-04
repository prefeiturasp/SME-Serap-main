namespace ProvaSP.Web.Services.UploadFiles
{
    internal interface IFileManagerServices
    {
        void Save(byte[] file, string nameFile, string path);
    }
}