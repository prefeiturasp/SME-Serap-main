using GestaoAvaliacao.Entities;
using System.Text;

namespace GestaoAvaliacao.IBusiness
{
	public interface IGenerateHtmlBusiness
	{
		StringBuilder BuildAnswerSheet(PDFFilter filter);
		string GetHtmlFromItems(PDFFilter filter);
		string GetContentHtml(PDFFilter filter);
		string GetHeader(PDFFilter modelTest);
		string GetFooter(PDFFilter modelTest);
	}
}