using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.Util.QRCode;
using System.Drawing;

namespace GestaoAvaliacao.Business
{
    public class AnswerSheetBusiness : IAnswerSheetBusiness
    {
        private readonly IStorage storage;

        public AnswerSheetBusiness(IStorage storage)
        {
            this.storage = storage;
        }

        #region Read

        #endregion

        #region Write

        public string RenderQrCode(string text, int pixelsPerModule, string virtualDirectory, string physicalDirectory, string fileName, string folderName)
        {
            Bitmap bmp = QRCode.RenderQRCode(text, pixelsPerModule);

            return storage.SaveBitmap(bmp, virtualDirectory, physicalDirectory, fileName, folderName);
        }

		#endregion

	}
}
