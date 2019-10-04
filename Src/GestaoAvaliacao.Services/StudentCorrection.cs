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

            var studentCorrections = await this.studentCorrectionBusiness.GetByTest(new List<long>() { testId });
            var tempCorrectionResultList = new List<TempCorrectionResult>();

            if (studentCorrections.Any())
            {
                if (teamId.HasValue)
                    studentCorrections = studentCorrections.FindAll(sc => sc.tur_id.Equals(teamId.Value));

                foreach (var studentCorrection in studentCorrections)
                {
                    var answer = studentCorrection.Answers.SingleOrDefault(a => a.Item_Id.Equals(itemIdOld) && a.AnswerChoice.Equals(alternativeIdOld));
                    if (answer != null)
                    {
                        answer.Item_Id = itemIdNew ?? itemIdNew.Value;
                        answer.AnswerChoice = alternativeIdNew.HasValue ? alternativeIdNew.Value : alternativeIdOld;
                        answer.Correct = answerCorrect ?? answerCorrect.Value;
                        studentCorrection.Hits = studentCorrection.Answers.Count(a => a.Correct);

                        var tempCorrectionResult = await testSectionStatusCorrectionBusiness.GetTempCorrection(testId, studentCorrection.tur_id);

                        if (tempCorrectionResult != null && !tempCorrectionResult.Processed)
                            continue;
                        else
                            tempCorrectionResult = new TempCorrectionResult(Guid.Parse(studentCorrection._id.Substring(0, 36)), studentCorrection.Test_Id, studentCorrection.tur_id);

                        if (!tempCorrectionResultList.Any(tcr => tcr.Tur_id.Equals(studentCorrection.tur_id)))
                            tempCorrectionResultList.Add(tempCorrectionResult);
                    }
                }

                var testTemplate = await correctionBusiness.GetTestTemplate(testId, Guid.Parse(studentCorrections.First()._id.Substring(0, 36)));
                var item = testTemplate.Items.SingleOrDefault(tt => tt.Item_Id.Equals(itemIdOld));
                if (item != null)
                {
                    item.Item_Id = itemIdNew ?? itemIdNew.Value;
                    item.Alternative_Id = alternativeIdNew ?? alternativeIdNew.Value;
                    var alternative = alternativeBusiness
                        .GetAlternativesByItens(new List<string>() { itemIdNew.ToString() }.AsEnumerable(), testId)
                        .SingleOrDefault(a => a.Id.Equals(alternativeIdNew.HasValue ? alternativeIdNew.Value : item.Alternative_Id));
                    item.Numeration = alternative != null ? alternative.Numeration : item.Numeration;
                    await testTemplateRepository.Replace(testTemplate);
                }

                await this.studentCorrectionBusiness.Save(studentCorrections);

                tempCorrectionResultList.ForEach(async tcr =>
                {
                    tcr.Processed = false;
                    await testSectionStatusCorrectionBusiness.UpdateTempCorrection(tcr);                    
                });
            }
        }
    }
}
