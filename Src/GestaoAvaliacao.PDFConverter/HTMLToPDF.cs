using EvoPdf;
using GestaoAvaliacao.IPDFConverter;
using System;
using System.Drawing;

namespace GestaoAvaliacao.PDFConverter
{
	public class HTMLToPDF : IHTMLToPDF
	{
		private HtmlToPdfConverter pdfConverter { get; set; }

		public byte[] ConvertHtml(string html, string title, float LeftMargin, float RightMargin, float BottomMargin, float TopMargin, int TopSpacing, int BottomSpacing)
		{
			pdfConverter = InitialPdfConverter(title, LeftMargin, RightMargin, BottomMargin, TopMargin, TopSpacing, BottomSpacing);

			pdfConverter.ConversionDelay = 0;
			var pdfDocument = pdfConverter.ConvertHtmlToPdfDocumentObject(html, null);

			return SavePDF(pdfDocument);
		}
		public byte[] ConvertHtml(string header, float heightHeader, string footer, float heightFooter, string html, string title, bool showBorder, float LeftMargin,
											float RightMargin, float BottomMargin, float TopMargin, int TopSpacing, int BottomSpacing)
		{
			pdfConverter = InitialPdfConverter(title, LeftMargin, RightMargin, BottomMargin, TopMargin, TopSpacing, BottomSpacing);
			pdfConverter = AddHeaderAndFooter(pdfConverter, header, heightHeader, footer, heightFooter);

			#region CONTEUDO

			Document pdfDocument = null;

			pdfDocument = pdfConverter.ConvertHtmlToPdfDocumentObject(html, null);

			#endregion

			#region BORDER

			if (showBorder)
			{
				if (pdfDocument != null)
				{
					pdfDocument = AddBorder(pdfDocument);
				}
			}

			#endregion

			return SavePDF(pdfDocument);
		}
		public byte[] ConvertUrl(string header, float heightHeader, string footer, float heightFooter, string url, string title, bool showBorder, float LeftMargin,
											float RightMargin, float BottomMargin, float TopMargin, int TopSpacing, int BottomSpacing)
		{
			pdfConverter = InitialPdfConverter(title, LeftMargin, RightMargin, BottomMargin, TopMargin, TopSpacing, BottomSpacing);
			pdfConverter = AddHeaderAndFooter(pdfConverter, header, heightHeader, footer, heightFooter);

			#region CONTEUDO

			Document pdfDocument = null;
			pdfConverter.NavigationTimeout = 600;
			pdfConverter.ConversionDelay = 2;
			pdfDocument = pdfConverter.ConvertUrlToPdfDocumentObject(url);


			#endregion

			#region BORDER

			if (showBorder)
			{
				if (pdfDocument != null)
				{
					pdfDocument = AddBorder(pdfDocument);
				}
			}

			#endregion

			return SavePDF(pdfDocument);
		}
		#region Private Methods

		private HtmlToPdfConverter InitialPdfConverter(string title, float LeftMargin, float RightMargin, float BottomMargin, float TopMargin, int TopSpacing, int BottomSpacing)
		{
			pdfConverter = new HtmlToPdfConverter();
			pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
			pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;
			pdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
			pdfConverter.PdfDocumentOptions.AvoidImageBreak = true;
			pdfConverter.PdfDocumentOptions.AvoidTextBreak = true;
			pdfConverter.PdfDocumentOptions.LiveUrlsEnabled = true;
			pdfConverter.PrerenderEnabled = true;
			pdfConverter.JavaScriptEnabled = true;
			pdfConverter.NavigationTimeout = 240;

			pdfConverter.PdfDocumentInfo.Title = title;
			pdfConverter.PdfDocumentInfo.CreatedDate = DateTime.Now;
			pdfConverter.LicenseKey = Config.EVOLicenseKey; ;

			#region MARGINS

			pdfConverter.PdfDocumentOptions.LeftMargin = LeftMargin;
			pdfConverter.PdfDocumentOptions.RightMargin = RightMargin;
			pdfConverter.PdfDocumentOptions.TopMargin = TopMargin;
			pdfConverter.PdfDocumentOptions.TopSpacing = TopSpacing;
			pdfConverter.PdfDocumentOptions.BottomSpacing = BottomSpacing;
			pdfConverter.PdfDocumentOptions.BottomMargin = BottomMargin;

			#endregion

			return pdfConverter;
		}
		private HtmlToPdfConverter AddHeaderAndFooter(HtmlToPdfConverter pdfConverter, string header, float heightHeader, string footer, float heightFooter)
		{
			#region CABEÇALHO

			if (!string.IsNullOrEmpty(header))
			{
				pdfConverter.PdfHeaderOptions.HeaderHeight = heightHeader;
				HtmlToPdfElement headerHtml = new HtmlToPdfElement(header, null);

				pdfConverter.PdfDocumentOptions.ShowHeader = true;
				pdfConverter.PdfHeaderOptions.AddElement(headerHtml);
			}


			#endregion

			#region RODAPÉ
			if (!string.IsNullOrEmpty(footer))
			{
				pdfConverter.PdfFooterOptions.FooterHeight = heightFooter;
				HtmlToPdfElement footerHtml = new HtmlToPdfElement(footer, null);

				pdfConverter.PdfDocumentOptions.ShowFooter = true;
				pdfConverter.PdfFooterOptions.AddElement(footerHtml);
			}

			Font footerFont = new Font(new FontFamily("Arial"), 10, GraphicsUnit.Point);
			TextElement footerTextElement = new TextElement(0, pdfConverter.PdfFooterOptions.FooterHeight - 20, "&p; de &P;  ", footerFont);
			footerTextElement.TextAlign = HorizontalTextAlign.Right;
			pdfConverter.PdfFooterOptions.AddElement(footerTextElement);

			#endregion

			return pdfConverter;
		}
		private Document AddBorder(Document pdfDocument)
		{
			foreach (PdfPage page in pdfDocument.Pages)
			{
				float width = (page.PageSize.Width - pdfConverter.PdfDocumentOptions.RightMargin - pdfConverter.PdfDocumentOptions.LeftMargin);
				float height = (page.PageSize.Height - pdfConverter.PdfDocumentOptions.TopMargin - pdfConverter.PdfDocumentOptions.BottomMargin);
				RectangleElement borderRectangleElement = new RectangleElement(0, -(page.Header.Height), width, height);
				page.AddElement(borderRectangleElement);
			}
			return pdfDocument;
		}
		private byte[] SavePDF(Document pdfDocument)
		{
			if (pdfDocument != null)
			{
				byte[] result = pdfDocument.Save();
				pdfDocument.Close();
				return result;
			}
			else
				return null;

		}

		#endregion
	}
}
