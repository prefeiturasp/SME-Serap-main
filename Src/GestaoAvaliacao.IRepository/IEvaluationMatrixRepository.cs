using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IEvaluationMatrixRepository
	{
		EvaluationMatrix Save(EvaluationMatrix entity);
		EvaluationMatrix Update(EvaluationMatrix entity);
		EvaluationMatrix Get(long id);
		IEnumerable<EvaluationMatrix> Load(Guid ent_id, ref Pager pager);
		void Delete(EvaluationMatrix entity);
		IEnumerable<EvaluationMatrix> Search(string search, string searchEdition, Guid ent_id, ref Pager pager);
		bool ExistsItemMatrix(long idMatrix);
		IEnumerable<EvaluationMatrix> GetByDiscipline(long idDiscipline);
        IEnumerable<EvaluationMatrix> LoadCombo(Guid entityId);
		EvaluationMatrix LoadUpdate(long evaluationMatrixId);
		EvaluationMatrix GetGradeByMatrix(int Id);
		IEnumerable<EvaluationMatrix> GetComboByDiscipline(long idDiscipline);
		IEnumerable<EvaluationMatrix> LoadComboSimple(Guid entityId, long? typeLevelEducation = null);
        IEnumerable<AJX_Select2> LoadMatrizByDiscipline(string description, string discipline, Guid EntityId);
        IEnumerable<EvaluationMatrix> GetByMatriz(long idMatriz);

    }
}
