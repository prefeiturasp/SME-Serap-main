using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.IPDFConverter;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.LogFacade;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EntityFile = GestaoAvaliacao.Entities.File;


namespace GestaoAvaliacao.Business
{
	public class BookletBusiness : IBookletBusiness
	{
		private readonly IBookletRepository bookletRepository;
		private readonly IStorage storage;
		private readonly IFileBusiness fileBusiness;
		private readonly IHTMLToPDF htmltopdf;
		private readonly IModelTestBusiness modelTestBusiness;
		private readonly IBlockBusiness blockBusiness;
		private readonly IParameterBusiness parameterBusiness;
		private readonly IGenerateHtmlBusiness generateHtmlBusiness;

		public BookletBusiness(IBookletRepository bookletRepository, IStorage storage, IFileBusiness fileBusiness, IHTMLToPDF htmltopdf, IModelTestBusiness modelTestBusiness,
								IBlockBusiness blockBusiness, IParameterBusiness parameterBusiness, IGenerateHtmlBusiness generateHtmlBusiness)
		{
			this.bookletRepository = bookletRepository;
			this.storage = storage;
			this.fileBusiness = fileBusiness;
			this.htmltopdf = htmltopdf;
			this.modelTestBusiness = modelTestBusiness;
			this.blockBusiness = blockBusiness;
			this.parameterBusiness = parameterBusiness;
			this.generateHtmlBusiness = generateHtmlBusiness;
		}

		#region Custom

		private Validate Validate(PDFFilter filter, Validate valid)
		{
			valid.Message = null;

			if ((filter.GenerateType.Equals(EnumGenerateType.AnswerSheet)) && (filter.Test.TestType.ItemType == null || (filter.Test.TestType.ItemType != null && filter.Test.TestType.ItemType.QuantityAlternative == null)))
			{
				valid.Message = "Não existe um tipo de item associado ao tipo da prova.";
			}

			if ((filter.IdentificationType.Equals(EnumIdentificationType.QRCode)) && (filter.StudentList == null || (filter.StudentList != null && filter.StudentList.Count <= 0)))
			{
				valid.Message = "Não existem alunos para gerar o PDF com QRCode.";
			}

			if (!string.IsNullOrEmpty(valid.Message))
			{
				string br = "<br/>";
				valid.Message = valid.Message.TrimStart(br.ToCharArray());

				valid.IsValid = false;

				if (valid.Code <= 0)
					valid.Code = 400;

				valid.Type = ValidateType.alert.ToString();
			}
			else
				valid.IsValid = true;

			return valid;
		}

		#endregion

		#region Read

		public IEnumerable<Booklet> GetAllByTest(long TestId)
		{
			return bookletRepository.GetAllByTest(TestId);
		}

		public Booklet GetTestBooklet(long Id)
		{
			return bookletRepository.GetTestBooklet(Id);
		}

		public Booklet GetBookletByTest(long Id)
		{
			return bookletRepository.GetBookletByTest(Id);
		}

		public string GetHtmlTest(PDFFilter filter, Guid? EntityId)
		{
			filter.Test.TestType.ModelTest = filter.Test.TestType.ModelTest_Id != null ? modelTestBusiness.Get(filter.Test.TestType.ModelTest_Id.Value) : modelTestBusiness.GetDefault((Guid)EntityId);

			filter.Parameters = GetParameters();

			if (filter.Test.TestType.ModelTest != null)
			{
				filter.Logo = GetLogo(filter.Test.TestType.ModelTest.Id, filter.Test.TestType.ModelTest.Id);
			}

			return generateHtmlBusiness.GetContentHtml(filter);
		}

