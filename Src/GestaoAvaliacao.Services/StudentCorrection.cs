using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Services
{
    public class StudentCorrection
    {
        private readonly IStudentCorrectionBusiness studentCorrectionBusiness;
        private readonly ICorrectionBusiness correctionBusiness;
        private readonly IAlternativeBusiness alternativeBusiness;
        private readonly ITestTemplateRepository testTemplateRepository;
        private readonly ITestSectionStatusCorrectionBusiness testSectionStatusCorrectionBusiness;

        public StudentCorrection(IStudentCorrectionBusiness studentCorrectionBusiness,
                                 ICorrectionBusiness correctionBusiness,
                                 IAlternativeBusiness alternativeBusiness,
                                 ITestTemplateRepository testTemplateRepository,
                                 ITestSectionStatusCorrectionBusiness testSectionStatusCorrectionBusiness)
        {
            this.studentCorrectionBusiness = studentCorrectionBusiness ?? throw new ArgumentNullException(nameof(studentCorrectionBusiness));
            this.correctionBusiness = correctionBusiness ?? throw new ArgumentNullException(nameof(correctionBusiness));
            this.alternativeBusiness = alternativeBusiness ?? throw new ArgumentNullException(nameof(alternativeBusiness));
            this.testTemplateRepository = testTemplateRepository ?? throw new ArgumentNullException(nameof(testTemplateRepository));
            this.testSectionStatusCorrectionBusiness = testSectionStatusCorrectionBusiness ?? throw new ArgumentNullException(nameof(testSectionStatusCorrectionBusiness));
        }

        public async Task UpdateAnswersTest(long testId, long? teamId, long itemIdOld, long? itemIdNew, long alternativeIdOld, long? alternativeIdNew, bool? answerCorrect)
        {
            if (testId < 1)
                throw new InvalidOperationException("Informe o Id do teste.");

            if (alternativeIdOld < 1)
                throw new InvalidOperationException("Informe o Id da alternativa antiga.");

            var studentCorrections = teamId.HasValue ?
                                    await this.studentCorrectionBusiness.GetByTest(testId, teamId.Value) :
                                    await this.studentCorrectionBusiness.GetByTest(new List<long>() { testId });

            var tempCorrectionResultList = new List<TempCorrectionResult>();

            if (studentCorrections.Any())
            {
                if (teamId.HasValue)
                    studentCorrections = studentCorrections.FindAll(sc => sc.tur_id.Equals(teamId.Value));

                studentCorrections = (from sc in studentCorrections
                                      from a in sc.Answers
                                      where a.Item_Id.Equals(itemIdOld) &&
                                            a.AnswerChoice.Equals(alternativeIdOld)
                                      select sc).ToList();

                foreach (var studentCorrection in studentCorrections)
                {
                    var answer = studentCorrection.Answers.SingleOrDefault(a => a.Item_Id.Equals(itemIdOld) && a.AnswerChoice.Equals(alternativeIdOld));
                    if (answer != null)
                    {
                        answer.Item_Id = itemIdNew.HasValue ? itemIdNew.Value : answer.Item_Id;
                        answer.AnswerChoice = alternativeIdNew.HasValue ? alternativeIdNew.Value : alternativeIdOld;
                        answer.Correct = answerCorrect.HasValue ? answerCorrect.Value : answer.Correct;
                        studentCorrection.Hits = studentCorrection.Answers.Count(a => a.Correct);

                        var tempCorrectionResult = await testSectionStatusCorrectionBusiness.GetTempCorrection(Guid.Parse(studentCorrection._id.Substring(0, 36)), testId, studentCorrection.tur_id);

                        if (tempCorrectionResult != null && !tempCorrectionResult.Processed)
                            continue;
                        else if (tempCorrectionResult == null)
                            tempCorrectionResult = new TempCorrectionResult(Guid.Parse(studentCorrection._id.Substring(0, 36)), studentCorrection.Test_Id, studentCorrection.tur_id);

                        if (!tempCorrectionResultList.Any(tcr => tcr.Tur_id.Equals(studentCorrection.tur_id)))
                            tempCorrectionResultList.Add(tempCorrectionResult);
                    }
                    System.Diagnostics.Debug.WriteLine(string.Concat(studentCorrections.IndexOf(studentCorrection), " de ", studentCorrections.Count));
                }

                await UpdateTestTemplate(testId, itemIdOld, itemIdNew, alternativeIdNew, studentCorrections);

                await this.studentCorrectionBusiness.Save(studentCorrections);

                tempCorrectionResultList.ForEach(async tcr =>
                {
                    tcr.Processed = false;
                    await testSectionStatusCorrectionBusiness.UpdateTempCorrection(tcr);
                });
            }
        }

        private async Task UpdateTestTemplate(long testId, long itemIdOld, long? itemIdNew, long? alternativeIdNew, List<MongoEntities.StudentCorrection> studentCorrections)
        {
            var testTemplate = await correctionBusiness.GetTestTemplate(testId, Guid.Parse(studentCorrections.First()._id.Substring(0, 36)));
            var item = testTemplate.Items.SingleOrDefault(tt => tt.Item_Id.Equals(itemIdOld));
            if (item != null)
            {
                item.Item_Id = itemIdNew.HasValue ? itemIdNew.Value : itemIdOld;
                item.Alternative_Id = alternativeIdNew ?? alternativeIdNew.Value;
                var alternative = alternativeBusiness
                    .GetAlternativesByItens(new List<string>() { itemIdNew.HasValue ? itemIdNew.Value.ToString() : itemIdOld.ToString() }.AsEnumerable(), testId)
                    .SingleOrDefault(a => a.Id.Equals(alternativeIdNew.HasValue ? alternativeIdNew.Value : item.Alternative_Id));
                item.Numeration = alternative != null ? alternative.Numeration : item.Numeration;
                await testTemplateRepository.Replace(testTemplate);
            }
        }

        public async Task IncludeTestNewCorrectionResult(long testId, long? teamId)
        {
            if (testId < 1)
                throw new InvalidOperationException("Informe o Id do teste.");

            var studentCorrections = teamId.HasValue ?
                                    await this.studentCorrectionBusiness.GetByTest(testId, teamId.Value) :
                                    await this.studentCorrectionBusiness.GetByTest(new List<long>() { testId });

            if (studentCorrections.Any())
            {
                var testAndTeams = studentCorrections.Select(sc => new
                {
                    Test_Id = sc.Test_Id,
                    Tur_id = sc.tur_id,
                    Guid_Student_Correction = Guid.Parse(sc._id.Substring(0, 36))
                }).Distinct();

                foreach (var item in testAndTeams)
                {
                    var tempCorrectionResult = await testSectionStatusCorrectionBusiness.GetTempCorrection(item.Guid_Student_Correction, item.Test_Id, item.Tur_id);

                    if (tempCorrectionResult != null && !tempCorrectionResult.Processed)
                        continue;
                    else if (tempCorrectionResult == null)
                        tempCorrectionResult = new TempCorrectionResult(item.Guid_Student_Correction, item.Test_Id, item.Tur_id);

                    tempCorrectionResult.Processed = false;
                    await testSectionStatusCorrectionBusiness.UpdateTempCorrection(tempCorrectionResult);
                }
            }
        }

        public async Task<IList<MongoEntities.StudentCorrection>> GetByTest(long testId, long teamId)
        {
            return await studentCorrectionBusiness.GetByTest(testId, teamId);
        }
    }
}
