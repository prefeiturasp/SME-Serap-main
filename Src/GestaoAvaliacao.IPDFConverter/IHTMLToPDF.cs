namespace GestaoAvaliacao.IPDFConverter
{
	public interface IHTMLToPDF
	{
		byte[] ConvertHtml(string html, string title, float LeftMargin, float RightMargin, float BottomMargin, float TopMargin, int TopSpacing, int BottomSpacing);
		byte[] ConvertHtml(string header, float heightHeader, string footer, float heightFooter, string html, string title, bool showBorder, float LeftMargin,
													float RightMargin, float BottomMargin, float TopMargin, int TopSpacing, int BottomSpacing);
		byte[] ConvertUrl(string header, float heightHeader, string footer, float heightFooter, string url, string title, bool showBorder, float LeftMargin,
													float RightMargin, float BottomMargin, float TopMargin, int TopSpacing, int BottomSpacing);

	}
}
