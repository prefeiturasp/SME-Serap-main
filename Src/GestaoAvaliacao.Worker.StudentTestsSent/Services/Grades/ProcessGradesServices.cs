using GestaoAvaliacao.Worker.Domain.MongoDB.Entities.Tests;
using GestaoAvaliacao.Worker.Repository.Contracts;
using GestaoAvaliacao.Worker.Repository.MongoDB.Contracts;
using GestaoAvaliacao.Worker.StudentTestsSent.Services.Grades.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Services.Grades
{
    internal class ProcessGradesServices : IProcessGradesServices
    {
        private readonly ITestTemplateMongoDBRepository _testTemplateMongoDBRepository;
        private readonly ISectionTestStatsMongoDBRepository _sectionTestStatsMongoDBRepository;
        private readonly IStudentCorrectionMongoDBRepository _studentCorrectionMongoDBRepository;
        private readonly IStudentCorrectionAuxiliarRepository _studentCorrectionAuxiliarRepository;

        public ProcessGradesServices(ITestTemplateMongoDBRepository testTemplateMongoDBRepository, ISectionTestStatsMongoDBRepository sectionTestStatsMongoDBRepository,
            IStudentCorrectionMongoDBRepository studentCorrectionMongoDBRepository, IStudentCorrectionAuxiliarRepository studentCorrectionAuxiliarRepository)
        {
            _testTemplateMongoDBRepository = testTemplateMongoDBRepository;
            _sectionTestStatsMongoDBRepository = sectionTestStatsMongoDBRepository;
            _studentCorrectionMongoDBRepository = studentCorrectionMongoDBRepository;
            _studentCorrectionAuxiliarRepository = studentCorrectionAuxiliarRepository;
        }

        public async Task ExecuteAsync(ProcessGradesDto dto, CancellationToken cancellationToken)
        {
            //Totais para tirar dados da turma
            int acertos = 0;
            double desempenho = 0;

            //Total para pegar o desempenho por ítem
            Dictionary<long, int> desempenhoItem = new Dictionary<long, int>();
            var revokeds = await _studentCorrectionAuxiliarRepository.GetRevokedItemsByTestAsync(dto.TestId);
            int totalItem = dto.TestTemplate.Items.Count - revokeds.Count();

            if (revokeds.Any())
            {
                foreach (var item in dto.TestTemplate.Items.Where(i => revokeds.Contains(i.Item_Id)))
                    item.Revoked = true;

                dto.TestTemplate = await _testTemplateMongoDBRepository.ReplaceAsync(dto.TestTemplate, cancellationToken);
            }

            foreach (var studentCorrection in dto.StudentCorrections)
            {
                if (cancellationToken.IsCancellationRequested) return;

                studentCorrection.Hits = studentCorrection.Answers.Count(i => i.Correct && !revokeds.Contains(i.Item_Id));

                var grade = (studentCorrection.Hits / (double)totalItem) * 100;
                studentCorrection.Grade = Math.Round(grade, 2);

                acertos += studentCorrection.Hits;
                desempenho += studentCorrection.Grade;

                studentCorrection.NumberAnswers = studentCorrection.Answers.Count;

                foreach (var answer in studentCorrection.Answers)
                {
                    if (!desempenhoItem.ContainsKey(answer.Item_Id))
                        desempenhoItem.Add(answer.Item_Id, 0);

                    if (answer.Correct)
                        desempenhoItem[answer.Item_Id]++;
                }

                await _studentCorrectionMongoDBRepository.InsertOrReplaceAsync(studentCorrection, cancellationToken);
            }

            var sectionTestStats = new SectionTestStatsEntityWorker(dto.TestId, dto.TurId, dto.EntId, dto.DreId, dto.EscId)
            {
                GeneralGrade = dto.QuantidadeDeAlunos == 0 ? 0 : Math.Round((desempenho / (double)dto.QuantidadeDeAlunos), 2),
                GeneralHits = dto.QuantidadeDeAlunos == 0 ? 0 : Math.Round((acertos / (double)dto.QuantidadeDeAlunos), 2),
                Answers = desempenhoItem.Select(x => new SectionTestStatsAnswes() { Item_Id = x.Key, Grade = Math.Round((x.Value / (double)dto.QuantidadeDeAlunos) * 100, 2) }).ToList()
            };

            sectionTestStats.NumberAnswers = sectionTestStats.Answers.Count;
            await _sectionTestStatsMongoDBRepository.InsertOrReplaceAsync(sectionTestStats, cancellationToken);
        }
    }
}