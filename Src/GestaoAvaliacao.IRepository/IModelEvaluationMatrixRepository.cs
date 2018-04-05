using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IModelEvaluationMatrixRepository
    {
        ModelEvaluationMatrix Get(long id);
        ModelEvaluationMatrix GetModelEvaluationMatrix(long id);
        IEnumerable<ModelEvaluationMatrix> Load(Guid ent_id);
        IEnumerable<ModelEvaluationMatrix> LoadPaginate(ref Pager pager, Guid ent_id);
        ModelEvaluationMatrix Save(ModelEvaluationMatrix entity);
        ModelEvaluationMatrix Update(ModelEvaluationMatrix entity);
        IEnumerable<ModelEvaluationMatrix> Search(ref Pager pager, Guid EntityId, String search = null, int levelqtd = 0);
        void Delete(ModelEvaluationMatrix entity);
        bool ExistsModelDescription(string description, Guid ent_id);
        bool ExistsModelDescriptionUpdate(string description, long Id, Guid ent_id);
		bool IsDeletedModelSkillBeenUsed(ModelEvaluationMatrix entity);
        bool ExistsMatrixRelated(ModelEvaluationMatrix entity);
    }
}
