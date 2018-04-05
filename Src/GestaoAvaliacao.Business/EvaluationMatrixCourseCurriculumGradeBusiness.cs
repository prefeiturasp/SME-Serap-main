using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
namespace GestaoAvaliacao.Business
{
    public class EvaluationMatrixCourseCurriculumGradeBusiness : IEvaluationMatrixCourseCurriculumGradeBusiness
    {
        private readonly IEvaluationMatrixCourseCurriculumGradeRepository evaluationMatrixCourseCurriculumGradeRepository;
        private readonly IACA_CurriculoPeriodoBusiness curriculumGradeBusiness;
        private readonly IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness;

        public EvaluationMatrixCourseCurriculumGradeBusiness(IEvaluationMatrixCourseCurriculumGradeRepository evaluationMatrixCourseCurriculumGradeRepository, IACA_CurriculoPeriodoBusiness curriculumGradeBusiness, IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness)
        {
            this.evaluationMatrixCourseCurriculumGradeRepository = evaluationMatrixCourseCurriculumGradeRepository;
            this.curriculumGradeBusiness = curriculumGradeBusiness;
            this.tipoCurriculoPeriodoBusiness = tipoCurriculoPeriodoBusiness;
        }

        #region Read

        public EvaluationMatrixCourseCurriculumGrade GetByCurriculumGradeId(long curriculumGradeId, int evaluationMatrixId, int courseId)
        {
            return evaluationMatrixCourseCurriculumGradeRepository.GetByCurriculumGradeId(curriculumGradeId, evaluationMatrixId, courseId);
        }

        public EvaluationMatrixCourseCurriculumGrade Get(long id)
        {
            return evaluationMatrixCourseCurriculumGradeRepository.Get(id);
        }

        public List<EvaluationMatrixCourseCurriculumGrade> GetCurriculumGradesByMatrix(int evaluationMatrixId)
        {
            return evaluationMatrixCourseCurriculumGradeRepository.GetCurriculumGradesByMatrix(evaluationMatrixId);
        }

        #endregion

        #region Write

        public void SaveList(List<EvaluationMatrixCourseCurriculumGrade> listEntity, int evaluationMatrixId, int courseId, int typeLevelEducationId, int modalityId, Guid ent_id)
        {
            IEnumerable<ACA_CurriculoPeriodo> lista = curriculumGradeBusiness.Load(ent_id, courseId);

            foreach (ACA_CurriculoPeriodo cg in lista)
            {
                EvaluationMatrixCourseCurriculumGrade selected = listEntity.FirstOrDefault(le => le.CurriculumGradeId == cg.crp_id);

                EvaluationMatrixCourseCurriculumGrade curriculum = GetByCurriculumGradeId(cg.crp_id, evaluationMatrixId, courseId);

                if ((selected != null) && (curriculum == null))
                {
                    selected.CreateDate = DateTime.Now;
                    selected.UpdateDate = DateTime.Now;
                    selected.State = Convert.ToByte(EnumState.ativo);
                    selected.EvaluationMatrixCourse_Id = selected.EvaluationMatrixCourse.Id;
                    selected.EvaluationMatrixCourse = null;
                    selected.TypeCurriculumGradeId = tipoCurriculoPeriodoBusiness.GetId(typeLevelEducationId, modalityId, selected.Ordem);

                    evaluationMatrixCourseCurriculumGradeRepository.SaveList(selected, typeLevelEducationId, modalityId);
                }

                if (((selected != null) && (curriculum != null)) && (curriculum.State == (Byte)EnumState.excluido))
                {                   
                        curriculum.State = (Byte)EnumState.ativo;
                        evaluationMatrixCourseCurriculumGradeRepository.Update(curriculum);
                }

                if (((selected == null) && (curriculum != null)) && (curriculum.State == (Byte)EnumState.ativo))
                {
                        curriculum.State = (Byte)EnumState.excluido;
                        evaluationMatrixCourseCurriculumGradeRepository.Update(curriculum);
                }
            }
        }

        #endregion
    }
}
