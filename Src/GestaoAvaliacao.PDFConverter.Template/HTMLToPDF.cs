using GestaoAvaliacao.IPDFConverter;
using System;

namespace GestaoAvaliacao.PDFConverter.Template
{
	class HTMLToPDF : IHTMLToPDF
	{
		public byte[] ConvertHtml(string html, string title, float LeftMargin, float RightMargin, float BottomMargin, float TopMargin, int TopSpacing, int BottomSpacing)
		{
			throw new NotImplementedException();
		}

		public byte[] ConvertHtml(string header, float heightHeader, string footer, float heightFooter, string html, string title, bool showBorder, float LeftMargin, float RightMargin, float BottomMargin, float TopMargin, int TopSpacing, int BottomSpacing)
		{
			throw new NotImplementedException();
		}

		public byte[] ConvertUrl(string header, float heightHeader, string footer, float heightFooter, string url, string title, bool showBorder, float LeftMargin, float RightMargin, float BottomMargin, float TopMargin, int TopSpacing, int BottomSpacing)
		{
			throw new NotImplementedException();
		}
	}
}
