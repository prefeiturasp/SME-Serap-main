using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.IPDFConverter;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Services
{
	public class AnswersheetLotService
	{
		private readonly Func<ServiceStatusInfo, EnumServiceState, AnswerSheetServiceMessage, ServiceStatusInfo> finishServiceStatusInfo = (x, y, z) =>
		{
			x.ServiceStateCode = y;
			x.Description = z.GetDescription();
			x.FinishDate = DateTime.Now;
			return x;
		};

		private readonly IAnswerSheetLotBusiness answerSheetLotBusiness;
		private readonly IAdherenceBusiness adherenceBusiness;
		private readonly ITestBusiness testBusiness;
		private readonly IAnswerSheetBusiness answerSheetBusiness;
		private readonly IBookletBusiness bookletBusiness;
		private readonly IModelTestBusiness modelTestBusiness;
		private readonly IBlockBusiness blockBusiness;
		private readonly IItemTypeBusiness itemTypeBusiness;
		private readonly IHTMLToPDF htmltopdf;
		private readonly IStorage storage;
		private readonly IPDFMerger pDFMerger;
		private readonly IFileBusiness fileBusiness;
		private readonly IParameterBusiness parameterBusiness;
		private readonly IServiceAnswerSheetInfoBusiness serviceAnswersheetInfoBusiness;
		private readonly IGenerateHtmlBusiness generateHtmlBusiness;
        private readonly ISectionTestGenerateLotBusiness sectionTestGenerateLotBusiness;
        private ServiceAnswerSheetInfo _logService;

		public AnswersheetLotService(IAnswerSheetLotBusiness answerSheetLotBusiness, IAdherenceBusiness adherenceBusiness, ITestBusiness testBusiness,
			IAnswerSheetBusiness answerSheetBusiness, IBookletBusiness bookletBusiness, IModelTestBusiness modelTestBusiness,
			IBlockBusiness blockBusiness, IItemTypeBusiness itemTypeBusiness, IHTMLToPDF htmltopdf, IStorage storage, IPDFMerger pDFMerger, IFileBusiness fileBusiness,
			IParameterBusiness parameterBusiness,
			IServiceAnswerSheetInfoBusiness serviceAnswersheetInfoBusiness,
			IGenerateHtmlBusiness generateHtmlBusiness, ISectionTestGenerateLotBusiness sectionTestGenerateLotBusiness)
		{
			this.answerSheetLotBusiness = answerSheetLotBusiness;
			this.adherenceBusiness = adherenceBusiness;
			this.testBusiness = testBusiness;
			this.answerSheetBusiness = answerSheetBusiness;
			this.bookletBusiness = bookletBusiness;
			this.modelTestBusiness = modelTestBusiness;
			this.blockBusiness = blockBusiness;
			this.itemTypeBusiness = itemTypeBusiness;
			this.htmltopdf = htmltopdf;
			this.storage = storage;
			this.pDFMerger = pDFMerger;
			this.fileBusiness = fileBusiness;
			this.parameterBusiness = parameterBusiness;
			this.serviceAnswersheetInfoBusiness = serviceAnswersheetInfoBusiness;
			this.generateHtmlBusiness = generateHtmlBusiness;
            this.sectionTestGenerateLotBusiness = sectionTestGenerateLotBusiness;
        }

		public void Execute(string serviceName)
		{
			try
			{
				var virtualPath = ConfigurationManager.AppSettings["VirtualPath"];
				var phisicalPath = ConfigurationManager.AppSettings["StoragePath"];
				//verifica quantidade limite de provas processando
				var LimitProcessing = ConfigurationManager.AppSettings["LimitProcessing"];

				if (!virtualPath.EndsWith(@"/"))
					virtualPath = virtualPath + @"/";

				if (!phisicalPath.EndsWith(@"\"))
					phisicalPath = phisicalPath + @"\";

				Init(phisicalPath);

				ExecuteLot(phisicalPath, virtualPath, Convert.ToInt32(LimitProcessing), serviceName);
			}
			catch (Exception e)
			{
				LogFacade.LogFacade.SaveError(e);
			}
		}

		private void Init(string phisicalPath)
		{
			string folderToSaveZip = string.Format("{0}\\{1}\\{2}\\", phisicalPath, EnumFileType.AnswerSheetLot.GetDescription(), DateTime.Today.Year);

			if (!Directory.Exists(folderToSaveZip + "Prova"))
				Directory.CreateDirectory(folderToSaveZip + "Prova");

			if (!Directory.Exists(folderToSaveZip + "Escola"))
				Directory.CreateDirectory(folderToSaveZip + "Escola");
		}

		private void ExecuteLot(string phisicalPath, string virtualPath, int LimitProcessing, string serviceName)
		{
			IEnumerable<AnswerSheetLot> list = answerSheetLotBusiness.GetParentByExecutionState(EnumServiceState.Processing);

			//verifica se a quantidade limite de provas processando foi atingida
			if (list != null && list.Count() < LimitProcessing)
			{
				var lots = answerSheetLotBusiness.GetParentByExecutionState(EnumServiceState.Pending);

				foreach (var lot in lots)
				{
					ProcessMainLot(phisicalPath, virtualPath, serviceName, lot);
				}
			}
		}

		private void ProcessMainLot(string phisicalPath, string virtualPath, string serviceName, AnswerSheetLot lot)
		{
			//Início Processamento do Lote.

			EnumServiceState serviceStateCode = EnumServiceState.Processing;
			AnswerSheetServiceMessage answerSheetServiceMessage = AnswerSheetServiceMessage.MainProcessingLot;

			ServiceStatusInfo statusService = new ServiceStatusInfo
			{
				Description = answerSheetServiceMessage.GetDescription(),
				ServiceStateCode = serviceStateCode,
				StartDate = DateTime.Now
			};

			_logService = new ServiceAnswerSheetInfo
			{
				LotId = lot.Id,
				Status = statusService
			};

			UpdateLog();

			List<AnswerSheetLotDTO> results = new List<AnswerSheetLotDTO>();
			try
			{
				List<AnswerSheetLotDTO> resultslot = new List<AnswerSheetLotDTO>();
				AnswerSheetLot currentLot = answerSheetLotBusiness.GetById(lot.Id);
				currentLot.ExecutionOwner = serviceName;
				if (currentLot.StateExecution.Equals(EnumServiceState.Pending))
				{
					if (currentLot.Type.Equals(EnumAnswerSheetBatchOwner.Test))
					{
						results = ProcessChildLot(currentLot, phisicalPath, virtualPath, serviceName);
						results = results.GroupBy(p => p.FileName).Select(g => g.First()).ToList();
						Result(results, currentLot, phisicalPath, virtualPath, serviceName);
					}
					else if (currentLot.Type.Equals(EnumAnswerSheetBatchOwner.School))
					{
						currentLot.StateExecution = EnumServiceState.Processing;
						answerSheetLotBusiness.Update(currentLot, true, false);

						IEnumerable<AnswerSheetLot> sublots = answerSheetLotBusiness.GetByParentId(currentLot.Id);
						foreach (var sublot in sublots)
						{
							results = ProcessChildLot(sublot, phisicalPath, virtualPath, serviceName);
							resultslot.AddRange(results);
						}
						resultslot = resultslot.GroupBy(p => p.FileName).Select(g => g.First()).ToList();
						Result(resultslot, currentLot, phisicalPath, virtualPath, serviceName);
					}
				}

				//Busca lot com erro
				if (_logService.HasError())
				{
					serviceStateCode = EnumServiceState.Error;
					answerSheetServiceMessage = AnswerSheetServiceMessage.MainProcessCompletedWithError;
				}
				else
				{
					serviceStateCode = EnumServiceState.Success;
					answerSheetServiceMessage = AnswerSheetServiceMessage.MainProcessCompleted;
				}
			}
			catch (Exception e)
			{
				serviceStateCode = EnumServiceState.Error;
				answerSheetServiceMessage = AnswerSheetServiceMessage.MainProcessCompletedWithError;

				lot.StateExecution = EnumServiceState.Error;
				lot.ExecutionOwner = serviceName;
				answerSheetLotBusiness.Update(lot, true);

				LogFacade.LogFacade.SaveError(e, string.Format("ExecuteLot: Erro ao gerar folha de resposta em lote para o lote {0}.", lot.Id));
			}
			finally
			{
				statusService = finishServiceStatusInfo(statusService, serviceStateCode, answerSheetServiceMessage);
				_logService.Status = statusService;
				UpdateLog();
			}
		}

		private void UpdateLog()
		{
			try
			{
				var task = Task.Run(async () =>
				{
					await serviceAnswersheetInfoBusiness.Save(_logService);
					return await serviceAnswersheetInfoBusiness.Get(_logService);
				});

				task.Wait();
				_logService = task.Result;
			}
			catch (Exception ex)
			{
				LogFacade.LogFacade.SaveError(ex, "Esta exception não é lançada na pilha de rastreamento, possui um comportamento de try/catch vazio.");
			}
		}

		private List<AnswerSheetLotDTO> ProcessChildLot(AnswerSheetLot lot, string phisicalPath, string virtualPath, string serviceName)
		{
			EnumServiceState serviceStateCode = EnumServiceState.Processing;
			AnswerSheetServiceMessage answerSheetServiceMessage = AnswerSheetServiceMessage.ChildProcessingLot;

			ServiceStatusInfo statusSubLote = new ServiceStatusInfo
			{
				Description = answerSheetServiceMessage.GetDescription(),
				ServiceStateCode = serviceStateCode,
				StartDate = DateTime.Now
			};

			LotServiceAnswerSheetInfo lotServiceInfo = new LotServiceAnswerSheetInfo
			{
				LotId = lot.Id,
				ParentLotId = lot.ParentId,
				Status = statusSubLote
			};
			_logService.AddOrUpdateLot(lotServiceInfo);
			UpdateLog();

			try
			{
				List<AnswerSheetLotDTO> results = new List<AnswerSheetLotDTO>();
				string typeLotFolder = lot.Type.Equals(EnumAnswerSheetBatchOwner.Test) ? "P_" : "E_";

				lot.StateExecution = EnumServiceState.Processing;
				lot.ExecutionOwner = serviceName;
				answerSheetLotBusiness.Update(lot, true, false);

				var booklet = bookletBusiness.GetBookletByTest(lot.TestId);
				List<Block> blocks = blockBusiness.GetBookletItems(booklet.Id).ToList();
				booklet.Blocks = blocks;
				var qtdeItens = blocks.SelectMany(p => p.BlockItems.OfType<BlockItem>()).ToList().Count();
				booklet.Test.TestType.ModelTest = modelTestBusiness.Get(booklet.Test.TestType.ModelTest_Id.Value);
				booklet.Test.TestType.ItemType = itemTypeBusiness.Get(booklet.Test.TestType.ItemType_Id.Value);

				var logo = bookletBusiness.GetLogo(booklet.Test.TestType.ModelTest.Id, booklet.Test.TestType.ModelTest.Id);

				var sections = adherenceBusiness.GetSectionsToAnswerSheetLot(booklet.Test.Id, booklet.Test.TestType.Id, booklet.Test.AllAdhered, lot.esc_id, lot.uad_id);
				var dres = (from ua in sections
							group ua by ua.dre_sigla into DREs
							from esc in
								(from esc in DREs
								 group esc by esc.esc_nome)
							group esc by DREs.Key);

				foreach (var dre in dres)
				{
					foreach (var esc in dre)
					{
						results.AddRange(ProcessSchool(lot, phisicalPath, virtualPath, typeLotFolder, booklet, qtdeItens, logo, dre, esc));
					}

					if (lot.Type.Equals(EnumAnswerSheetBatchOwner.Test))
					{
						string folder = string.Format("\\{0}\\{1}\\{2}\\{3}", EnumFileType.AnswerSheetLot.GetDescription(), DateTime.Today.Year, string.Format("{0}{1}", typeLotFolder, booklet.Test.Id.ToString()), dre.Key);
						ClearFolder(string.Concat(phisicalPath, folder), lot.Id, false);
					}
				}

				if (_logService.LotHasError(lot.Id))
				{
					serviceStateCode = EnumServiceState.Error;
					answerSheetServiceMessage = AnswerSheetServiceMessage.ChildProcessCompletedWithError;
				}
				else
				{
					serviceStateCode = EnumServiceState.Success;
					answerSheetServiceMessage = AnswerSheetServiceMessage.ChildProcessCompleted;
				}

				return results;
			}
			catch (Exception e)
			{
				serviceStateCode = EnumServiceState.Error;
				answerSheetServiceMessage = AnswerSheetServiceMessage.ChildProcessCompletedWithError;

				UpdateAnswerShetLot(lot, null, EnumServiceState.Error, serviceName);

				LogFacade.LogFacade.SaveError(e, string.Format("GenerateLot: Erro ao gerar folha de resposta em lote para o lote {0}.", lot.Id));
				throw;
			}
			finally
			{
				statusSubLote = finishServiceStatusInfo(statusSubLote, serviceStateCode, answerSheetServiceMessage);

				_logService.UpdateStatusLot(lot.Id, statusSubLote);

				UpdateLog();
			}
		}

		private List<AnswerSheetLotDTO> ProcessSchool(AnswerSheetLot lot, string phisicalPath, string virtualPath, string typeLotFolder, Booklet booklet, int qtdeItens, Entities.File logo, IGrouping<string, IGrouping<string, AdherenceDTO>> dre, IGrouping<string, AdherenceDTO> esc)
		{
			EnumServiceState serviceStateCode = EnumServiceState.Processing;
			AnswerSheetServiceMessage answerSheetServiceMessage = AnswerSheetServiceMessage.SchoolProcessingAnswerSheet;

			ServiceStatusInfo statusSchool = new ServiceStatusInfo
			{
				ServiceStateCode = serviceStateCode,
				Description = answerSheetServiceMessage.GetDescription(),
				StartDate = DateTime.Now
			};

			List<AnswerSheetLotDTO> results = new List<AnswerSheetLotDTO>();
			bool schoolHasStudent = false;
			var school = esc.FirstOrDefault();
			int esc_id = school.esc_id;
            List<SectionTestGenerateLot> listSectionTestGenerateLot = new List<SectionTestGenerateLot>();

            var schoolServiceInfo = new SchoolServiceAnswerSheetInfo
			{
				SchoolId = esc_id,
				SchoolName = school.esc_nome,
				Status = statusSchool
			};
			try
			{
				_logService.AddOrUpdateSchoolLot(lot.Id, schoolServiceInfo);

				UpdateLog();

				string mergeFile = lot.Type.Equals(EnumAnswerSheetBatchOwner.Test) ? esc.Key.ToString() : booklet.Test.Description.ToString();
				string partialFolder = lot.Type.Equals(EnumAnswerSheetBatchOwner.Test) ? string.Format("{0}\\{1}\\{2}", booklet.Test.Id.ToString(), dre.Key, esc_id) : string.Format("{0}\\{1}\\{2}\\{3}", lot.ParentId.ToString(), dre.Key, esc_id, booklet.Test.Id.ToString());
				string folderName = string.Format("{0}\\{1}\\{2}{3}", EnumFileType.AnswerSheetLot.GetDescription(), DateTime.Today.Year, typeLotFolder, partialFolder).Replace(".", string.Empty);

				var countAnswerSheetGenerated = 0;
				var countanswerSheetQuantityToGenerate = 0;

                var studentsBySchool = testBusiness.GetTeamStudents(esc_id, 0, 0, booklet.Test.Id, booklet.Test.AllAdhered).ToList();
                if (studentsBySchool != null && studentsBySchool.Count > 0)
                {
                    foreach (var section in esc)
                    {
                        var sectionInfo = new SectionInfo { tur_id = section.tur_id };
                        var stopWatch = new Stopwatch();

                        try
                        {
                            var students = studentsBySchool.FindAll(p => p.SectionId == section.tur_id);

                            if (students != null && students.Count > 0)
                            {
                                List<long> studentsSection = new List<long>();
                                studentsSection.AddRange(students.Select(p => p.Id));
                                SectionTestGenerateLot sectionTestGenerateLot = new SectionTestGenerateLot
                                {
                                    test_id = booklet.Test.Id,
                                    esc_id = esc_id,
                                    tur_id = section.tur_id,
                                    alu_ids = studentsSection
                                };
                                listSectionTestGenerateLot.Add(sectionTestGenerateLot);

                                sectionInfo.QuantityToGenerate = students.Count;

                                countanswerSheetQuantityToGenerate += students.Count;

                                schoolHasStudent = true;
                                stopWatch.Start();

                                GenerateQRCodes(students, virtualPath, phisicalPath, booklet.Test.Id, section.esc_id, section.tur_id, folderName);
                                stopWatch.Stop();

                                sectionInfo.TotalMillisecondsQrCode = stopWatch.ElapsedMilliseconds;

                                stopWatch.Restart();

                                GenerateSectionPDF(students, section, qtdeItens, booklet, virtualPath, phisicalPath, folderName, logo);

                                stopWatch.Stop();
                                sectionInfo.TotalMillisecondsPdf = stopWatch.ElapsedMilliseconds;


                                //Sucesso
                                countAnswerSheetGenerated += students.Count;
                            }
                        }
                        finally
                        {
                            schoolServiceInfo.Sections.Add(sectionInfo);
                            schoolServiceInfo.AnswerSheetCountGenerated = countAnswerSheetGenerated;
                            schoolServiceInfo.AnswerSheetQuantityToGenerate = countanswerSheetQuantityToGenerate;
                            _logService.AddOrUpdateSchoolLot(lot.Id, schoolServiceInfo);
                            UpdateLog();
                        }
                    }
                }

				if (schoolHasStudent)
				{
                    // Salva a indicação dos alunos que tiveram a folha gerada por lote.
                    if (listSectionTestGenerateLot.Count > 0)
                    {
                        sectionTestGenerateLotBusiness.Save(listSectionTestGenerateLot);
                    }

                    statusSchool.Description = AnswerSheetServiceMessage.SchoolProcessAnswerSheetMerge.GetDescription();
					statusSchool.ServiceStateCode = EnumServiceState.Processing;
					_logService.UpdateStatusSchoolLot(lot.Id, esc_id, statusSchool);
					UpdateLog();

					MergePDFFiles(folderName, mergeFile, virtualPath, Constants.StorageFilePath, lot.Id);

					if (lot.Type.Equals(EnumAnswerSheetBatchOwner.Test) && !results.Any(i => i.TestCode.Equals(lot.TestId)))
					{
						results.Add(new AnswerSheetLotDTO
						{
							Id = lot.Id,
							TestCode = lot.TestId,
							Description = booklet.Test.Description,
							FileName = string.Format("{0}_{1}", lot.Id, StringHelper.RemoveSpecialCharactersWithRegex(booklet.Test.Description, Constants.UnderlineChar.ToString())),
							Type = EnumAnswerSheetBatchOwner.Test,
							FilePath = string.Format("{0}{1}", typeLotFolder, booklet.Test.Id.ToString())
						});
					}
					else if (lot.Type.Equals(EnumAnswerSheetBatchOwner.School) && !results.Any(i => i.TestCode.Equals(esc_id) && i.Parent_Id.Equals(lot.ParentId)))
					{
						results.Add(new AnswerSheetLotDTO
						{
							Id = lot.Id,
							Parent_Id = lot.ParentId,
							TestCode = esc_id,
							Description = esc_id.ToString(),
							FileName = string.Format("{0}_{1}", lot.ParentId, StringHelper.RemoveSpecialCharactersWithRegex(esc.Key, Constants.UnderlineChar.ToString())),
							Type = EnumAnswerSheetBatchOwner.School,
							FilePath = string.Format("{0}{1}\\{2}", typeLotFolder, lot.ParentId.ToString(), dre.Key),
							SupAdmUnitName = dre.Key

						});
					}
				}

				if (lot.Type.Equals(EnumAnswerSheetBatchOwner.School))
				{
					string folder = string.Format("\\{0}\\{1}\\{2}\\{3}\\{4}", EnumFileType.AnswerSheetLot.GetDescription(), DateTime.Today.Year, string.Format("{0}{1}", typeLotFolder, lot.ParentId.ToString()), dre.Key, esc_id);
					ClearFolder(string.Concat(phisicalPath, folder), lot.Id, false);
				}

				serviceStateCode = EnumServiceState.Success;
				answerSheetServiceMessage = AnswerSheetServiceMessage.SchoolProcessAnswerSheetCompleted;
			}
			catch
			{
				serviceStateCode = EnumServiceState.Error;
				answerSheetServiceMessage = AnswerSheetServiceMessage.SchoolProcessAnswerSheetCompletedWithError;
				throw;
			}
			finally
			{
				statusSchool = finishServiceStatusInfo(statusSchool, serviceStateCode, answerSheetServiceMessage);
				_logService.UpdateStatusSchoolLot(lot.Id, esc_id, statusSchool);
				UpdateLog();
			}

			return results;
		}

		private void Result(List<AnswerSheetLotDTO> results, AnswerSheetLot lot, string phisicalPath, string virtualPath, string serviceName)
		{
			try
			{
				foreach (var test in results)
				{
					string folderFiles = test.Type.Equals(EnumAnswerSheetBatchOwner.School) ? test.Description : null;

					ZipFiles(phisicalPath, virtualPath, test.FileName, test.FilePath, folderFiles, test.Id, test.TestCode, test.Type, test.SupAdmUnitName);
				}

				results = results.GroupBy(p => p.FilePath).Select(g => g.First()).ToList();

				if (lot.Type.Equals(EnumAnswerSheetBatchOwner.School))
				{
					foreach (var test in results)
					{
						string folder = string.Format("{0}\\{1}\\{2}\\{3}", phisicalPath, EnumFileType.AnswerSheetLot.GetDescription(), DateTime.Today.Year, test.FilePath);
						if (Directory.Exists(folder))
							ClearFolder(folder, test.Id, true);
					}
				}

				lot.StateExecution = EnumServiceState.Success;
				lot.ExecutionOwner = serviceName;
				answerSheetLotBusiness.Update(lot, true);
			}
			catch (Exception e)
			{
				LogFacade.LogFacade.SaveError(e, error: string.Format("Erro ao criar zip do lote {0}.", lot.Id));
				throw;
			}
		}

		private AnswerSheetLot UpdateAnswerShetLot(AnswerSheetLot lot, AnswerSheetLot parentLot, EnumServiceState status, string serviceName)
		{
			try
			{
				lot.StateExecution = status;
				lot.ExecutionOwner = serviceName;
				answerSheetLotBusiness.Update(lot, true, false);

				if (lot.Type.Equals(EnumAnswerSheetBatchOwner.School))
				{
					if (parentLot == null || (parentLot != null && !parentLot.Id.Equals(lot.ParentId)))
					{
						parentLot = answerSheetLotBusiness.GetById(lot.ParentId);
					}

					if (parentLot != null && parentLot.Id > 0 && !parentLot.StateExecution.Equals(status))
					{
						parentLot.StateExecution = status;
						parentLot.ExecutionOwner = serviceName;
						answerSheetLotBusiness.Update(parentLot, true, false);
					}
				}
			}
			catch (Exception e)
			{
				LogFacade.LogFacade.SaveError(e, error: string.Format("Erro ao atualizar o lote {0}.", lot.Id));
				throw;
			}

			return parentLot;
		}

		private bool ZipFiles(string phisicalPath, string virtualPath, string fileName, string lotFolder, string folderFiles, long fileOwnerId, long fileParentId, EnumAnswerSheetBatchOwner lotType, string dre)
		{
			try
			{
				fileName = StringHelper.RemoveSpecialCharactersWithRegex(fileName, Constants.UnderlineChar.ToString());
				string nomeArquivo = string.Concat(fileName, ".zip");



				string zipFolder = lotType.Equals(EnumAnswerSheetBatchOwner.Test) ? "Prova" : "Escola";
				if (!string.IsNullOrEmpty(dre))
					zipFolder = string.Format("{0}\\{1}", zipFolder, dre);

				string folderToSaveZip = string.Format("{0}\\{1}\\{2}\\{3}", phisicalPath, EnumFileType.AnswerSheetLot.GetDescription(), DateTime.Today.Year, zipFolder);
				string folderToGetFiles = string.Format("{0}\\{1}\\{2}\\{3}", phisicalPath, EnumFileType.AnswerSheetLot.GetDescription(), DateTime.Today.Year, lotFolder);
				if (!string.IsNullOrEmpty(folderFiles))
					folderToGetFiles = string.Format("{0}\\{1}", folderToGetFiles, folderFiles);

				if (!Directory.Exists(folderToSaveZip))
				{
					Directory.CreateDirectory(folderToSaveZip);
				}

				folderToSaveZip = Path.Combine(folderToSaveZip, nomeArquivo);

				CreateFile(folderToSaveZip, folderToGetFiles);

				FileInfo fileZip = new FileInfo(folderToSaveZip);
				if (fileZip.Exists)
				{
					string virtualPathFile = string.Format("{0}{1}/{2}/{3}/{4}", virtualPath, EnumFileType.AnswerSheetLot.GetDescription(), DateTime.Today.Year, zipFolder, nomeArquivo).Replace('\\', '/');
					Entities.File fileDB = CreateFileRegister(fileName + ".zip", fileOwnerId, virtualPathFile, fileParentId, nomeArquivo);

					if (fileDB != null && fileDB.Validate.IsValid)
					{
						if (lotType.Equals(EnumAnswerSheetBatchOwner.Test))
							ClearFolder(string.Format("{0}\\{1}\\{2}\\{3}", phisicalPath, EnumFileType.AnswerSheetLot.GetDescription(), DateTime.Today.Year, lotFolder), fileParentId, true);

						return true;
					}
					else
					{
						LogFacade.LogFacade.SaveError(error: fileDB != null ? fileDB.Validate.Message : "CreateFileRegister (salva o arquivo no banco) retornou null");
					}
				}
				else
				{
					LogFacade.LogFacade.SaveError(error: string.Format("Arquivo não existe {0}.", folderToSaveZip));
				}

				return false;
			}
			catch (Exception e)
			{
				LogFacade.LogFacade.SaveError(e, error: string.Format("Erro ao zipar os arquivos da folha de resposta para o lote {0}.", fileOwnerId));
				throw;
			}
		}

		private void GenerateQRCodes(List<AnswerSheetStudentInformation> students, string virtualDirectory, string physicalDirectory, long Id, int schoolId, long teamId, string folderName)
		{
			try
			{
				folderName = string.Concat(folderName, "\\QRCodes").Replace(".", string.Empty);

				foreach (var student in students)
				{
					string name = Guid.NewGuid() + ".png";
					string QRCodeId = "{";
					QRCodeId += string.Format(@" ""Test_Id"": {0}, ""School_Id"": {1}, ""Section_Id"": {2}, ""Student_Id"": {3} ", Id, schoolId, teamId, student.Id);
					QRCodeId += "}";

					student.RelativePath = answerSheetBusiness.RenderQrCode(QRCodeId, 5, virtualDirectory, physicalDirectory, name, folderName);
					if (!string.IsNullOrEmpty(student.RelativePath))
						student.StoragePath = string.Concat(physicalDirectory, new Uri(student.RelativePath).AbsolutePath.Replace("/", "\\"));
				}
			}
			catch (Exception e)
			{
				LogFacade.LogFacade.SaveError(e, error: string.Format("Erro ao gerar os QR codes da folha de resposta em lote para a prova {0}.", Id));
				throw;
			}
		}

		private void GenerateSectionPDF(List<AnswerSheetStudentInformation> students, AdherenceDTO section, int qtdItens, Booklet booklet, string virtualPath,
			string phisicalPath, string folderName, Entities.File logo)
		{
			try
			{
				Uri uri = new Uri(virtualPath);
				var parameters = parameterBusiness.GetAll();

				string cssFile = string.Format("http://{0}/Assets/css/prova.css", uri.Authority);
				string cssPdf = string.Format("<link href='{0}' rel='stylesheet' />", cssFile);
				StringBuilder html = new StringBuilder(string.Format("<html><head>{0}</head><body><div class='pdf-content'>", cssPdf));

				PDFFilter filter;
				foreach (var student in students)
				{
					filter = new PDFFilter
					{
						Booklet = booklet,
						Test = booklet.Test,
						FontSize = 20,
						UrlSite = uri.Authority,
						GenerateType = EnumGenerateType.AnswerSheet,
						Student = student,
						QtdeItens = qtdItens,
						IdentificationType = EnumIdentificationType.QRCode,
						Logo = logo,
						Parameters = parameters
					};

					html.Append("<div class='pageBreakInside'>");
					html.Append(generateHtmlBusiness.BuildAnswerSheet(filter));
					html.Append("</div>");
				}

				html.Append("</div></body></html>");

				Entities.File entityFile;
				string pdfName = string.Format("{0} - {1}", section.tur_codigo, StringHelper.RemoveSpecialCharactersWithRegex(section.ttn_nome, Constants.UnderlineChar.ToString()));
				storage.Save(htmltopdf.ConvertHtml(html.ToString(), section.dre_nome, (float)18.34, (float)18.34, 0, (float)14.34, 0, 0), string.Format("{0}.pdf", pdfName), "application/pdf", folderName, virtualPath, phisicalPath, out entityFile);
			}
			catch (Exception e)
			{
				LogFacade.LogFacade.SaveError(e, error: string.Format("Erro ao gerar os arquivos de turmas da folha de resposta em lote para a prova {0}.", booklet.Test_Id));
				throw;
			}
		}

		private void MergePDFFiles(string folderName, string esc_nome, string virtualPath, string phisicalPath, long Id)
		{
			try
			{
				var info = new DirectoryInfo(Path.Combine(phisicalPath, folderName));

				var files = info.GetFiles("*.pdf", SearchOption.TopDirectoryOnly).Select(f => f.FullName);

				var dre_folder = folderName.Substring(0, folderName.LastIndexOf('\\'));

				Entities.File entityFile;
				string pdfName = StringHelper.RemoveSpecialCharactersWithRegex(esc_nome, Constants.UnderlineChar.ToString());
				storage.Save(pDFMerger.Merge(files), string.Format("{0}.pdf", pdfName), "application/pdf", dre_folder, virtualPath, phisicalPath, out entityFile);
			}
			catch (Exception e)
			{
				LogFacade.LogFacade.SaveError(e, error: string.Format("Erro ao agrupar os arquivos de turmas da folha de resposta para o lote {0}.", Id));
				throw;
			}
		}

		private void ClearFolder(string Path, long Id, bool deleteItSelf = false)
		{
			try
			{
				var info = new DirectoryInfo(Path);

				if (info.Exists)
				{
					foreach (var item in info.GetDirectories())
						DeleteSubDirectories(item.FullName);

					if (deleteItSelf)
						info.Delete(true);
				}
			}
			catch (Exception e)
			{
				LogFacade.LogFacade.SaveError(e, error: string.Format("Erro ao limpar arquivos temporários da folha de resposta para o lote {0}.", Id));
				throw;
			}
		}

		private void DeleteSubDirectories(string path)
		{
			var info = new DirectoryInfo(path);

			foreach (var item in info.GetDirectories())
				DeleteSubDirectories(item.FullName);

			if (info.GetDirectories().Length == 0)
				info.Delete(true);
		}

		private void CreateFile(string folderToSaveZip, string folderToGetFiles)
		{
			try
			{
				FileInfo fileZip = new FileInfo(folderToSaveZip);

				if (fileZip.Exists)
					fileZip.Delete();

				ZipFileCreator.CreateZipFileFromDirectory(folderToSaveZip, folderToGetFiles);
			}
			catch (Exception e)
			{
				LogFacade.LogFacade.SaveError(e, error: string.Format("Erro ao criar o arquivo zip {0}.", folderToSaveZip));
				throw;
			}
		}

		private Entities.File CreateFileRegister(string testName, long ownerId, string virtualPath, long parentOwnerId, string nomeArquivo)
		{
			try
			{
				Entities.File file = fileBusiness.Save(new Entities.File()
				{
					ContentType = "application/zip",
					Name = nomeArquivo,
					OriginalName = testName,
					OwnerId = ownerId,
					OwnerType = (byte)EnumFileType.AnswerSheetLot,
					Path = virtualPath,
					ParentOwnerId = parentOwnerId,
				});

				return file;
			}
			catch (Exception e)
			{
				LogFacade.LogFacade.SaveError(e, error: string.Format("Erro ao criar o registro do arquivo da folha de resposta para o lote {0}.", ownerId));
				throw;
			}
		}
	}
}