namespace ProvaSP.Web.Services.UploadFiles.Dtos.Search
{
    public class UploadFileSearchDto
    {
        public int Page { get; set; }
        public string Edicao { get; set; }
        public short? AreaDeConhecimento { get; set; }
        public short? CicloDeAprendizagem { get; set; }
    }
}