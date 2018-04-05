using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities.DTO;
using GestaoAvaliacao.Util;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Business
{
	public class ReportTestPerformanceBusiness : IReportTestPerformanceBusiness
	{
		private readonly ICorrectionResultsRepository _correctionResultsRepository;
		private readonly IAdherenceBusiness _adherenceBusiness;
		private readonly IFileBusiness _fileBusiness;
		private readonly IStorage _storage;

		public ReportTestPerformanceBusiness(ICorrectionResultsRepository correctionResultsRepository,
			IAdherenceBusiness adherenceBusiness, IFileBusiness fileBusiness, IStorage storage)
		{
			_correctionResultsRepository = correctionResultsRepository;
			_adherenceBusiness = adherenceBusiness;
			_fileBusiness = fileBusiness;
			_storage = storage;
		}

		public List<TestAverageDreDTO> GetTestAverageByTestGroupByDre(long testId, long? discipline_id)
		{
			return _correctionResultsRepository.GetTestAverageByTestGroupByDre(testId, discipline_id);
		}

		public List<TestAverageSchoolDTO> GetTestAverageByTestDreGroupBySchool(long testId, long? discipline_id, Guid dre_id)
		{
			return _correctionResultsRepository.GetTestAverageByTestDreGroupBySchool(testId, discipline_id, dre_id);
		}

		public List<TestAveragePerformanceDTO> GetTestAveragePerformanceGeneral(long test_Id, long? discipline_id)
		{
			return _correctionResultsRepository.GetTestAveragePerformanceGeneral(test_Id, discipline_id);
		}

		public TestAverageViewModel ObterDresDesempenho(long provaId, long? discipline_id, SYS_Usuario usuario, SYS_Grupo grupo)
		{
			var dres = _adherenceBusiness.LoadDreSimpleAdherence(provaId, usuario, grupo);
			List<TestAverageDreDTO> medias = _correctionResultsRepository.GetTestAverageByTestGroupByDre(provaId, discipline_id);

			int totalQuestoes = medias.Sum(p => p.TotalItems);
			int totalQuestoesCorretas = medias.Sum(p => p.TotalCorretItems);
			double calculoMediaSME = 0;
			if (totalQuestoes != 0)
			{
				calculoMediaSME = ((double)totalQuestoesCorretas * 100 / totalQuestoes);
			}

			TestAveragePerformanceDTO testAveragePerformanceDTO = new TestAveragePerformanceDTO();
			testAveragePerformanceDTO.Media = Math.Round(calculoMediaSME, 2);

			var query = (from dre in dres
						 join media in medias on new Guid(dre.EntityId) equals media.Dre_id
						 select new TestAveragePerformanceDTO { DreId = new Guid(dre.EntityId), DreName = dre.Description, Media = (media != null && !double.IsNaN(media.Media)) ? Math.Round(media.Media, 2) : 0 })
						.ToList();

			return new TestAverageViewModel { success = true, lista = query, MediaSME = testAveragePerformanceDTO };
		}

		public TestAverageViewModel ObterEscolasDesempenho(long provaId, long? discipline_id, Guid DreId, SYS_Usuario usuario, SYS_Grupo grupo)
		{
			var escolas = _adherenceBusiness.LoadSchoolSimpleAdherence(provaId, usuario, grupo, DreId);
			List<TestAverageSchoolDTO> medias = _correctionResultsRepository.GetTestAverageByTestDreGroupBySchool(provaId, discipline_id, DreId);

			var query = (from escola in escolas
						 join media in medias on Convert.ToInt32(escola.EntityId) equals media.Esc_id
						 select new TestAveragePerformanceDTO { EscId = Convert.ToInt32(escola.EntityId), EscName = escola.Description, Media = media != null ? Math.Round(media.Media, 2) : 0 })
						 .ToList();

			return new TestAverageViewModel { success = true, lista = query };
		}

		public EntityFile ExportReport(List<TestAveragePerformanceDTO> lista, TypeReportsPerformanceExport typeExport, string separator, string virtualDirectory, string physicalDirectory, SYS_Usuario usuario)
		{
			EntityFile ret = new EntityFile();
			StringBuilder stringBuilder = new StringBuilder();
			string fileName = string.Empty;

			if (lista != null)
			{
				if (typeExport == TypeReportsPerformanceExport.Dre)
				{
					fileName = "Rel_Test_Performance_DRE";
					stringBuilder.Append(string.Format("DRE{0} Desempenho{0}", separator));
					stringBuilder.AppendLine();

					foreach (TestAveragePerformanceDTO entity in lista)
					{
						stringBuilder.Append(entity.DreName + separator);
						stringBuilder.Append(entity.Media + " %" + separator);
						stringBuilder.AppendLine();
					}
				}
				else
				{
					fileName = "Rel_Test_Performance_Escola";
					stringBuilder.Append(string.Format("Escola{0} Desempenho{0}", separator));
					stringBuilder.AppendLine();

					foreach (TestAveragePerformanceDTO entity in lista)
					{
						stringBuilder.Append(entity.EscName + separator);
						stringBuilder.Append(entity.Media + " %" + separator);
						stringBuilder.AppendLine();
					}
				}
			}

			var fileContent = stringBuilder.ToString();
			if (!string.IsNullOrEmpty(fileContent))
			{
				byte[] buffer = System.Text.Encoding.Default.GetBytes(fileContent);
				string originalName = string.Format("{0}.csv", fileName);
				string name = string.Format("{0}.csv", Guid.NewGuid());
				string contentType = MimeType.CSV.GetDescription();

				var csvFiles = _fileBusiness.GetAllFilesByType(EnumFileType.ExportReportTestPerformance, DateTime.Now.AddDays(-1));
				if (csvFiles != null && csvFiles.Count() > 0)
				{
					_fileBusiness.DeletePhysicalFiles(csvFiles.ToList(), physicalDirectory);
					_fileBusiness.DeleteFilesByType(EnumFileType.ExportReportTestPerformance, DateTime.Now.AddDays(-1));
				}

				ret = _storage.Save(buffer, name, contentType, EnumFileType.ExportReportTestPerformance.GetDescription(), virtualDirectory, physicalDirectory, out ret);
				if (ret.Validate.IsValid)
				{
					ret.Name = name;
					ret.ContentType = contentType;
					ret.OriginalName = StringHelper.Normalize(originalName);
					ret.OwnerId = 0;
					ret.ParentOwnerId = 0;
					ret.OwnerType = (byte)EnumFileType.ExportReportTestPerformance;
					ret = _fileBusiness.Save(ret);
				}
			}
			else
			{
				ret.Validate.IsValid = false;
				ret.Validate.Type = ValidateType.alert.ToString();
				ret.Validate.Message = "Os dados ainda não foram gerados.";
			}

			return ret;
		}
	}
}
