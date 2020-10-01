namespace ProvaSP.Web.Services.UploadFiles.Dtos.Itens
{
    public class UploadFileItemDto
    {
        public long Id { get; private set; }
        public string OriginPath { get; set; }
        public string FileName { get; set; }
        public string Situation { get; private set; }
        public string ResultMessage { get; private set; }
    }
}