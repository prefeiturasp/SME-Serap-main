using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Business
{
    public class TestTypeCourseCurriculumGradeBusiness : ITestTypeCourseCurriculumGradeBusiness
    {
        private readonly ITestTypeCourseCurriculumGradeRepository testTypeCourseCurriculumGradeRepository;
        private readonly IACA_CurriculoPeriodoBusiness curriculumGradeBusiness;
        private readonly IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness;

        public TestTypeCourseCurriculumGradeBusiness(ITestTypeCourseCurriculumGradeRepository testTypeCourseCurriculumGradeRepository, IACA_CurriculoPeriodoBusiness curriculumGradeBusiness, IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness)
        {
            this.testTypeCourseCurriculumGradeRepository = testTypeCourseCurriculumGradeRepository;
            this.curriculumGradeBusiness = curriculumGradeBusiness;
            this.tipoCurriculoPeriodoBusiness = tipoCurriculoPeriodoBusiness;
        }

        #region Custom

        private Validate Validate(TestTypeCourseCurriculumGrade entity, ValidateAction action, Validate valid)
        {
            valid.Message = null;

            if (action == ValidateAction.Delete)
            {
                TestTypeCourseCurriculumGrade ent = Get(entity.Id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrado o ano a ser excluído.";
                    valid.Code = 404;
                }
            }

            if (!string.IsNullOrEmpty(valid.Message))
            {
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

        public List<TestTypeCourseCurriculumGrade> GetCurriculumGradesByTestType(int testTypeId)
        {
            return testTypeCourseCurriculumGradeRepository.GetCurriculumGradesByTestType(testTypeId);
        }

        public TestTypeCourseCurriculumGrade Get(long id)
        {
            return testTypeCourseCurriculumGradeRepository.Get(id);
        }

        public TestTypeCourseCurriculumGrade GetByCurriculumGradeId(long curriculumGradeId, int testTypeId, int courseId)
        {
            return testTypeCourseCurriculumGradeRepository.GetByCurriculumGradeId(curriculumGradeId, testTypeId, courseId);
        }

        #endregion

        #region Write

        public void SaveList(List<TestTypeCourseCurriculumGrade> listEntity, int testTypeId, int courseId, int typeLevelEducationId, int modalityId, Guid ent_id)
        {
            IEnumerable<ACA_CurriculoPeriodo> lista = curriculumGradeBusiness.Load(ent_id, courseId);

            foreach (ACA_CurriculoPeriodo cg in lista)
            {
                TestTypeCourseCurriculumGrade selected = listEntity.FirstOrDefault(le => le.CurriculumGradeId == cg.crp_id);

                TestTypeCourseCurriculumGrade curriculum = GetByCurriculumGradeId(cg.crp_id, testTypeId, courseId);

                if ((selected != null) && (curriculum == null))
                {
                    selected.CreateDate = DateTime.Now;
                    selected.UpdateDate = DateTime.Now;
                    selected.State = Convert.ToByte(EnumState.ativo);
                    selected.TestTypeCourse_Id = selected.TestTypeCourse.Id;
                    selected.TestTypeCourse = null;
                    selected.TypeCurriculumGradeId = tipoCurriculoPeriodoBusiness.GetId(typeLevelEducationId, modalityId, selected.Ordem);

                    testTypeCourseCurriculumGradeRepository.SaveList(selected, typeLevelEducationId, modalityId);
                }

                if (((selected != null) && (curriculum != null)) && (curriculum.State == (Byte)EnumState.excluido))
                {
                        curriculum.State = (Byte)EnumState.ativo;
                        testTypeCourseCurriculumGradeRepository.Update(curriculum);
                }

                if (((selected == null) && (curriculum != null)) && (curriculum.State == (Byte)EnumState.ativo))
                {                    
                        curriculum.State = (Byte)EnumState.excluido;
                        testTypeCourseCurriculumGradeRepository.Update(curriculum);
                }
            }
        }

        public TestTypeCourseCurriculumGrade Delete(long id)
        {
            TestTypeCourseCurriculumGrade entity = new TestTypeCourseCurriculumGrade { Id = id };
            entity.Validate = Validate(entity, ValidateAction.Delete, entity.Validate);
            if (entity.Validate.IsValid)
            {
                testTypeCourseCurriculumGradeRepository.Delete(entity);
                entity.Validate.Type = ValidateType.Delete.ToString();
                entity.Validate.Message = "Ano excluído com sucesso.";
            }

            return entity;
        }

        #endregion        
    }
}
