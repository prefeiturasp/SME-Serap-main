using ProvaSP.Model.Entidades.UploadFiles;

namespace ProvaSP.Web.Services.UploadFiles.RevistasEBoletins
{
    public class UploadRevistasEBoletinsServices : UploadFileServices, IUploadRevistasEBoletinsServices
    {
        protected override UploadFileBatchType Type => UploadFileBatchType.RevistasEBoletins;
    }
}