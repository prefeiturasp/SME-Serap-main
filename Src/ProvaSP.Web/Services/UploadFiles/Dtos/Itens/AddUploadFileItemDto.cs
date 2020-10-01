namespace ProvaSP.Web.Services.UploadFiles.Dtos.Itens
{
    public class AddUploadFileItemDto
    {
        public long BatchId { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
    }
}