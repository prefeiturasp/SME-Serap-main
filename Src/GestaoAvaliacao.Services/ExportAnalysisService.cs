using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Services
{
    public class ExportAnalysisService
    {
        readonly IExportAnalysisBusiness exportAnalysisBusiness;
        readonly IParameterBusiness parameterBusiness;
        readonly IBlockBusiness blockBusiness;
        readonly ICorrectionBusiness correctionBusiness;
        readonly ITestSectionStatusCorrectionBusiness testSectionStatusCorrectionBusiness;
        readonly ITUR_TurmaBusiness turmaBusiness;
        readonly IStudentCorrectionBusiness studentCorrectionBusiness;
        readonly ITestBusiness testBusiness;
        readonly IFileBusiness fileBusiness;
        readonly IACA_AlunoBusiness alunoBusiness;

        public ExportAnalysisService(IParameterBusiness parameterBusiness, IBlockBusiness blockBusiness, ICorrectionBusiness correctionBusiness,
            ITestSectionStatusCorrectionBusiness testSectionStatusCorrectionBusiness, ITUR_TurmaBusiness turmaBusiness, IStudentCorrectionBusiness studentCorrectionBusiness,
            ITestBusiness testBusiness, IFileBusiness fileBusiness, IExportAnalysisBusiness exportAnalysisBusiness, IACA_AlunoBusiness alunoBusiness)
        {
            this.exportAnalysisBusiness = exportAnalysisBusiness;
            this.parameterBusiness = parameterBusiness;
            this.blockBusiness = blockBusiness;
            this.correctionBusiness = correctionBusiness;
            this.testSectionStatusCorrectionBusiness = testSectionStatusCorrectionBusiness;
            this.turmaBusiness = turmaBusiness;
            this.studentCorrectionBusiness = studentCorrectionBusiness;
            this.testBusiness = testBusiness;
            this.fileBusiness = fileBusiness;
            this.alunoBusiness = alunoBusiness;

        }

        public async Task Execute()
        {
            await ExportAnalysis(ConfigurationManager.AppSettings["StoragePath"], ConfigurationManager.AppSettings["VirtualPath"].Replace("/files", string.Empty));
        }

        public async Task ExportAnalysis(string physicalPath, string virtualDirectory)
        {

            var solicitations = exportAnalysisBusiness.GetByExecutionState(EnumServiceState.Pending);

            foreach (var solicitation in solicitations)
            {
                try
                {
                    var test = testBusiness.GetObjectWithTestType(solicitation.Test_Id);
                    var separator = parameterBusiness.GetByKey("CHAR_SEP_CSV", test.TestType.EntityId).Value;
                    var report = new StringBuilder(string.Format("Prova{0}IdDre{0}DRE{0}IdEscola{0}Escola{0}IdTurma{0}Turma{0}IdAluno{0}Codigo EOL{0}Aluno{0}", separator));

                    report.Append(await AnalysisExportItems(separator, test));
                    report.Append(await AnalysisExportSections(separator, test));

                    MemoryStream stream = new MemoryStream();
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(report.ToString());
                    writer.Flush();
                    stream.Position = 0;

                    DeleteFiles(solicitation.Id, solicitation.Test_Id, EnumFileType.AnalysisItem, test.TestType.EntityId);


                    var file = fileBusiness.Upload(new UploadModel()
                    {
                        ContentLength = Convert.ToInt32(stream.Length),
                        ContentType = "text/csv",
                        Stream = stream,
                        FileName = string.Format("{0}_{1}.csv", test.Id, test.Description),
                        FileType = EnumFileType.AnalysisItem,
                        PhysicalDirectory = physicalPath,
                        VirtualDirectory = virtualDirectory
                    });

                    file.OwnerId = solicitation.Id;
                    file.ParentOwnerId = solicitation.Test_Id;
                    file.OriginalName = string.Format("ExportAnalysis_{0}.csv", test.Description);

                    fileBusiness.Update(file.Id, file);

                    solicitation.StateExecution = EnumServiceState.Success;
                    exportAnalysisBusiness.Update(solicitation);
                }
                catch (Exception e)
                {
                    solicitation.StateExecution = EnumServiceState.Error;
                    exportAnalysisBusiness.Update(solicitation);
                    GestaoAvaliacao.LogFacade.LogFacade.SaveError(e, string.Format("Erro ao gerar folha de resposta em lote para a prova {0}, id do lote {1}", solicitation.Test_Id, solicitation.Id));
                }
            }
        }

        private void DeleteFiles(long ownerId, long parentId, EnumFileType ownerType, Guid EntityId)
        {
            try
            {
                List<Entities.File> files = fileBusiness.GetFilesByOwner(ownerId, parentId, ownerType);
                var physicalDirectory = parameterBusiness.GetParamByKey(EnumParameterKey.STORAGE_PATH.GetDescription(), EntityId);
                foreach (var file in files)
                {
                    string nameFile = string.Concat(physicalDirectory.Value, new Uri(file.Path).AbsolutePath.Replace("Files/", string.Empty).Replace("/", "\\"));
                    fileBusiness.Delete(file.Id, nameFile);
                }
            }
            catch (Exception e)
            {
                GestaoAvaliacao.LogFacade.LogFacade.SaveError(e, "Erro ao deletar arquivos de exportação das respostas gerados na execução anterior");
                throw;
            }

        }

        private async Task<string> AnalysisExportItems(string separator, Test test)
        {
            try
            {
                var report = new StringBuilder();

                var idsItem = string.Format("{0}{0}{0}{0}{0}{0}{0}{0}{0}{0}", separator);
                var testTemplate = await correctionBusiness.GetTestTemplate(test.Id, test.TestType.EntityId);

                foreach (var item in testTemplate.Items)
                {
                    report.AppendFormat("{0} - {1}{2}", item.Numeration.Substring(0, 1), ((item.Order + 1).ToString().PadLeft(2, '0')), separator);
                    idsItem = string.Concat(idsItem, item.Item_Id, separator);
                }

                report.AppendLine();
                report.AppendLine(idsItem);

                return report.ToString();
            }
            catch (Exception e)
            {
                GestaoAvaliacao.LogFacade.LogFacade.SaveError(e, string.Format("Erro ao obter dados dos items da prova {0} para geração de arquivo de análise de item", test.Id));
                throw;
            }
        }
        private async Task<string> AnalysisExportSections(string separator, Test test)
        {
            try
            {
                var report = new StringBuilder();
                var sections = testSectionStatusCorrectionBusiness.GetByTest(test.Id).OrderBy(a => a.tur_id);
                var items = blockBusiness.GetTestQuestions(test.Id);

                sections
                    .AsParallel()
                    .WithDegreeOfParallelism(5)
                    .ForAll(async (section) => report.Append(await this.AnalysisExportStudents(items, test, section, separator)));

                return report.ToString();
            }
            catch (Exception e)
            {
                GestaoAvaliacao.LogFacade.LogFacade.SaveError(e, string.Format("Erro ao obter dados das turmas da prova {0} para geração de arquivo de análise de item", test.Id));
                throw;
            }
        }

        private async Task<string> AnalysisExportStudents(IEnumerable<StudentCorrectionAnswerGrid> items, Test test, TestSectionStatusCorrection turma,
            string separator)
        {
            try
            {
                Parameter answerDuplicate = parameterBusiness.GetByKey("CODE_ALTERNATIVE_DUPLICATE", test.TestType.EntityId);
                Parameter answerEmpty = parameterBusiness.GetByKey("CODE_ALTERNATIVE_EMPTY", test.TestType.EntityId);

                var report = new StringBuilder();
                var corrections = await studentCorrectionBusiness.GetByTest(test.Id, turma.tur_id);
                IEnumerable<ACA_Aluno> alunos = new List<ACA_Aluno>();
                if (corrections != null && corrections.Count > 0)
                {
                    alunos = alunoBusiness.Get(corrections.Select(i => i.alu_id));
                }

                foreach (var aluno in corrections)
                {
                    try
                    {
                        ACA_Aluno dadoAluno = alunos.FirstOrDefault(p => p.alu_id == aluno.alu_id);

                        report.AppendFormat("{0}{1}", test.Id, separator);
                        report.AppendFormat("{0}{1}{2}{1}", turma.idDRE, separator, turma.DRE);
                        report.AppendFormat("{0}{1}{2}{1}", turma.esc_id, separator, turma.esc_nome);
                        report.AppendFormat("{0}{1}{2}{1}", turma.tur_id, separator, turma.tur_codigo);
                        report.AppendFormat("{0}{1}{2}{1}{3}{1}", aluno.alu_id, separator, dadoAluno.alu_matricula, dadoAluno.alu_nome);

                        foreach (var item in items)
                        {
                            //Verifica se já foi corrigida a questão para o aluno, se foi, pegar a resposta, senão deixa vazio
                            var answer = aluno.Answers.FirstOrDefault(a => a.Item_Id == item.Item_Id);

                            var choice = (answer != null && answer.AnswerChoice > 0
                                            ? (item.Alternatives.First(a => a.Id == answer.AnswerChoice).Numeration.Substring(0, 1))
                                            : (answer != null && answer.AnswerChoice == 0 && answer.Empty
                                                ? (answerEmpty != null ? answerEmpty.Value : "N")
                                                : (answer != null && answer.AnswerChoice == 0 && answer.StrikeThrough
                                                    ? (answerDuplicate != null ? answerDuplicate.Value : "R")
                                                    : string.Empty))
                                        );
                            report.AppendFormat("{0}{1}", choice, separator);
                        }
                        report.AppendLine();
                    }
                    catch (Exception e)
                    {
                        GestaoAvaliacao.LogFacade.LogFacade.SaveError(e, string.Format("Erro ao obter dados do aluno {0} da prova {0} para geração de arquivo de análise de item", aluno.alu_id, test.Id));
                        throw;
                    }
                }

                return report.ToString();
            }
            catch (Exception e)
            {
                GestaoAvaliacao.LogFacade.LogFacade.SaveError(e, string.Format("Erro ao obter dados dos alunos da turma {0} da prova {1} para geração de arquivo de análise de item", turma.tur_id, test.Id));
                throw;
            }
        }
    }
}