		public byte[] GetHtmlBytes(PDFFilter filter, Guid? EntityId)
		{
			List<Block> blocks = blockBusiness.GetBookletItems(filter.Booklet.Id).ToList();
			filter.Booklet.Blocks = blocks;

			filter.BlockItemList = blocks.SelectMany(p => p.BlockItems.OfType<BlockItem>()).ToList();

			filter.Parameters = GetParameters();

			filter.Test.TestType.ModelTest = filter.Test.TestType.ModelTest_Id != null ? modelTestBusiness.Get(filter.Test.TestType.ModelTest_Id.Value) : modelTestBusiness.GetDefault((Guid)EntityId);

			if (filter.Test.TestType.ModelTest != null)
			{
				filter.Logo = GetLogo(filter.Test.TestType.ModelTest.Id, filter.Test.TestType.ModelTest.Id);
			}

			filter.HtmlContent = string.Format("https://{0}/TestContent?id={1}&EntityId={2}&generateType={3}&fileType={4}", filter.UrlSite, filter.Booklet.Id, EntityId, (byte)filter.GenerateType, (byte)filter.FileType);
			
			string header = string.Empty;
			string footer = string.Empty;
			bool showBorder = false;
			float heightHeader = 55;
			float heightFooter = 75;
			if (filter.GenerateType.Equals(EnumGenerateType.Cover) || filter.GenerateType.Equals(EnumGenerateType.Test))
			{
				header = generateHtmlBusiness.GetHeader(filter);
				if (filter.Test.TestType.ModelTest.ShowLogoHeader && filter.Test.TestType.ModelTest.FileHeader_Id > 0)
				{
					switch (filter.Test.TestType.ModelTest.LogoHeaderSize)
					{
						case EnumSize.Default:
							heightHeader = 85; break;
						case EnumSize.Small:
							heightHeader = 55; break;
						case EnumSize.Big:
							heightHeader = 105; break;
					}
				}
				footer = generateHtmlBusiness.GetFooter(filter);
				if (filter.Test.TestType.ModelTest.ShowLogoFooter && filter.Test.TestType.ModelTest.FileFooter_Id > 0)
				{
					switch (filter.Test.TestType.ModelTest.LogoFooterSize)
					{
						case EnumSize.Default:
							heightFooter = 95; break;
						case EnumSize.Small:
							heightFooter = 75; break;
						case EnumSize.Big:
							heightFooter = 120; break;
					}
				}

				if (!filter.PDFPreview)
				{
					showBorder = filter.Test.TestType.ModelTest.ShowBorder;
				}

			}
			if (filter.GenerateType.Equals(EnumGenerateType.Test) || filter.GenerateType.Equals(EnumGenerateType.Items))
			{
				return htmltopdf.ConvertUrl(header, heightHeader, footer, heightFooter, filter.HtmlContent, filter.Test.Description, showBorder, (float)18.34, (float)18.34, (float)18.34, (float)18.34, 0, 0);
			}
			else
			{
				string contentHtml = @"<div class='pdf-content'>";

				if (filter.GenerateType.Equals(EnumGenerateType.AnswerSheet) && filter.StudentList != null && filter.StudentList.Count > 0)
				{
					int count = 1;
					StringBuilder sb = new StringBuilder();
					foreach (AnswerSheetStudentInformation student in filter.StudentList)
					{
						if (count != 1)
							sb.Append(@"<div class='pageBreakBefore'>");

						filter.Student = student;
						sb.Append(generateHtmlBusiness.GetContentHtml(filter));

						if (count != 1)
							sb.Append(@"</div>");

						count++;
					}
					contentHtml += sb.ToString();
				}
				else
					contentHtml += generateHtmlBusiness.GetContentHtml(filter);

				contentHtml += @"</div>";
				return htmltopdf.ConvertHtml(header, heightHeader, footer, heightFooter, contentHtml, filter.Test.Description, showBorder, (float)18.34, (float)18.34, (float)18.34, (float)18.34, 0, 0);
			}
		}

		public string GetBookletContent(PDFFilter filter, Guid EntityId)
		{
			filter.Parameters = GetParameters();

			filter.Booklet = this.GetTestBooklet(filter.Booklet.Id);
			filter.Test = filter.Booklet.Test;
			List<Block> blocks = blockBusiness.GetBookletItems(filter.Booklet.Id).ToList();
			filter.Booklet.Blocks = blocks;
			filter.Test.TestType.ModelTest = filter.Test.TestType.ModelTest_Id != null ? modelTestBusiness.Get(filter.Test.TestType.ModelTest_Id.Value) : modelTestBusiness.GetDefault(EntityId);
			filter.BlockItemList = blocks.SelectMany(p => p.BlockItems.OfType<BlockItem>()).ToList();

			if (filter.Test.TestType.ModelTest != null)
			{
				filter.Logo = GetLogo(filter.Test.TestType.ModelTest.Id, filter.Test.TestType.ModelTest.Id);
			}

			return generateHtmlBusiness.GetContentHtml(filter);
		}

