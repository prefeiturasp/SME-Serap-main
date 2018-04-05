using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using MSTech.CoreSSO.Entities;
using OMRService;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Business
{
    public class AnswerSheetBatchFilesBusiness : IAnswerSheetBatchFilesBusiness
    {
        private readonly IAnswerSheetBatchFilesRepository batchRepository;
        private readonly IAnswerSheetBatchBusiness batchBusiness;
        private readonly ITestBusiness testBusiness;
        private readonly IItemTypeBusiness itemTypeBusiness;
        private readonly IESC_EscolaBusiness escolaBusiness;
        private readonly ITUR_TurmaBusiness turmaBusiness;
        private readonly IACA_AlunoBusiness alunoBusiness;
        private readonly IParameterBusiness parameterBusiness;
        private readonly IFileBusiness fileBusiness;
        private readonly ITestSectionStatusCorrectionBusiness testSectionStatusCorrectionBusiness;
        private readonly IStorage storage;
        private readonly IAnswerSheetBatchQueueBusiness answerSheetBatchQueueBusiness;
        private readonly IBookletBusiness bookletBusiness;
        private readonly IBlockBusiness blockBusiness;

        public AnswerSheetBatchFilesBusiness(IAnswerSheetBatchFilesRepository batchRepository, IAnswerSheetBatchBusiness batchBusiness,
            ITestBusiness testBusiness, IItemTypeBusiness itemTypeBusiness, IESC_EscolaBusiness escolaBusiness, ITUR_TurmaBusiness turmaBusiness,
            IParameterBusiness parameterBusiness, IFileBusiness fileBusiness, ITestSectionStatusCorrectionBusiness testSectionStatusCorrectionBusiness, IACA_AlunoBusiness alunoBusiness, IStorage storage,
            IAnswerSheetBatchQueueBusiness answerSheetBatchQueueBusiness, IBookletBusiness bookletBusiness, IBlockBusiness blockBusiness)
        {
            this.batchRepository = batchRepository;
            this.batchBusiness = batchBusiness;
            this.testBusiness = testBusiness;
            this.itemTypeBusiness = itemTypeBusiness;
            this.escolaBusiness = escolaBusiness;
            this.turmaBusiness = turmaBusiness;
            this.parameterBusiness = parameterBusiness;
            this.fileBusiness = fileBusiness;
            this.testSectionStatusCorrectionBusiness = testSectionStatusCorrectionBusiness;
            this.alunoBusiness = alunoBusiness;
            this.storage = storage;
            this.answerSheetBatchQueueBusiness = answerSheetBatchQueueBusiness;
            this.bookletBusiness = bookletBusiness;
            this.blockBusiness = blockBusiness;
        }

        #region Read

        public IEnumerable<AnswerSheetBatchResult> SearchBatchFiles(ref Pager pager, AnswerSheetBatchFilter filter, string nomeAluno, string turma)
        {
            return batchRepository.SearchBatchFiles(ref pager, filter, nomeAluno, turma);
        }

        public AnswerSheetBatchFileCountResult GetCountBatchInformation(AnswerSheetBatchFilter filter)
        {
            return batchRepository.GetCountBatchInformation(filter);
        }

        public IEnumerable<AnswerSheetBatchFileResult> GetBatchFiles(long batchId, bool sent, int rows)
        {
            return batchRepository.GetBatchFiles(batchId, sent, rows);
        }

        public IEnumerable<AnswerSheetBatchFileResult> GetBatchFiles(EnumBatchSituation situation, int rows)
        {
            return batchRepository.GetBatchFiles(situation, rows);
        }

        public IEnumerable<AnswerSheetBatchFiles> GetFiles(long batchId, bool excludeErrorFiles)
        {
            return batchRepository.GetFiles(batchId, excludeErrorFiles);
        }

        public AnswerSheetBatchFiles GetFile(long Id, long fileId)
        {
            return batchRepository.GetFile(Id, fileId);
        }

        public EntityFile GetStudentFile(long testId, long studentId, long sectionId, string physicalDirectory, string virtualDirectory, SYS_Usuario usuarioLogado)
        {
            EntityFile retorno = new EntityFile();
            retorno.Validate.IsValid = false;

            var pBatchFile = parameterBusiness.GetParamByKey(EnumParameterKey.DELETE_BATCH_FILES.GetDescription(), usuarioLogado.ent_id);
            bool batchFile = pBatchFile != null && Convert.ToBoolean(pBatchFile.Value);

            if (!batchFile)
            {
                AnswerSheetBatchFileResult studentFile = GetStudentFile(testId, studentId, sectionId);
                if (studentFile != null)
                {
                    var pdownloadOMRFile = parameterBusiness.GetParamByKey(EnumParameterKey.DOWNLOAD_OMR_FILE.GetDescription(), usuarioLogado.ent_id);
                    bool downloadOMRFile = pdownloadOMRFile != null && Convert.ToBoolean(pdownloadOMRFile.Value);

                    if (downloadOMRFile)
                    {
                        retorno.Id = 0;
                        retorno.Path = AuthenticationService.GetUri() + "api/file/equalized?externalId=" + studentFile.Id;
                        retorno.OriginalName = string.Format("gabarito{0}{1}{2}.png", studentId, sectionId, studentFile.AnswerSheetBatch_Id);
                        retorno.Validate.IsValid = true;
                    }
                    else
                    {
                        retorno.Id = studentFile.FileId;
                        retorno.Path = studentFile.FilePath;
                        retorno.OriginalName = studentFile.FileOriginalName;
                        retorno.Validate.IsValid = true;
                    }
                }
            }

            return retorno;
        }

        public AnswerSheetBatchFiles Get(long Id)
        {
            return batchRepository.Get(Id);
        }

        public int GetFilesCount(long batchId)
        {
            return batchRepository.GetFilesCount(batchId);
        }

        public int GetFilesNotSentCount(long batchId)
        {
            return batchRepository.GetFilesNotSentCount(batchId);
        }

        public IEnumerable<AnswerSheetFollowUpIdentificationResult> GetIdentificationList(AnswerSheetBatchFilter filter)
        {
            Pager pager = null;
            return batchRepository.GetIdentificationList(ref pager, filter);
        }

        public IEnumerable<AnswerSheetFollowUpIdentificationResult> GetIdentificationList(ref Pager pager, AnswerSheetBatchFilter filter)
        {
            return batchRepository.GetIdentificationList(ref pager, filter);
        }

        public IEnumerable<AnswerSheetFollowUpIdentificationResult> GetIdentificationFilesList(AnswerSheetBatchFilter filter)
        {
            Pager pager = null;
            return batchRepository.GetIdentificationFilesList(ref pager, filter);
        }

        public IEnumerable<AnswerSheetFollowUpIdentificationResult> GetIdentificationFilesList(ref Pager pager, AnswerSheetBatchFilter filter)
        {
            var situation = filter.Processing;
            if (!string.IsNullOrEmpty(situation))
            {
                filter.Processing = string.Empty;
                if (situation.Contains(Convert.ToString((byte)EnumFollowUpIdentificationDataType.Identified)))
                {
                    var listSituationIdentified = new string[] { Convert.ToString((byte)EnumBatchSituation.Pending), Convert.ToString((byte)EnumBatchSituation.Success), Convert.ToString((byte)EnumBatchSituation.Error), Convert.ToString((byte)EnumBatchSituation.Warning) };
                    filter.Processing = string.Join(",", listSituationIdentified);
                }

                if (situation.Contains(Convert.ToString((byte)EnumFollowUpIdentificationDataType.NotIdentified)))
                {
                    filter.Processing += !string.IsNullOrEmpty(filter.Processing) ? "," + Convert.ToString((byte)EnumBatchSituation.NotIdentified) : Convert.ToString((byte)EnumBatchSituation.NotIdentified);
                }

                if (situation.Contains(Convert.ToString((byte)EnumFollowUpIdentificationDataType.Pending)))
                {
                    filter.Processing += !string.IsNullOrEmpty(filter.Processing) ? "," + Convert.ToString((byte)EnumBatchSituation.PendingIdentification) : Convert.ToString((byte)EnumBatchSituation.PendingIdentification);
                }
            }

            return batchRepository.GetIdentificationFilesList(ref pager, filter);
        }

        public AnswerSheetFollowUpIdentification GetIdentificationReportInfo(AnswerSheetBatchFilter filter)
        {
            return batchRepository.GetIdentificationReportInfo(filter);
        }

        #endregion

        #region Write

        public void SaveList(List<AnswerSheetBatchFiles> list)
        {
            batchRepository.SaveList(list);
        }

        public void UpdateBatchFilesIdentified(IEnumerable<AnswerSheetBatch> answerSheetBatchList)
        {
            foreach (var answerSheetBatch in answerSheetBatchList)
            {
                answerSheetBatch.Validate = ValidateAnswerSheetBatch(answerSheetBatch, answerSheetBatch.Validate);
                if (answerSheetBatch.Validate.IsValid)
                {
                    var batch = FindBatchByTest(answerSheetBatch.Test_Id);
                    UpdateSituationToPending(answerSheetBatch.AnswerSheetBatchFiles, EnumBatchSituation.Pending, batch.Id);
                    ProcessBatch(batch, answerSheetBatch.Test_Id);
                }
                else
                {
                    UpdateSituationToPending(answerSheetBatch.AnswerSheetBatchFiles, EnumBatchSituation.NotIdentified);
                }
            }
        }

        public AnswerSheetBatchFiles Update(AnswerSheetBatchFiles entity)
        {
            entity.Validate = ValidateUpdate(entity, entity.Validate);
            if (entity.Validate.IsValid)
            {
                if (entity.Section_Id != null && entity.Section_Id > 0)
                {
                    TUR_Turma turma = turmaBusiness.Get((long)entity.Section_Id);
                    if (turma != null)
                    {
                        entity.School_Id = turma.esc_id;

                        ESC_Escola escola = escolaBusiness.Get(turma.esc_id);
                        if (escola != null && escola.uad_idSuperiorGestao.HasValue)
                        {
                            entity.SupAdmUnit_Id = escola.uad_idSuperiorGestao.Value;
                        }
                    }
                }

                batchRepository.Update(entity);
                entity.Validate.Type = ValidateType.Update.ToString();
                entity.Validate.Message = "Arquivo do lote alterado com sucesso.";
            }

            return entity;
        }

        public void UpdateList(List<AnswerSheetBatchFiles> list)
        {
            batchRepository.UpdateList(list);
        }

        public void UpdateSentList(List<AnswerSheetBatchFiles> list)
        {
            batchRepository.UpdateSentList(list);
        }

        public void DeleteList(List<AnswerSheetBatchFiles> list)
        {
            batchRepository.DeleteList(list);
        }

        public AnswerSheetBatch SendToProcessing(AnswerSheetBatchFilter filter)
        {
            var retorno = new AnswerSheetBatch();

            if (filter.TestId > 0)
            {
                var test = testBusiness.GetObjectWithTestTypeItemType(filter.TestId);

                List<AnswerSheetBatchItems> answers = testBusiness.GetTestAnswers(filter.TestId);
                AnswerSheetBatch batch = batchBusiness.GetSimple(filter.BatchId);
                test.Validate = ValidateProcessing(test, filter.BatchId, answers, test.Validate, batch);

                if (test.Validate.IsValid)
                {
                    if (batch != null)
                    {
                        List<AnswerSheetBatchItems> groupedItemsList = answers.GroupBy(i => new { i.Item_Id, i.KnowledgeArea_Id }).Select(grp => new AnswerSheetBatchItems { Item_Id = grp.Key.Item_Id, KnowledgeArea_Id = grp.Key.KnowledgeArea_Id }).ToList();
                        List<AnswerSheetBatchItems> listItems = new List<AnswerSheetBatchItems>();

                        bool QRCode = batch.BatchType.Equals(EnumAnswerSheetBatchType.QRCode);
                        int columns = 0, questions = 0;
                        if (test.TestType.ItemType_Id.HasValue)
                        {
                            int numberItem = groupedItemsList.Count;

                            // Caso a prova seja separada em blocos de área de conhecimento.
                            if (test.KnowledgeAreaBlock)
                            {
                                var knowledgeAreaItens = answers.Select(p => new { p.KnowledgeArea_Id, p.KnowledgeArea_Description, p.Order }).OrderBy(q => q.Order).Select(r => new { r.KnowledgeArea_Id, r.KnowledgeArea_Description }).Distinct();

                                numberItem += knowledgeAreaItens.Count();

                                int order = 0;
                                foreach (var knowledgeArea in knowledgeAreaItens)
                                {
                                    var templateKnowledgeArea = new AnswerSheetBatchItems
                                    {
                                        KnowledgeArea_Id = knowledgeArea.KnowledgeArea_Id,
                                        KnowledgeArea_Description = knowledgeArea.KnowledgeArea_Description,
                                        Order = order,
                                        Ignore = true
                                    };
                                    listItems.Add(templateKnowledgeArea);
                                    order++;

                                    var itensKnowledgeArea = groupedItemsList.Where(p => p.KnowledgeArea_Id == knowledgeArea.KnowledgeArea_Id);
                                    itensKnowledgeArea.Select(c => { c.Order = order; c.Ignore = false; order++; return c; }).ToList();

                                    listItems.AddRange(itensKnowledgeArea);
                                }
                            }
                            else
                            {
                                listItems = groupedItemsList;
                            }

                            test.TestType.ItemType = itemTypeBusiness.Get(test.TestType.ItemType_Id.Value);
                            if (numberItem <= 20) { columns = 1; questions = 20; }
                            else if (numberItem <= 40) { columns = 2; questions = 40; }
                            else if (numberItem <= 60) { columns = 3; questions = 60; }
                            else if (numberItem <= 80) { columns = 4; questions = 80; }
                            else if (numberItem <= 100) { columns = 5; questions = 100; }
                        }
                        StringBuilder OMRBody = new StringBuilder("{");
                        OMRBody.AppendFormat("\"externalId\": \"{0}\", ", filter.BatchId);
                        if (batch.OwnerEntity.Equals(EnumAnswerSheetBatchOwner.Test))
                            OMRBody.Append("\"paginate\": 1 , ");

                        OMRBody.Append("\"template\": { ");
                        OMRBody.AppendFormat("\"externalId\": \"{0}\", ", filter.TestId);
                        OMRBody.AppendFormat("\"questions\": {0}, ", questions);
                        OMRBody.AppendFormat("\"alternatives\": {0}, ", test.TestType.ItemType.QuantityAlternative);
                        OMRBody.AppendFormat("\"columns\": {0}, ", columns);

                        StringBuilder sb = new StringBuilder();
                        foreach (AnswerSheetBatchItems item in listItems)
                        {
                            List<AnswerSheetBatchItems> alternatives = answers.Where(i => i.Item_Id.Equals(item.Item_Id)).ToList();
                            long correctAlternative = alternatives.Where(i => i.Correct).Select(i => i.Id).FirstOrDefault();
                            sb.Append(@" { ""externalId"": """ + item.Item_Id + @""",  ""correctId"": """ + correctAlternative + @""",  ""ignore"": " + (item.Ignore ? "true" : "false"));
                            sb.Append(@", ""answers"": [ " + String.Join(",", alternatives.Select(i => i.Id)) + @" ]");
                            sb.Append("},");
                        }
                        string items = sb.ToString().TrimEnd(','); 

                        OMRBody.AppendFormat("\"items\": [ {0} ], ", items);

                        OMRBody.Append("\"lines\": 1 ");
                        if (QRCode)
                            OMRBody.Append(", \"qrCode\": 1 ");
                        OMRBody.Append("} ");

                        if (!batch.OwnerEntity.Equals(EnumAnswerSheetBatchOwner.Test))
                        {
                            OMRBody.Append(", \"group\": [ ");

                            if (filter.SchoolId != null && filter.SchoolId > 0)
                            {
                                OMRBody.Append("{ ");
                                OMRBody.AppendFormat("\"externalId\": \"{0}\", ", filter.SchoolId);
                                OMRBody.Append("\"type\": 0, ");
                                OMRBody.AppendFormat("\"name\": \"{0}\" ", escolaBusiness.Get((int)filter.SchoolId).esc_nome);
                                OMRBody.Append("} ");
                            }

                            if (filter.SectionId != null && filter.SectionId > 0)
                            {
                                OMRBody.Append(",{ ");
                                OMRBody.AppendFormat("\"externalId\": \"{0}\", ", filter.SectionId);
                                OMRBody.Append("\"type\": 1, ");
                                OMRBody.AppendFormat("\"name\": \"{0}\" ", turmaBusiness.Get((long)filter.SectionId).tur_codigo);
                                OMRBody.Append("} ");
                            }

                            OMRBody.Append("] ");
                        }

                        OMRBody.Append("} ");

                        BatchService OMRService = new BatchService();
                        var result = OMRService.CreateBatch(OMRBody.ToString());
                        if (result != null && result.isValid())
                        {
                            test.Validate.IsValid = true;

                            test.Validate.Message = "Lote enviado para processamento com sucesso.";

                            if (batch != null && batch.Id > 0)
                            {
                                //Atualiza situação do lote para Iniciado
                                if (!batch.Processing.Equals(EnumBatchProcessing.Initiate) && !batch.Processing.Equals(EnumBatchProcessing.Processing))
                                {
                                    batch.Processing = EnumBatchProcessing.Initiate;
                                    batchBusiness.Update(batch.Id, batch);
                                }

                                if (batch.OwnerEntity.Equals(EnumAnswerSheetBatchOwner.Section))
                                {
                                    TestSectionStatusCorrection statusCorrection = testSectionStatusCorrectionBusiness.Get(batch.Test_Id, (long)batch.Section_Id);
                                    if (statusCorrection != null && statusCorrection.Id > 0)
                                    {
                                        statusCorrection.StatusCorrection = EnumStatusCorrection.ProcessingSection;
                                        testSectionStatusCorrectionBusiness.Update(statusCorrection);
                                    }
                                    else
                                    {
                                        statusCorrection = new TestSectionStatusCorrection
                                        {
                                            Test_Id = batch.Test_Id,
                                            tur_id = (long)batch.Section_Id,
                                            StatusCorrection = EnumStatusCorrection.ProcessingSection
                                        };

                                        testSectionStatusCorrectionBusiness.Save(statusCorrection);
                                    }
                                }
                            }
                        }
                        else
                        {
                            test.Validate.IsValid = false;
                            test.Validate.Type = ValidateType.error.ToString();
                            test.Validate.Message = result != null ? result.Message : "Erro ao enviar lote para processamento.";
                        }
                    }
                    else
                    {
                        test.Validate.IsValid = false;
                        test.Validate.Type = ValidateType.error.ToString();
                        test.Validate.Message = "Erro ao enviar lote para processamento.";
                    }
                }
                retorno.Validate = test.Validate;
            }
            return retorno;
        }

        public AnswerSheetBatch SaveBatch(AnswerSheetBatch entity, string virtualDirectory, string physicalDirectory, SYS_Usuario usuarioLogado)
        {
            AnswerSheetBatchFiles f = entity.AnswerSheetBatchFiles.FirstOrDefault();

            ConcurrentBag<AnswerSheetBatchFiles> zipFiles = new ConcurrentBag<AnswerSheetBatchFiles>();
            if (f != null)
            {
                EntityFile file = fileBusiness.Get(f.File_Id);

                #region Extract ZIP

                var paramZipFiles = parameterBusiness.GetParamByKey(EnumParameterKey.ZIP_FILES.GetDescription(), usuarioLogado.ent_id);
                var zipFilesAllowed = paramZipFiles != null ? paramZipFiles.Value.TrimEnd(Constants.StringArraySeparator).Split(Constants.StringArraySeparator) : null;

                if (file != null && !string.IsNullOrEmpty(file.ContentType) && StringHelper.ValidateValuesAllowed(zipFilesAllowed, file.ContentType))
                {
                    AnswerSheetBatchQueue answerSheetBatchQueue = new AnswerSheetBatchQueue
                    {
                        File_Id = file.Id,
                        School_Id = entity.School_Id,
                        SupAdmUnit_Id = entity.SupAdmUnit_Id,
                        Situation = EnumBatchQueueSituation.PendingUnzip,
                        CreatedBy_Id = usuarioLogado.usu_id,
                        EntityId = usuarioLogado.ent_id
                    };

                    if ((entity.Test_Id > 0) && (entity.Id <= 0))
                    {
                        entity.OwnerEntity = EnumAnswerSheetBatchOwner.Test;
                        entity.BatchType = EnumAnswerSheetBatchType.QRCode;
                        entity.Processing = EnumBatchProcessing.Pending;

                        entity = CreateBatch(entity, usuarioLogado.usu_id);
                    }
                    if (entity.Id > 0)
                        answerSheetBatchQueue.AnswerSheetBatch_Id = entity.Id;

                    AnswerSheetBatchQueue AnswerSheetBatchQueue = answerSheetBatchQueueBusiness.Save(answerSheetBatchQueue);
                    entity.Validate = AnswerSheetBatchQueue.Validate;
                    entity.AnswerSheetBatchFiles = null;
                }

                #endregion
            }

            #region Batch

            if (zipFiles != null && zipFiles.Count > 0)
            {
                entity.AnswerSheetBatchFiles = zipFiles.ToList();
            }

            if (entity.Test_Id > 0)
            {
                if (entity.AnswerSheetBatchFiles != null && entity.AnswerSheetBatchFiles.Count > 0)
                {
                    entity.AnswerSheetBatchFiles.ForEach(i =>
                    {
                        i.AnswerSheetBatch_Id = entity.Id;
                        i.AnswerSheetBatch = null;
                        i.Sent = false;
                        i.Situation = EnumBatchSituation.Pending;
                    });

                    //Se não tiver a turma, é porque é lote da escola (qrcode), se tiver a turma é lote da turma (identificação por nº de chamada)
                    if (entity.Section_Id != null && entity.Section_Id > 0)
                        entity.OwnerEntity = EnumAnswerSheetBatchOwner.Section;
                    else if (entity.School_Id != null && entity.School_Id > 0)
                    {
                        entity.OwnerEntity = EnumAnswerSheetBatchOwner.School;
                        entity.BatchType = EnumAnswerSheetBatchType.QRCode;
                    }
                    else if (entity.Test_Id > 0)
                    {
                        entity.OwnerEntity = EnumAnswerSheetBatchOwner.Test;
                        entity.BatchType = EnumAnswerSheetBatchType.QRCode;
                    }

                    entity.Processing = EnumBatchProcessing.Pending;

                    if (entity.Id > 0)
                    {
                        entity = batchBusiness.Update(entity.Id, entity);
                        SaveList(entity.AnswerSheetBatchFiles);
                    }
                    else
                    {
                        entity = CreateBatch(entity, usuarioLogado.usu_id);
                    }
                }
            }
            else
            {
                if (entity.AnswerSheetBatchFiles != null && entity.AnswerSheetBatchFiles.Count > 0)
                {
                    foreach (var bachFile in entity.AnswerSheetBatchFiles)
                    {
                        bachFile.CreatedBy_Id = usuarioLogado.usu_id;
                    }
                    entity.Validate = SavePendingIdentification(entity.AnswerSheetBatchFiles, 0, entity.Id, entity.School_Id, entity.SupAdmUnit_Id);
                }

            }

            if ((entity.Validate.IsValid) && (entity.AnswerSheetBatchFiles != null))
            {
                AssociateFilesToEntity(entity.AnswerSheetBatchFiles, entity.Id);
            }

            #endregion

            if (!entity.Validate.IsValid && f != null)
            {
                fileBusiness.LogicalDelete(f.File_Id, usuarioLogado.usu_id, 0);

                foreach (AnswerSheetBatchFiles file in entity.AnswerSheetBatchFiles)
                    fileBusiness.LogicalDelete(file.File_Id, usuarioLogado.usu_id, 0);
            }

            return entity;
        }

        public EntityFile ExportAnswerSheetData(AnswerSheetBatchFilter filter, string separator, string virtualDirectory, string physicalDirectory)
        {
            EntityFile ret = new EntityFile();

            // Metodo GetCountBatchInformation altera o valor do filter.Processing que depois precisa ser usado no metodo SearchBatchFiles
            // por isso a auxProcessing armazena o valor do filter.Processing
            var auxProcessing = filter.Processing;

            StringBuilder stringBuilder = new StringBuilder();

            AnswerSheetBatch batch = null;
            if (filter.TestId > 0)
            {
                batch = batchBusiness.Find(filter);
                stringBuilder.Append(string.Format("Lote{0} Data de criação{0} Situação{0}", separator));
            }

            AnswerSheetBatchFileCountResult batchInfo = GetCountBatchInformation(filter);

            stringBuilder.Append(string.Format("Total{0} Na fila para identificação{0} Erro na identificação{0} Aguardando correção{0} Erro{0} Conferir{0} Sucesso{0}", separator));
            stringBuilder.AppendLine();

            if (filter.TestId > 0 && batch != null)
            {
                filter.BatchId = batch.Id;

                stringBuilder.Append(batch.Description + separator);
                stringBuilder.Append(batch.CreateDate.ToShortDateString() + separator);
                stringBuilder.Append(EnumHelper.GetDescriptionFromEnumValue(batch.Processing) + separator);
            }

            stringBuilder.Append(batchInfo.Total + separator);
            stringBuilder.Append(batchInfo.PendingIdentification + separator);
            stringBuilder.Append(batchInfo.NotIdentified + separator);
            stringBuilder.Append(batchInfo.Pending + separator);
            stringBuilder.Append(batchInfo.Errors + separator);
            stringBuilder.Append(batchInfo.Warnings + separator);
            stringBuilder.Append(batchInfo.Success + separator);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(string.Format("Arquivo{0} DRE{0} Escola{0} Turma{0} Aluno{0} Data de upload{0} Processamento{0}", separator));

            Pager pager = null;
            filter.Processing = auxProcessing;
            IEnumerable<AnswerSheetBatchResult> result = SearchBatchFiles(ref pager, filter, null, null);

            foreach (AnswerSheetBatchResult entity in result)
            {
                EnumBatchSituation situation = entity.Situation != null ? (EnumBatchSituation)entity.Situation : (EnumBatchSituation)entity.Processing;

                stringBuilder.Append(entity.FileName + separator);
                stringBuilder.Append(entity.SupAdmUnitName + separator);
                stringBuilder.Append(entity.SchoolName + separator);
                stringBuilder.Append(entity.SectionName + separator);
                stringBuilder.Append(entity.StudentName + separator);
                stringBuilder.Append(entity.CreateDate.ToShortDateString() + separator);
                stringBuilder.Append(EnumHelper.GetDescriptionFromEnumValue(situation) + separator);
                stringBuilder.AppendLine();
            }

            var fileContent = stringBuilder.ToString();

            if (!string.IsNullOrEmpty(fileContent))
            {
                string fileName = batch != null ? "Arquivos_" + batch.Description
                    : filter.TestId > 0 ? string.Format("Arquivos_{0}_{1}", filter.TestId, filter.SchoolId)
                    : "Arquivos_Lote_Geral";
                if (batch == null && filter.SectionId > 0)
                    fileName += "_" + filter.SectionId;

                byte[] buffer = System.Text.Encoding.Default.GetBytes(fileContent);
                string originalName = string.Format("{0}.csv", fileName);
                string name = string.Format("{0}.csv", Guid.NewGuid());
                string contentType = MimeType.CSV.GetDescription();

                var csvFiles = fileBusiness.GetAllFilesByType(EnumFileType.ExportAnswerSheetResult, DateTime.Now.AddDays(-1));
                if (csvFiles != null && csvFiles.Count() > 0)
                {
                    fileBusiness.DeletePhysicalFiles(csvFiles.ToList(), physicalDirectory);
                    fileBusiness.DeleteFilesByType(EnumFileType.ExportAnswerSheetResult, DateTime.Now.AddDays(-1));
                }

                ret = storage.Save(buffer, name, contentType, EnumFileType.ExportAnswerSheetResult.GetDescription(), virtualDirectory, physicalDirectory, out ret);
                if (ret.Validate.IsValid)
                {
                    ret.ContentType = contentType;
                    ret.OriginalName = StringHelper.Normalize(originalName);
                    ret.OwnerId = filter.BatchId;
                    ret.ParentOwnerId = filter.TestId;
                    ret.OwnerType = (byte)EnumFileType.ExportAnswerSheetResult;
                    ret = fileBusiness.Save(ret);
                }
            }
            else
            {
                ret.Validate.IsValid = false;
                ret.Validate.Type = ValidateType.alert.ToString();
                ret.Validate.Message = "Dados dos arquivos ainda não foram gerados.";
            }

            return ret;
        }

        public EntityFile ExportFollowUpIdentification(AnswerSheetBatchFilter filter, string separator, string virtualDirectory, string physicalDirectory)
        {
            EntityFile ret = new EntityFile();
            StringBuilder stringBuilder = new StringBuilder();

            switch (filter.View)
            {
                case EnumFollowUpIdentificationView.DRE:
                    stringBuilder.Append(string.Format("DRE{0} Qtde. de arquivos enviados{0} Na fila para identificação{0} Qtde. de arquivos identificados{0} Qtde. de arquivos não identificados (erro){0}", separator));
                    break;
                case EnumFollowUpIdentificationView.School:
                    stringBuilder.Append(string.Format("Escola{0} Qtde. de arquivos enviados{0} Na fila para identificação{0} Qtde. de arquivos identificados{0} Qtde. de arquivos não identificados (erro){0}", separator));
                    break;
                case EnumFollowUpIdentificationView.Files:
                    stringBuilder.Append(string.Format("Arquivo{0} Data de {1}{0} Situação{0}", separator, (filter.FilterDateUpdate ? "correção" : "envio")));
                    break;
                default: break;
            }

            stringBuilder.AppendLine();

            Pager pager = null;
            var view = filter.View;

            filter.View = EnumFollowUpIdentificationView.Total;
            var TotalFiles = filter.SupAdmUnitId == null && view == EnumFollowUpIdentificationView.DRE ? GetIdentificationList(filter).FirstOrDefault() : null;

            filter.View = view;
            IEnumerable<AnswerSheetFollowUpIdentificationResult> result = view == EnumFollowUpIdentificationView.Files ? GetIdentificationFilesList(ref pager, filter) : GetIdentificationList(ref pager, filter);

            if (result != null && result.Count() > 0)
            {
                if (TotalFiles != null)
                {
                    stringBuilder.Append(TotalFiles.Name + separator);
                    stringBuilder.Append(TotalFiles.TotalSent + separator);
                    stringBuilder.Append(TotalFiles.TotalPendingIdentification + separator);
                    stringBuilder.Append(TotalFiles.TotalIdentified + separator);
                    stringBuilder.Append(TotalFiles.TotalNotIdentified + separator);
                    stringBuilder.AppendLine();
                }

                foreach (AnswerSheetFollowUpIdentificationResult entity in result)
                {
                    switch (filter.View)
                    {
                        case EnumFollowUpIdentificationView.DRE:
                        case EnumFollowUpIdentificationView.School:
                            stringBuilder.Append(entity.Name + separator);
                            stringBuilder.Append(entity.TotalSent + separator);
                            stringBuilder.Append(entity.TotalPendingIdentification + separator);
                            stringBuilder.Append(entity.TotalIdentified + separator);
                            stringBuilder.Append(entity.TotalNotIdentified + separator);
                            break;
                        case EnumFollowUpIdentificationView.Files:
                            stringBuilder.Append(entity.Name + separator);
                            stringBuilder.Append(entity.CreateDate.Value.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR")) + separator);
                            switch (entity.Situation)
                            {
                                case EnumBatchSituation.NotIdentified:
                                    stringBuilder.Append(EnumBatchSituation.NotIdentified.GetDescription() + separator); break;
                                case EnumBatchSituation.PendingIdentification:
                                    stringBuilder.Append(EnumBatchSituation.PendingIdentification.GetDescription() + separator); break;
                                case EnumBatchSituation.Success:
                                case EnumBatchSituation.Error:
                                case EnumBatchSituation.Warning:
                                case EnumBatchSituation.Pending:
                                    stringBuilder.Append("Identificado" + separator); break;
                            }
                            break;
                        default: break;
                    }

                    stringBuilder.AppendLine();
                }

                var fileContent = stringBuilder.ToString();
                if (!string.IsNullOrEmpty(fileContent))
                {
                    AnswerSheetFollowUpIdentification entity = GetIdentificationReportInfo(filter);

                    string fileName = string.Empty;
                    switch (filter.View)
                    {
                        case EnumFollowUpIdentificationView.DRE:
                            fileName = "Rel_Folhas_respostas_por_DRE";
                            if (entity != null && !string.IsNullOrEmpty(entity.SupAdmUnitInitials))
                                fileName += "_" + entity.SupAdmUnitInitials;
                            break;
                        case EnumFollowUpIdentificationView.School:
                            fileName = "Rel_folhas_respostas_por_Escola";
                            if (entity != null && entity.SchoolId != null)
                                fileName += "_" + entity.SchoolId;
                            break;
                        case EnumFollowUpIdentificationView.Files:
                            fileName = "Rel_folhas_respostas_por_Arquivo";
                            if (entity != null && !string.IsNullOrEmpty(entity.SupAdmUnitInitials) && entity.SchoolId != null)
                                fileName += "_" + entity.SupAdmUnitInitials + "_" + entity.SchoolId;
                            break;
                        default: break;
                    }

                    byte[] buffer = System.Text.Encoding.Default.GetBytes(fileContent);
                    string originalName = string.Format("{0}.csv", fileName);
                    string name = string.Format("{0}.csv", Guid.NewGuid());
                    string contentType = MimeType.CSV.GetDescription();

                    var csvFiles = fileBusiness.GetAllFilesByType(EnumFileType.ExportFollowUpIdentification, DateTime.Now.AddDays(-1));
                    if (csvFiles != null && csvFiles.Count() > 0)
                    {
                        fileBusiness.DeletePhysicalFiles(csvFiles.ToList(), physicalDirectory);
                        fileBusiness.DeleteFilesByType(EnumFileType.ExportFollowUpIdentification, DateTime.Now.AddDays(-1));
                    }

                    ret = storage.Save(buffer, name, contentType, EnumFileType.ExportFollowUpIdentification.GetDescription(), virtualDirectory, physicalDirectory, out ret);
                    if (ret.Validate.IsValid)
                    {
                        ret.Name = name;
                        ret.ContentType = contentType;
                        ret.OriginalName = StringHelper.Normalize(originalName);
                        ret.OwnerId = 0;
                        ret.ParentOwnerId = 0;
                        ret.OwnerType = (byte)EnumFileType.ExportFollowUpIdentification;
                        ret = fileBusiness.Save(ret);
                    }
                }
                else
                {
                    ret.Validate.IsValid = false;
                    ret.Validate.Type = ValidateType.alert.ToString();
                    ret.Validate.Message = "Dados dos arquivos ainda não foram gerados.";
                }
            }
            else
            {
                ret.Validate.IsValid = false;
                ret.Validate.Type = ValidateType.alert.ToString();
                ret.Validate.Message = "Ainda não existem dados para exportar.";
            }

            return ret;
        }

        public EntityFile ZipFollowUpIdentification(AnswerSheetBatchFilter filter, string virtualDirectory, string physicalDirectory)
        {
            EntityFile ret = new EntityFile();

            var files = GetIdentificationFilesList(filter);

            if (files != null && files.Count() > 0)
            {
                IEnumerable<ZipFileInfo> fileNames = files.Select(f => new ZipFileInfo
                {
                    Path = string.Concat(physicalDirectory, new Uri(f.FilePath).AbsolutePath.Replace("Files/", string.Empty).Replace("/", "\\")),
                    Name = f.Name
                });

                var filenNotExists = fileNames.Where(i => !System.IO.File.Exists(HttpUtility.UrlDecode(i.Path)));
                if (filenNotExists != null && filenNotExists.Any())
                {
                    ret.Validate.Type = ValidateType.alert.ToString();
                    ret.Validate.IsValid = false;
                    ret.Validate.Message = "Arquivo(s) não encontrado(s).";
                }
                else
                {
                    string originalName = string.Format("Folhas_respostas_por_Arquivo_{0}{1}.zip", DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HHmmss"));
                    originalName = Regex.Replace(originalName, @"[^\w\.@-]", "_");
                    string zipName = Guid.NewGuid() + ".zip";

                    var zipFiles = fileBusiness.GetAllFilesByType(EnumFileType.ZipFollowUpIdentification, DateTime.Now.AddDays(-1));
                    if (zipFiles != null && zipFiles.Count() > 0)
                    {
                        fileBusiness.DeletePhysicalFiles(zipFiles.ToList(), physicalDirectory);
                        fileBusiness.DeleteFilesByType(EnumFileType.ZipFollowUpIdentification, DateTime.Now.AddDays(-1));
                    }

                    ret = fileBusiness.SaveZip(zipName, EnumFileType.ZipFollowUpIdentification.GetDescription(), fileNames, physicalDirectory);
                    if (ret.Validate.IsValid)
                    {
                        ret.Path = Path.Combine(virtualDirectory, EnumFileType.ZipFollowUpIdentification.GetDescription(), zipName).Replace('\\', '/');
                        ret.Name = zipName;
                        ret.ContentType = MimeType.ZIP.GetDescription();
                        ret.OriginalName = StringHelper.Normalize(originalName);
                        ret.OwnerId = 0;
                        ret.ParentOwnerId = (long)filter.SchoolId;
                        ret.OwnerType = (byte)EnumFileType.ZipFollowUpIdentification;
                        ret = fileBusiness.Save(ret);
                    }
                }
            }

            return ret;
        }

        public void AssociateFilesToEntity(List<AnswerSheetBatchFiles> answerSheetBatchFiles, long answerSheetBatch_Id)
        {
            List<EntityFile> files = answerSheetBatchFiles.Select(i => new EntityFile { Id = i.File_Id, OwnerId = i.Id, ParentOwnerId = answerSheetBatch_Id }).ToList();
            fileBusiness.AssociateFilesToEntity(0, files);
        }

        public Validate SavePendingIdentification(List<AnswerSheetBatchFiles> answerSheetBatchFiles, long AnswerSheetBatchQueue_Id, long? AnswerSheetBatch_Id, int? School_Id, Guid? SupAdmUnit_Id)
        {
            Validate valid = new Validate();
            foreach (var bachFile in answerSheetBatchFiles)
            {
                if (School_Id > 0)
                    bachFile.School_Id = School_Id;
                if (SupAdmUnit_Id != null)
                    bachFile.SupAdmUnit_Id = SupAdmUnit_Id;
                if (AnswerSheetBatchQueue_Id > 0)
                    bachFile.AnswerSheetBatchQueue_Id = AnswerSheetBatchQueue_Id;
                if (AnswerSheetBatch_Id > 0)
                {
                    bachFile.AnswerSheetBatch_Id = AnswerSheetBatch_Id;
                    bachFile.Situation = EnumBatchSituation.Pending;
                }
                else
                {
                    bachFile.Situation = EnumBatchSituation.PendingIdentification;
                }

                bachFile.Sent = false;

            }
            SaveList(answerSheetBatchFiles);
            valid.IsValid = true;
            valid.Type = ValidateType.Save.ToString();
            valid.Message = "Arquivo(s) salvo com sucesso.";
            return valid;
        }

        public AnswerSheetBatchQueue Unzip(EntityFile file, AnswerSheetBatchQueue entity, Guid EntityId, Guid CreatedBy_Id)
        {
            var paramZipFilesAllowed = parameterBusiness.GetParamByKey(EnumParameterKey.ZIP_FILES_ALLOWED.GetDescription(), EntityId);
            var virtualDirectory = parameterBusiness.GetParamByKey(EnumParameterKey.VIRTUAL_PATH.GetDescription(), EntityId);
            var physicalDirectory = parameterBusiness.GetParamByKey(EnumParameterKey.STORAGE_PATH.GetDescription(), EntityId);

            ZipFileInfo zipFile = new ZipFileInfo
            {
                Name = string.Concat(physicalDirectory.Value, new Uri(file.Path).AbsolutePath.Replace("Files/", string.Empty).Replace("/", "\\")),
                ExtensionsAllowed = paramZipFilesAllowed != null ? paramZipFilesAllowed.Value.TrimEnd(Constants.StringArraySeparator).Split(Constants.StringArraySeparator) : null
            };

            Validate zipValid;

            List<AnswerSheetBatchFiles> zipFiles = new List<AnswerSheetBatchFiles>();
            List<ZipFileInfo> zipEntries = ZipFileCreator.ExtractZipFile(zipFile, out zipValid);
            if (zipValid.IsValid)
            {
                if (zipEntries != null && zipEntries.Count > 0)
                {
                    foreach (ZipFileInfo zip in zipEntries)
                    {
                        UploadModel upload = new UploadModel
                        {
                            ContentLength = zip.Length,
                            ContentType = MimeMapping.GetMimeMapping(zip.Name),
                            InputStream = null,
                            Stream = null,
                            Data = zip.Data,
                            FileName = zip.Name,
                            VirtualDirectory = virtualDirectory.Value,
                            PhysicalDirectory = physicalDirectory.Value,
                            FileType = EnumFileType.AnswerSheetBatch,
                            UsuId = CreatedBy_Id
                        };

                        EntityFile fileUploaded = fileBusiness.Upload(upload);
                        if (fileUploaded.Validate.IsValid)
                            zipFiles.Add(new AnswerSheetBatchFiles
                            {
                                File_Id = fileUploaded.Id,
                                CreatedBy_Id = CreatedBy_Id
                            });
                    }
                }
                else
                {
                    entity.AnswerSheetBatchFiles = new List<AnswerSheetBatchFiles>();
                    entity.Validate.IsValid = false;
                    entity.Validate.Type = ValidateType.alert.ToString();
                    entity.Validate.Message = !zipValid.IsValid ? zipValid.Message : "Existe(m) arquivo(s) inválido(s) no arquivo compactado.";
                }
            }
            else
            {
                entity.Validate = zipValid;
            }

            entity.AnswerSheetBatchFiles = zipFiles.ToList();
            entity.CountFiles = zipFiles.Count();
            return entity;
        }

        #endregion

        #region Private Methods

        private Validate ValidateProcessing(Test entity, long BatchId, List<AnswerSheetBatchItems> answers, Validate valid, AnswerSheetBatch batch)
        {
            valid.Message = null;

            if (!entity.TestType.ItemType_Id.HasValue)
            {
                valid.Message = "Não existe um tipo de item associado ao tipo da prova para processar o lote.";
            }
            else
            {
                if (entity.TestType.ItemType == null || (entity.TestType.ItemType != null && entity.TestType.ItemType.QuantityAlternative == null))
                {
                    valid.Message += "Não existe um tipo de item associado ao tipo da prova.";
                }
            }

            if (entity.NumberItem <= 0)
            {
                valid.Message += "<br/> Não existe a quantidade de itens da prova para processar o lote.";
            }

            if (answers == null || (answers != null && answers.Count <= 0))
            {
                valid.Message += "<br/> Não existem itens e alternativas da prova para processar o lote.";
            }

            if (BatchId <= 0)
            {
                valid.Message += "<br/> O lote não existe.";
            }
            else
            {
                if (batch != null)
                {
                    int countNotSent = GetFilesNotSentCount(batch.Id);
                    if (countNotSent <= 0)
                        valid.Message += "<br/> Não existem arquivos novos para serem processados. Por favor, realize o upload de novos arquivos.";
                }
                else
                {
                    valid.Message += "<br/> O lote não existe.";
                }
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

        private Validate ValidateUpdate(AnswerSheetBatchFiles entity, Validate valid)
        {
            valid.Message = null;

            if (entity.Section_Id != null && entity.Section_Id > 0)
            {
                TUR_Turma turma = turmaBusiness.Get((long)entity.Section_Id);
                if (turma == null)
                    entity.Validate.Message = "A turma do arquivo não existe.";
            }
            else if (entity.Student_Id != null && entity.Student_Id > 0)
            {
                ACA_Aluno aluno = alunoBusiness.Get((long)entity.Student_Id);
                if (aluno == null)
                    entity.Validate.Message = "O aluno do arquivo não existe.";
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

        private Validate ValidateAnswerSheetBatch(AnswerSheetBatch answerSheetBatch, Validate valid)
        {
            Test test = testBusiness.GetObject(answerSheetBatch.Test_Id);
            valid.Message = null;

            if (test == null)
            {
                valid.Message = String.Format("Prova {0} não cadastrada.", answerSheetBatch.Test_Id);
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

        private AnswerSheetBatch SaveAnswerSheetBatch(long test_id)
        {
            var entity = new AnswerSheetBatch
            {
                Test_Id = test_id,
                Description = string.Format("Lote_P_{0}", test_id),
                BatchType = EnumAnswerSheetBatchType.QRCode,
                OwnerEntity = EnumAnswerSheetBatchOwner.Test,
                Processing = EnumBatchProcessing.Pending
            };

            return batchBusiness.Save(entity);
        }

        private AnswerSheetBatch CreateBatch(AnswerSheetBatch entity, Guid CreatedBy_Id)
        {
            long SchoolId = entity.School_Id != null ? (int)entity.School_Id : 0;
            long SectionId = entity.Section_Id != null ? (long)entity.Section_Id : 0;

            if (SchoolId > 0 || SectionId > 0)
            {
                AnswerSheetBatchResult info = batchBusiness.GetSchoolSectionInformation(entity.School_Id, entity.Section_Id);
                if (info.Validate.IsValid)
                {
                    entity.SupAdmUnit_Id = info.SupAdmUnitId;
                    if (SectionId > 0)
                        entity.Description = string.Format("Lote_{0}_{1}_{2}", info.SupAdmUnitCode, info.SchoolId, info.SectionCode);
                    else if (SchoolId > 0)
                        entity.Description = string.Format("Lote_{0}_{1}", info.SupAdmUnitCode, info.SchoolId);
                }
            }
            else
            {
                entity.Description = string.Format("Lote_P_{0}", entity.Test_Id);
            }

            entity.CreatedBy_Id = CreatedBy_Id;
            return batchBusiness.Save(entity);

        }

        private void UpdateSituationToPending(List<AnswerSheetBatchFiles> answerSheetBatch, EnumBatchSituation situation, long? batch_id = null)
        {
            foreach (var batchFiles in answerSheetBatch)
            {
                if (batch_id != null)
                    batchFiles.AnswerSheetBatch_Id = batch_id;
                batchFiles.Situation = situation;
                UpdateFileOwner(batchFiles);
            }
            batchRepository.UpdateList(answerSheetBatch);
        }

        private void UpdateFileOwner(AnswerSheetBatchFiles answerSheetBatchFile)
        {
            answerSheetBatchFile = Get(answerSheetBatchFile.Id);
            if (answerSheetBatchFile != null && answerSheetBatchFile.File_Id > 0)
            {
                fileBusiness.UpdateOwnerAndParentId(answerSheetBatchFile.File_Id, answerSheetBatchFile.Id, answerSheetBatchFile.AnswerSheetBatch_Id);
            }
        }

        private void ProcessBatch(AnswerSheetBatch batch, long test_id)
        {
            SendToProcessing(new AnswerSheetBatchFilter() { BatchId = batch.Id, TestId = test_id });
        }

        private AnswerSheetBatch FindBatchByTest(long test_id)
        {
            var batch = batchBusiness.Find(new AnswerSheetBatchFilter { TestId = test_id });
            if (batch == null)
            {
                batch = SaveAnswerSheetBatch(test_id);
            }
            return batch;
        }

        private AnswerSheetBatchFileResult GetStudentFile(long testId, long studentId, long sectionId)
        {
            return batchRepository.GetStudentFile(testId, studentId, sectionId);
        }

        #endregion
    }
}
