using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IEvaluationMatrixBusiness
	{
		EvaluationMatrix Save(EvaluationMatrix entity);
		EvaluationMatrix Update(long id, EvaluationMatrix entity);
		EvaluationMatrix Get(long id);
		EvaluationMatrix Delete(long id);
		IEnumerable<EvaluationMatrix> Load(ref Pager pager, Guid ent_id);
		IEnumerable<EvaluationMatrix> Search(string search, string searchEdition, ref Pager pager, Guid ent_id);
		IEnumerable<EvaluationMatrix> GetByDiscipline(long idDiscipline);
        IEnumerable<EvaluationMatrix> GetByTestDiscipline(long idDiscipline, long idTest, long idSubGroup,int idTcp);
        IEnumerable<EvaluationMatrix> LoadCombo(Guid entityId);
		EvaluationMatrix LoadUpdate(long evaluationMatrixId);
		EvaluationMatrix GetGradeByMatrix(int Id);
		List<State> LoadComboSituation();
		bool ExistsItemMatrix(long idMatrix);
		IEnumerable<EvaluationMatrix> GetComboByDiscipline(long idDiscipline);
		IEnumerable<EvaluationMatrix> LoadComboSimple(Guid entityId, long? typeLevelEducation = null);
        IEnumerable<AJX_Select2> LoadMatrizByDiscipline(string description, string discipline, Guid EntityId);
        IEnumerable<EvaluationMatrix> GetByMatriz(long idMatriz);

    }
}
