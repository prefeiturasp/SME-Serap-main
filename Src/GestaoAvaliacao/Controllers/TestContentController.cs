using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Linq;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
	public class TestContentController : Controller
	{
		private readonly IBookletBusiness bookletBusiness;
		private readonly ITestBusiness testBusiness;

		public TestContentController(IBookletBusiness bookletBusiness, ITestBusiness testBusiness)
		{
			this.bookletBusiness = bookletBusiness;
			this.testBusiness = testBusiness;
		}
		//
		// GET: /TestContent/
		public string Index(long id, Guid EntityId, EnumGenerateType generateType, EnumFileType fileType, bool preview = false)
		{
			PDFFilter filter = new PDFFilter
			{
				Booklet = new Entities.Booklet() { Id = id },
				FontSize = 20,
				UrlSite = Request.Url.Authority.ToString(),
				GenerateType = generateType,
				FileType = fileType,
				PDFPreview = preview,
				CDNMathJax = bool.Parse(ApplicationFacade.Parameters.First(p => p.Key.Equals("UTILIZACDNMATHJAX")).Value)
			};

			return this.bookletBusiness.GetBookletContent(filter, EntityId);
		}
	}
}