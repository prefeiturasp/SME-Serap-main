namespace GestaoAvaliacao.IBusiness
{
    public interface IAnswerSheetBusiness
    {
        string RenderQrCode(string text, int pixelsPerModule, string virtualDirectory, string physicalDirectory, string fileName, string folderName);
	}
}
