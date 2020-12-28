using GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta.Repositories.Auxs;
using GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta.Repositories.StudentTests;
using GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta.Services.Dtos;
using GestaoAvaliacao.MongoEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta.Services
{
    internal class FindTestsDoneWithoutAnswersServices : IFindTestsDoneWithoutAnswersServices
    {
        private readonly IFindTestsDoneWithoutAnswersRepository _findTestsDoneWithoutAnswersRepository;
        private readonly ITestTemplateWithoutAnswersRepository _testTemplateWithoutAnswersRepository;
        private readonly IAuxilarRepository _auxilarRepository;

        public FindTestsDoneWithoutAnswersServices()
        {
            _findTestsDoneWithoutAnswersRepository = new FindTestsDoneWithoutAnswersRepository();
            _testTemplateWithoutAnswersRepository = new TestTemplateWithoutAnswersRepository();
            _auxilarRepository = new AuxilarRepository();
        }

        public async Task<IEnumerable<StudentWithTestsDoneWithoutAnswersDto>> FindStudentsWithNoAnswersAsync(DateTime updateDateStart)
        {
            var studentTestsAnswersEmpty = await GetTestsAnswersEmptyAsync(updateDateStart);
            var studentTestsNoAnswers = await _findTestsDoneWithoutAnswersRepository.GetNoAnswersTestsAsync(updateDateStart);
            var studentTestResults = studentTestsNoAnswers.Concat(studentTestsAnswersEmpty);
            if (!studentTestResults?.Any() ?? true) return null;

            var aluIds = studentTestResults.Select(x => x.alu_id).Distinct();
            var testIds = studentTestResults.Select(x => x.Test_Id).Distinct();
            var turIds = studentTestResults.Select(x => x.tur_id).Distinct();

            var students = await _auxilarRepository.GetStudentsById(aluIds);
            var tests = await _auxilarRepository.GetTestsById(testIds);
            var schools = await _auxilarRepository.GetSchoolsBySectionAsync(turIds);

            return studentTestResults
                .Select(x =>
                {
                    var student = students.First(y => y.Id == x.alu_id);
                    var test = tests.First(y => y.Id == x.Test_Id);
                    var school = schools.First(y => y.SectionId == x.tur_id);

                    return new StudentWithTestsDoneWithoutAnswersDto
                    {
                        EolCode = student.EolCode,
                        Name = student.Name,
                        TestName = test.Description,
                        SchoolName = school.Name,
                        DreName = school.DreName
                    };
                })
                .ToList();
        }

        public async Task<IEnumerable<StudentWithTestsDoneWithoutAnswersDto>> FindStudentsMissingTheLastAnswerAsync(DateTime updateDateStart)
        {
            var studentTestsMissingTheLastAnswer = await GetTestsMissingTheLastAnswerAsync(updateDateStart);
            var testIds = studentTestsMissingTheLastAnswer.Select(x => x.Test_Id).Distinct();
            var testTemplates = await _testTemplateWithoutAnswersRepository.GetAsync(testIds);

            foreach(var studentTest in studentTestsMissingTheLastAnswer)
            {
                var testTemplate = testTemplates.FirstOrDefault(x => x.Test_Id == studentTest.Test_Id);
                if (testTemplate is null) continue;

                var lastAnswer = studentTest.Answers.Last();
                var lastAnswerTemplate = testTemplate.Items.FirstOrDefault(x => x.Item_Id == lastAnswer.Item_Id);
                if (lastAnswerTemplate is null) continue;

                lastAnswer.AnswerChoice = lastAnswerTemplate.Alternative_Id;
                lastAnswer.Correct = true;
                lastAnswer.Empty = false;
                lastAnswer.StrikeThrough = false;
                lastAnswer.Automatic = false;

                await _findTestsDoneWithoutAnswersRepository.Replace(studentTest);
            }
        }

        private async Task<IEnumerable<StudentCorrection>> GetTestsAnswersEmptyAsync(DateTime updateDateStart)
        {
            var result = new List<StudentCorrection>();
            for(var row = 0; ;row++)
            {
                var studentTestsAnyAnswersEmpty = await _findTestsDoneWithoutAnswersRepository.GetTestsAnswersEmptyAsync(updateDateStart, row);
                if (!studentTestsAnyAnswersEmpty?.Any() ?? true) break;
                var studentTestsAllAnswersEmpty = studentTestsAnyAnswersEmpty.Where(x => x.Answers.All(y => y.Empty)).ToList();
                result.AddRange(studentTestsAllAnswersEmpty);
            }

            return result;
        }

        private async Task<IEnumerable<StudentCorrection>> GetTestsMissingTheLastAnswerAsync(DateTime updateDateStart)
        {
            var result = new List<StudentCorrection>();
            for (var row = 0; ; row++)
            {
                var studentTestsAnyAnswersEmpty = await _findTestsDoneWithoutAnswersRepository.GetTestsAnswersEmptyAsync(updateDateStart, row);
                if (!studentTestsAnyAnswersEmpty?.Any() ?? true) break;
                var studentTestsAllAnswersEmpty = studentTestsAnyAnswersEmpty.Where(x => x.Answers.Last().Empty && x.Answers.Any(y => !y.Empty)).ToList();
                result.AddRange(studentTestsAllAnswersEmpty);
            }

            return result;
        }
    }
}