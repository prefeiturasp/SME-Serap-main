using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.Worker.Repository.Contracts;
using GestaoAvaliacao.Worker.Repository.MongoDB.Contracts;
using GestaoAvaliacao.Worker.StudentTestsSent.Services.CorrectionResult.Dtos;
using GestaoAvaliacao.Worker.StudentTestsSent.Services.Grades;
using GestaoAvaliacao.Worker.StudentTestsSent.Services.Grades.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Services.CorrectionResult
{
    internal class GenerateCorrectionResultsServices : IGenerateCorrectionResultsServices
    {
        private readonly IParameterRepository _parameterRepository;
        private readonly IStudentCorrectionAuxiliarRepository _studentCorrectionAuxiliarRepository;
        private readonly IStudentCorrectionMongoDBRepository _studentCorrectionMongoDBRepository;
        private readonly ISectionTestStatsMongoDBRepository _sectionTestStatsMongoDBRepository;
        private readonly ITestSectionStatusCorrectionRepository _testSectionStatusCorrectionRepository;
        private readonly ICorrectionResultsMongoDBRepository _correctionResultsMongoDBRepository;
        private readonly IProcessGradesServices _processGradesServices;

        public GenerateCorrectionResultsServices(IParameterRepository parameterRepository, IStudentCorrectionAuxiliarRepository studentCorrectionAuxiliarRepository,
            IStudentCorrectionMongoDBRepository studentCorrectionMongoDBRepository, ISectionTestStatsMongoDBRepository sectionTestStatsMongoDBRepository,
            ITestSectionStatusCorrectionRepository testSectionStatusCorrectionRepository, ICorrectionResultsMongoDBRepository correctionResultsMongoDBRepository,
            IProcessGradesServices processGradesServices)
        {
            _parameterRepository = parameterRepository;
            _studentCorrectionAuxiliarRepository = studentCorrectionAuxiliarRepository;
            _studentCorrectionMongoDBRepository = studentCorrectionMongoDBRepository;
            _sectionTestStatsMongoDBRepository = sectionTestStatsMongoDBRepository;
            _testSectionStatusCorrectionRepository = testSectionStatusCorrectionRepository;
            _correctionResultsMongoDBRepository = correctionResultsMongoDBRepository;
            _processGradesServices = processGradesServices;
        }

        public async Task ExecuteAsync(GenerateCorrectionResultsDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var answerDuplicate = await _parameterRepository.GetAsync("CODE_ALTERNATIVE_DUPLICATE", cancellationToken);
                var answerEmpty = await _parameterRepository.GetAsync("CODE_ALTERNATIVE_EMPTY", cancellationToken);

                var studentCorrections = await _studentCorrectionMongoDBRepository.GetClassCorrectionsAsync(dto.TestId, dto.TurId, cancellationToken);
                var aluMongoList = studentCorrections.Select(i => i.alu_id);
                var alunos = await _studentCorrectionAuxiliarRepository.GetByTestSectionAsync(dto.TestId, dto.TurId, aluMongoList, true);
                var absences = await _studentCorrectionAuxiliarRepository.GetAbsencesByTestSectionAsync(dto.TestId, dto.TurId);

                var processGradesDto = new ProcessGradesDto
                {
                    DreId = dto.DreId,
                    EntId = dto.EntId,
                    EscId = dto.EscId,
                    QuantidadeDeAlunos = studentCorrections.Count(),
                    StudentCorrections = studentCorrections.ToList(),
                    TestId = dto.TestId,
                    TestTemplate = dto.TestTemplate,
                    TurId = dto.TurId
                };
                await _processGradesServices.ExecuteAsync(processGradesDto, cancellationToken);

                int qtdeItems = dto.TestTemplate.Items.Count;
                int qtdeAlunos = alunos.Count();
                int qtdeAusencias = alunos.Count(p => absences.Any(q => q.alu_id == p.alu_id));

                int qtdeLancamentos = 0;

                foreach (var item in studentCorrections)
                    qtdeLancamentos += item.Answers.Count;

                var statusCorrection = (qtdeLancamentos + (qtdeAusencias * qtdeItems)) < (qtdeItems * qtdeAlunos) ? EnumStatusCorrection.PartialSuccess : EnumStatusCorrection.Success;
                await SetStatusCorrecion(dto.TestId, dto.TurId, statusCorrection, cancellationToken);

                var sectionStats = await _sectionTestStatsMongoDBRepository.GetClassSectionStatsAsync(dto.TestId, dto.TurId, cancellationToken);

                var provaDisciplinas = await _studentCorrectionAuxiliarRepository.GetDisciplineItemByTestIdAsync(dto.TestId);

                var statsAluno = (from a in alunos
                                  join s in studentCorrections on a.alu_id equals s.alu_id
                                  select new CorrectionResultsStudents()
                                  {
                                      alu_id = a.alu_id,
                                      alu_nome = a.alu_nome,
                                      Hits = s.Hits,
                                      Performance = s.Grade,
                                      mtu_numeroChamada = a.mtu_numeroChamada,
                                      Alternatives = (from alternative in s.Answers
                                                      join answer in dto.AnswersGrid on alternative.Item_Id equals answer.Item_Id
                                                      orderby answer.Order
                                                      select new CorrectionResultsStudentsAnswers()
                                                      {
                                                          Alternative_Id = alternative.AnswerChoice > 0 ? answer.Alternatives.First(alt => alt.Id == alternative.AnswerChoice).Id : 0,
                                                          Item_Id = alternative.Item_Id,
                                                          Correct = alternative.Correct,
                                                          Numeration = alternative.StrikeThrough ? (answerDuplicate != null ? answerDuplicate.Value : "R") : (alternative.Empty ? (answerEmpty != null ? answerEmpty.Value : "N") :
                                                             answer.Alternatives.First(alt => alt.Id == alternative.AnswerChoice).Numeration.Replace(")", "")),
                                                          Revoked = dto.TestTemplate.Items.First(i => i.Item_Id == alternative.Item_Id).Revoked,
                                                          Discipline_Id = provaDisciplinas.First(p => p.Item_Id == alternative.Item_Id).Discipline_Id
                                                      }).ToList()
                                  }).ToList();

                statsAluno.AddRange((from a in alunos
                                     join abs in absences on a.alu_id equals abs.alu_id
                                     select new CorrectionResultsStudents()
                                     {
                                         alu_id = a.alu_id,
                                         AbsenceReason = abs.AbsenceReason.Description,
                                         mtu_numeroChamada = a.mtu_numeroChamada,
                                         alu_nome = a.alu_nome
                                     }));

                statsAluno.AddRange((from a in alunos
                                     where statsAluno.Count(alu => alu.mtu_numeroChamada == a.mtu_numeroChamada) == 0
                                     select new CorrectionResultsStudents()
                                     {
                                         alu_id = a.alu_id,
                                         mtu_numeroChamada = a.mtu_numeroChamada,
                                         alu_nome = a.alu_nome
                                     }));

                int cur_id = alunos.First().cur_id;
                int crr_id = alunos.First().crr_id;
                int crp_id = alunos.First().crp_id;

                var entity = new CorrectionResults(dto.EntId, dto.TestId, dto.TurId, dto.DreId, dto.EscId, cur_id, crr_id, crp_id)
                {
                    Answers = dto.TestTemplate.Items.Select(i => new CorrectionResultsItems()
                    {
                        Alternative_Id = i.Alternative_Id,
                        Item_Id = i.Item_Id,
                        Order = i.Order,
                        RightChoice = i.Numeration.Replace(")", ""),
                        Revoked = i.Revoked,
                        Discipline_Id = provaDisciplinas.First(p => p.Item_Id == i.Item_Id).Discipline_Id
                    }).OrderBy(i => i.Order).ToList(),
                    Statistics = new CorrectionResultsSectionStats()
                    {
                        GeneralGrade = sectionStats.GeneralGrade,
                        GeneralHits = sectionStats.GeneralHits,
                        Averages = (from s in sectionStats.Answers
                                    join i in dto.TestTemplate.Items on s.Item_Id equals i.Item_Id
                                    orderby i.Order
                                    select new Averages() { Item_Id = s.Item_Id, Average = s.Grade, Revoked = i.Revoked, Discipline_Id = provaDisciplinas.First(p => p.Item_Id == s.Item_Id).Discipline_Id }).ToList()
                    },
                    Students = statsAluno.OrderBy(n => n.mtu_numeroChamada).ToList()
                };

                entity.NumberAnswers = entity.Answers.Count;

                await _correctionResultsMongoDBRepository.DeleteAsync(new CorrectionResults(dto.EntId, dto.TestId, dto.TurId), cancellationToken);
                await _correctionResultsMongoDBRepository.InsertOrReplaceAsync(entity, cancellationToken);
            }
            catch (Exception ex)
            {
                dto.AddError(ex.Message);
            }
        }

        private async Task SetStatusCorrecion(long testId, long turId, EnumStatusCorrection statusCorrection, CancellationToken cancellationToken)
        {
            var testSectionStatusCorrection = await _testSectionStatusCorrectionRepository.GetFirstOrDefaultAsync(testId, turId, cancellationToken);
            testSectionStatusCorrection.StatusCorrection = statusCorrection;
            await _testSectionStatusCorrectionRepository.UpdateAsync(testSectionStatusCorrection, cancellationToken);
        }
    }
}