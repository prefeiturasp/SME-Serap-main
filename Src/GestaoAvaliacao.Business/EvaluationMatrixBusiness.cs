using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Business
{
    public class EvaluationMatrixBusiness : IEvaluationMatrixBusiness
    {
        private readonly IEvaluationMatrixRepository evaluationMatrixRepository;
        private readonly ITestCurriculumGradeBusiness testCurriculumGradeBusiness;

        public EvaluationMatrixBusiness(IEvaluationMatrixRepository evaluationMatrixRepository, ITestCurriculumGradeBusiness testCurriculumGradeBusiness)
        {
            this.evaluationMatrixRepository = evaluationMatrixRepository;
            this.testCurriculumGradeBusiness = testCurriculumGradeBusiness;
        }

        #region Custom

        private Validate Validate(EvaluationMatrix entity, ValidateAction action, Validate valid)
        {
            valid.Message = null;
            if ((action == ValidateAction.Save) && (entity == null || string.IsNullOrEmpty(entity.Description) || string.IsNullOrEmpty(entity.Edition) || entity.ModelEvaluationMatrix == null || entity.Discipline == null))
            {
                valid.Message = "Não foram preenchidos todos os campos obrigatórios.";
            }

            if (action == ValidateAction.Update)
            {
                EvaluationMatrix ent = Get(entity.Id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrada a matriz de avaliação a ser atualizada.";
                    valid.Code = 404;
                }
            }

            if (action == ValidateAction.Delete)
            {
                EvaluationMatrix ent = Get(entity.Id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrada a matriz de avaliação a ser excluída.";
                    valid.Code = 404;
                }

                if (evaluationMatrixRepository.ExistsItemMatrix(entity.Id))
                    valid.Message += "<br/>Não foi possível excluir essa matriz de avaliação, pois existem um ou mais itens vinculados a ela.";
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

        #endregion

        #region Read

        public EvaluationMatrix Get(long id)
        {
            return evaluationMatrixRepository.Get(id);
        }

        public IEnumerable<EvaluationMatrix> Load(ref Pager pager, Guid ent_id)
        {
            return evaluationMatrixRepository.Load(ent_id, ref pager);
        }

        public IEnumerable<EvaluationMatrix> Search(string search, string searchEdition, ref Pager pager, Guid ent_id)
        {
            return evaluationMatrixRepository.Search(search, searchEdition, ent_id, ref pager);
        }

        public IEnumerable<EvaluationMatrix> GetByDiscipline(long idDiscipline)
        {
            return evaluationMatrixRepository.GetByDiscipline(idDiscipline);
        }

        public IEnumerable<EvaluationMatrix> GetByTestDiscipline(long idDiscipline, long idTest, long idSubGroup, int idTcp)
        {
            List<EvaluationMatrix> result = new List<EvaluationMatrix>();
            if (idTcp > 0)
            {
                result.AddRange(evaluationMatrixRepository.GetByDiscipline(idDiscipline).Where(m => m.EvaluationMatrixCourse.Any(c => c.EvaluationMatrixCourseCurriculumGrades.Any(g => g.CurriculumGradeId == idTcp))));
            }
            else
            {
                var curriculumGrade = idTest > 0 ? testCurriculumGradeBusiness.GetCurricumGradeByTest_Id(idTest) : testCurriculumGradeBusiness.GetDistinctCurricumGradeByTestSubGroup_Id(idSubGroup);

                if (curriculumGrade != null && curriculumGrade.Count() > 0)
                    foreach (var cg in curriculumGrade)
                    {
                        var matrix = evaluationMatrixRepository.GetByDiscipline(idDiscipline).Where(m => m.EvaluationMatrixCourse.Any(c => c.EvaluationMatrixCourseCurriculumGrades.Any(g => g.CurriculumGradeId == cg.tcp_id)));

                        if (matrix.Count() > 0)
                            result.AddRange(matrix);
                    }
            }

            return result;
        }

        public IEnumerable<EvaluationMatrix> GetByMatriz(long idMatriz)
        {
            return evaluationMatrixRepository.GetByMatriz(idMatriz);
        }

        public IEnumerable<EvaluationMatrix> GetComboByDiscipline(long idDiscipline)
        {
            return evaluationMatrixRepository.GetComboByDiscipline(idDiscipline);
        }

        public IEnumerable<EvaluationMatrix> LoadCombo(Guid entityId)
        {
            return evaluationMatrixRepository.LoadCombo(entityId);
        }

        public IEnumerable<EvaluationMatrix> LoadComboSimple(Guid entityId, long? typeLevelEducation = null)
        {
            return evaluationMatrixRepository.LoadComboSimple(entityId, typeLevelEducation);
        }

        public List<State> LoadComboSituation()
        {
            return (Enum.GetValues(typeof(EnumState)).Cast<EnumState>().Select(
                state => new State() { Id = (Byte)state, Description = EnumExtensions.GetDescription(state) }).Where(a => a.Description != EnumExtensions.GetDescription(EnumState.excluido)).ToList());
        }

        public EvaluationMatrix GetGradeByMatrix(int Id)
        {
            return evaluationMatrixRepository.GetGradeByMatrix(Id);
        }

        public bool ExistsItemMatrix(long idMatrix)
        {
            return evaluationMatrixRepository.ExistsItemMatrix(idMatrix);
        }

        public EvaluationMatrix LoadUpdate(long evaluationMatrixId)
        {
            return evaluationMatrixRepository.LoadUpdate(evaluationMatrixId);
        }

        public IEnumerable<AJX_Select2> LoadMatrizByDiscipline(string description, string discipline, Guid EntityId)
        {
            return evaluationMatrixRepository.LoadMatrizByDiscipline(description, discipline, EntityId);
        }

        #endregion

        #region Write

        public EvaluationMatrix Save(EvaluationMatrix entity)
        {
            entity.Validate = Validate(entity, ValidateAction.Save, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity = evaluationMatrixRepository.Save(entity);
                entity.Validate.Type = ValidateType.Save.ToString();
                entity.Validate.Message = "Matriz de avaliação salva com sucesso.";
            }

            return entity;
        }

        public EvaluationMatrix Update(long id, EvaluationMatrix entity)
        {
            entity.Validate = Validate(entity, ValidateAction.Update, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity.Id = id;
                entity = evaluationMatrixRepository.Update(entity);
                entity.Validate.Type = ValidateType.Save.ToString();
                entity.Validate.Message = "Matriz de avaliação alterada com sucesso.";
            }

            return entity;
        }

        public EvaluationMatrix Delete(long id)
        {
            EvaluationMatrix entity = new EvaluationMatrix { Id = id };
            entity.Validate = Validate(entity, ValidateAction.Delete, entity.Validate);
            if (entity.Validate.IsValid)
            {
                evaluationMatrixRepository.Delete(entity);
                entity.Validate.Type = ValidateType.Delete.ToString();
                entity.Validate.Message = "Matriz de avaliação excluída com sucesso.";
            }

            return entity;
        }

        #endregion
    }
}