		public EntityFile GetLogo(long ownerId, long parentId)
		{
			List<EntityFile> logos = fileBusiness.GetFilesByOwner(ownerId, parentId, EnumFileType.ModelTestHeader);
			EntityFile logo = logos.FirstOrDefault();

			if (logo != null)
			{
				string physicalPath = string.Concat(Constants.StorageFilePath, new Uri(logo.Path).AbsolutePath.Replace("Files/", string.Empty).Replace("/", "\\"));

				if (!System.IO.File.Exists(physicalPath))
				{
					logo = null;
				}
			}

			return logo;
		}

		private List<Parameter> GetParameters()
		{
			List<Parameter> parameters = parameterBusiness.GetParamsByPage(3).ToList();
			parameters.AddRange(parameterBusiness.GetParamsByPage(4).ToList());
			return parameters;
		}

		#endregion

		#region Write

		public EntityFile SavePdfTest(PDFFilter filter, Guid? EntityId)
		{
			EntityFile entity = new EntityFile();
			entity.Validate = Validate(filter, entity.Validate);
			if (entity.Validate.IsValid)
			{
				filter.Booklet = this.GetTestBooklet(filter.Booklet.Id);
				filter.PDFBytes = GetHtmlBytes(filter, EntityId);
				entity = this.SavePdfTest(filter);
			}

			return entity;
		}

		public EntityFile SavePdfTest(PDFFilter filter)
		{
			long ownerId = 0;
			long parentOwnerId = 0;

			if (filter.GenerateType.Equals(EnumGenerateType.AnswerSheet))
			{
				ownerId = filter.OwnerId;
				parentOwnerId = filter.ParentOwnerId;
			}
			else
			{
				ownerId = filter.Booklet.Id;
				parentOwnerId = filter.Booklet.Test.Id;
			}

			string fileTypeDescription = filter.FileType.GetDescription();

			string originalName = String.Format("{0}_{1}_{2}_{3}.pdf", parentOwnerId, ownerId, fileTypeDescription, filter.Booklet.Test.Description);

			if (!string.IsNullOrEmpty(filter.OriginalName))
				originalName = filter.OriginalName;

			originalName = Regex.Replace(originalName, @"[^\w\.@-]", "_");

			string name = Guid.NewGuid() + ".pdf";

			if (!string.IsNullOrEmpty(filter.FileName))
				name = filter.FileName;

			string folderName = string.Format("{0}\\{1}\\{2}", string.IsNullOrEmpty(filter.FolderName) ? fileTypeDescription : filter.FolderName, DateTime.Today.Year, DateTime.Today.Month);

			EnumFileType fileType = filter.FileType;

			EntityFile filedb = fileBusiness.GetFilesByOwner(ownerId, parentOwnerId, fileType).FirstOrDefault();
			if (filedb != null)
			{
				if (!string.IsNullOrEmpty(filedb.OriginalName))
					originalName = filedb.OriginalName;

				name = filedb.Name;
			}

			EntityFile entityFile;
			EntityFile fileUploaded = storage.Save(filter.PDFBytes, name, filter.ContentType, folderName, filter.VirtualDirectory, filter.PhysicalDirectory, out entityFile);
			if (fileUploaded.Validate.IsValid && filter.SaveToDB)
			{
				fileUploaded.OriginalName = originalName;
				fileUploaded.OwnerId = ownerId;
				fileUploaded.ParentOwnerId = parentOwnerId;

				if (filedb != null)
				{
					fileBusiness.Update(filedb.Id, fileUploaded);
				}
				else
				{
					fileUploaded.OwnerType = (byte)fileType;
					fileBusiness.Save(fileUploaded);
				}
			}
			else
				entityFile.Validate = fileUploaded.Validate;

			return entityFile;
		}

		#endregion
	}
}
