using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Controllers
{
	[Authorize]
	[AuthorizeModule]
	public class BookletController : Controller
	{
		private readonly IBookletBusiness bookletBusiness;
		private readonly IBlockBusiness blockBusiness;
		private readonly IFileBusiness fileBusiness;
		private readonly ITestBusiness testBusiness;

		public BookletController(IBookletBusiness bookletBusiness, IBlockBusiness blockBusiness, IFileBusiness fileBusiness, ITestBusiness testBusiness)
		{
			this.bookletBusiness = bookletBusiness;
			this.blockBusiness = blockBusiness;
			this.fileBusiness = fileBusiness;
			this.testBusiness = testBusiness;
		}

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult IndexList()
		{
			return View("partials/_indexList");
		}

		public ActionResult IndexForm()
		{
			return View("partials/_indexForm");
		}

		#region Read

		[HttpGet]
		public JsonResult GetAllByTest(long Id)
		{
			try
			{
				IEnumerable<Booklet> bookletList = bookletBusiness.GetAllByTest(Id);

				if (bookletList != null && bookletList.Count() > 0)
				{
					IEnumerable<EntityFile> filesTest = fileBusiness.GetFilesByParent(Id, EnumFileType.Test);
					IEnumerable<EntityFile> filesAnswerSheet = fileBusiness.GetFilesByParent(Id, EnumFileType.AnswerSheetStudentNumber);
					IEnumerable<EntityFile> filesFeedback = fileBusiness.GetFilesByParent(Id, EnumFileType.TestFeedback);

					var ret = bookletList.Select(p => new
					{
						Id = p.Id,
						Description = p.Order,
						Blocks = p.Blocks.Select(a => new
						{
							Id = a.Id,
							Description = a.Description
						}),
						Registered = p.Test.TestSituation == EnumTestSituation.Registered,
						TestId = p.Test != null ? p.Test.Id : p.Test_Id,
						File = filesTest != null ? filesTest.Where(f => f.OwnerId == p.Id && f.OwnerType == (byte)EnumFileType.Test).Select(f => new
						{
							Id = f.Id,
							Name = !string.IsNullOrEmpty(f.OriginalName) ? f.OriginalName : f.Name,
							Path = f.Path,
							AllowLink = !f.ContentType.Equals(MimeType.CSV.GetDescription()),
							GenerationData = (!f.UpdateDate.Equals(f.CreateDate) ? f.UpdateDate.ToString("dd/MM/yyyy") : f.CreateDate.ToString("dd/MM/yyyy"))
						}) : null,
						FileAnswerSheet = filesAnswerSheet != null ? filesAnswerSheet.Where(f => f.OwnerId == p.Id && f.OwnerType == (byte)EnumFileType.AnswerSheetStudentNumber).Select(f => new
						{
							Id = f.Id,
							Name = !string.IsNullOrEmpty(f.OriginalName) ? f.OriginalName : f.Name,
							Path = f.Path,
							AllowLink = !f.ContentType.Equals(MimeType.CSV.GetDescription()),
							GenerationData = (!f.UpdateDate.Equals(f.CreateDate) ? f.UpdateDate.ToString("dd/MM/yyyy") : f.CreateDate.ToString("dd/MM/yyyy"))
						}) : null,
						FileFeedback = filesFeedback != null ? filesFeedback.Where(f => f.OwnerId == p.Id && f.OwnerType == (byte)EnumFileType.TestFeedback).Select(f => new
						{
							Id = f.Id,
							Name = !string.IsNullOrEmpty(f.OriginalName) ? f.OriginalName : f.Name,
							Path = f.Path,
							AllowLink = !f.ContentType.Equals(MimeType.CSV.GetDescription()),
							GenerationData = (!f.UpdateDate.Equals(f.CreateDate) ? f.UpdateDate.ToString("dd/MM/yyyy") : f.CreateDate.ToString("dd/MM/yyyy"))
						}) : null
					});

					return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Não existe(m) bloco(s) de prova criado(s)." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao buscar blocos da prova." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetHTMLObjTest(long Id, bool sheet)
		{
			try
			{
				Booklet booklet = bookletBusiness.GetTestBooklet(Id);

				if (booklet != null)
				{
					List<Block> blocks = blockBusiness.GetBookletItems(Id).ToList();
					booklet.Blocks = blocks;

					List<BlockItem> items = blocks.SelectMany(p => p.BlockItems.OfType<BlockItem>()).ToList();
					PDFFilter filter = new PDFFilter
					{
						Test = booklet.Test,
						BlockItemList = items,
						UrlSite = Request.Url.Authority.ToString(),
						GenerateAnswerSheet = sheet,
						PDFPreview = true,
						GenerateType = EnumGenerateType.Test,
						FileType = EnumFileType.Test,
						Booklet = booklet
					};

					string html = bookletBusiness.GetHtmlTest(filter, SessionFacade.UsuarioLogado.Usuario.ent_id);
					string htmlfeedback = testBusiness.GetTestFeedbackHtml(filter.Test);

					return Json(new { success = true, Test = html, TestFeedback = htmlfeedback }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Não existe o caderno selecionado." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao gerar prova." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GenerateTest(long Id, bool sheet, bool publicFeedback)
		{
			GenerateTestDTO ret = new GenerateTestDTO();

			try
			{
				bool CDNMathJax = bool.Parse(ApplicationFacade.Parameters.First(p => p.Key.Equals(EnumParameterKey.UTILIZACDNMATHJAX.GetDescription())).Value);

				string separator = ";";
				Parameter param = ApplicationFacade.Parameters.FirstOrDefault(p => p.Key.Equals(EnumParameterKey.CHAR_SEP_CSV.GetDescription()));
				if (param != null)
					separator = param.Value;

				ret = testBusiness.GenerateTest(Id, sheet, publicFeedback, CDNMathJax, separator, SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo, Request.Url.Authority.ToString(), ApplicationFacade.VirtualDirectory, ApplicationFacade.PhysicalDirectory);

				if (ret.Validate.IsValid)
				{
					var urlToRemove = Url.Action("GetHTMLTest", "Booklet");
					Response.RemoveOutputCacheItem(urlToRemove);
				}
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				if (ret.Validate == null)
				{
					ret.Validate = new Validate();
				}
				ret.Validate.Type = ValidateType.error.ToString();
				ret.Validate.IsValid = false;
				ret.Validate.Message = "Erro ao gerar prova.";
			}

			return Json(new { success = ret.Validate.IsValid, type = ret.Validate.Type, message = ret.Validate.Message, generateTest = ret }, JsonRequestBehavior.AllowGet);
		}


		[HttpGet]
		public void ExportTestDoc(long Id)
		{
			GenerateTestDTO ret = new GenerateTestDTO();
			try
			{
				bool CDNMathJax = bool.Parse(ApplicationFacade.Parameters.First(p => p.Key.Equals(EnumParameterKey.UTILIZACDNMATHJAX.GetDescription())).Value);

				PDFFilter filter = new PDFFilter
				{
					UrlSite = Request.Url.Authority.ToString(),
					ContentType = "application/pdf",
					VirtualDirectory = ApplicationFacade.VirtualDirectory,
					PhysicalDirectory = ApplicationFacade.PhysicalDirectory,
					GenerateType = EnumGenerateType.Test,
					FileType = EnumFileType.Test,
					PDFPreview = true,
					CDNMathJax = CDNMathJax
				};
				ret = testBusiness.ExportTestDoc(Id, filter, SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo);

				System.Web.HttpContext.Current.Response.Clear();
				System.Web.HttpContext.Current.Response.Charset = "UTF-8";
				System.Web.HttpContext.Current.Response.ContentType = "application/msword; charset=UTF-8";
				string strFileName = ret.File.Name + ".doc";
				System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;filename=" + strFileName);

				StringBuilder strHTMLContent = new StringBuilder();
				strHTMLContent.Append("<html xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:word\" xmlns=\"http://www.w3.org/TR/REC-html40\"><head></head><body>");




				strHTMLContent.Append(ret.Html);

				strHTMLContent.Append("</body></html>");
				System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Unicode;
				System.Web.HttpContext.Current.Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());
				System.Web.HttpContext.Current.Response.Write(strHTMLContent);
				//System.Web.HttpContext.Current.Response.Write('\uFEFF');
				System.Web.HttpContext.Current.Response.End();
				System.Web.HttpContext.Current.Response.Flush();

			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				if (ret.Validate == null)
				{
					ret.Validate = new Validate();
				}
				ret.Validate.Type = ValidateType.error.ToString();
				ret.Validate.IsValid = false;
				ret.Validate.Message = "Erro ao gerar prova.";
			}

			//return Json(new { success = ret.Validate.IsValid, type = ret.Validate.Type, message = ret.Validate.Message, generateTest = ret }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult CheckFilesExists(long Id)
		{
			bool success = false;

			try
			{
				IEnumerable<EntityFile> filesTest = fileBusiness.GetFilesByParent(Id, EnumFileType.Test);
				IEnumerable<EntityFile> filesAnswerSheet = fileBusiness.GetFilesByParent(Id, EnumFileType.AnswerSheetStudentNumber);
				IEnumerable<EntityFile> filesFeedback = fileBusiness.GetFilesByParent(Id, EnumFileType.TestFeedback);

				List<EntityFile> files = new List<EntityFile>();
				files.AddRange(filesTest);
				files.AddRange(filesAnswerSheet);
				files.AddRange(filesFeedback);

				success = fileBusiness.CheckFilesExists(files.Select(f => f.Id), ApplicationFacade.PhysicalDirectory);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
			}

			return Json(new { success = success }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public void DownloadZipFiles(long Id)
		{
			string completePath = string.Empty;
			bool redirect = false;

			try
			{
				IEnumerable<EntityFile> filesTest = fileBusiness.GetFilesByParent(Id, EnumFileType.Test);
				IEnumerable<EntityFile> filesAnswerSheet = fileBusiness.GetFilesByParent(Id, EnumFileType.AnswerSheetStudentNumber);
				IEnumerable<EntityFile> filesFeedback = fileBusiness.GetFilesByParent(Id, EnumFileType.TestFeedback);

				List<EntityFile> files = new List<EntityFile>();
				files.AddRange(filesTest);
				files.AddRange(filesAnswerSheet);
				files.AddRange(filesFeedback);

				if (files != null && files.Count > 0)
				{
					IEnumerable<ZipFileInfo> fileNames = files.Select(f => new ZipFileInfo
					{
						Path = string.Concat(ApplicationFacade.PhysicalDirectory, new Uri(f.Path).AbsolutePath.Replace("Files/", string.Empty).Replace("/", "\\")),
						Name = !string.IsNullOrEmpty(f.OriginalName) ? f.OriginalName : f.Name
					});

					var filenNotExists = fileNames.Where(i => !System.IO.File.Exists(HttpUtility.UrlDecode(i.Path)));
					if (filenNotExists != null && filenNotExists.Any())
					{
						redirect = true;
					}
					else
					{
						Test test = testBusiness.GetTestById(Id);

						string displayName = String.Format("ArquivosProva_{0}_{1}_{2}_{3}.zip", test != null ? "_" + test.Description : string.Empty, Id, DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HHmmss"));
						displayName = Regex.Replace(displayName, @"[^\w\.@-]", "_");

						string zipName = Guid.NewGuid() + ".zip";

						EntityFile file = fileBusiness.SaveZip(zipName, "Zip", fileNames, ApplicationFacade.PhysicalDirectory);
						if (file.Validate.IsValid)
						{
							completePath = Path.Combine(file.Path, zipName);

							System.IO.FileStream fs = System.IO.File.Open(completePath, System.IO.FileMode.Open);
							byte[] btFile = new byte[fs.Length];
							fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
							fs.Close();

							Response.Clear();
							Response.AddHeader("Content-disposition", "attachment; filename=" + displayName);
							Response.ContentType = "application/octet-stream";
							Response.BinaryWrite(btFile);
							Response.End();
						}
						else
							redirect = true;
					}
				}
			}
			catch (Exception ex)
			{
				redirect = true;
				LogFacade.SaveError(ex);
			}

			if (!string.IsNullOrEmpty(completePath) && System.IO.File.Exists(completePath))
				System.IO.File.Delete(completePath);

			if (redirect)
				Response.Redirect("/Test", false);
		}

		#endregion
	}
}