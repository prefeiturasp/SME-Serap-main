using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IModelEvaluationMatrixBusiness
    {
        ModelEvaluationMatrix Get(long id);
        ModelEvaluationMatrix GetModelEvaluationMatrix(long id);
        IEnumerable<ModelEvaluationMatrix> Load(Guid ent_id);
        IEnumerable<ModelEvaluationMatrix> LoadPaginate(ref Pager pager, Guid ent_id);
        IEnumerable<ModelEvaluationMatrix> Search(ref Pager pager,  Guid ent_id, string search = null, int levelQntd = 0);
        ModelEvaluationMatrix Save(ModelEvaluationMatrix entity, Guid ent_id);
        ModelEvaluationMatrix Update(ModelEvaluationMatrix entity, Guid ent_id);
        ModelEvaluationMatrix Delete(long id, Guid ent_id);
    }
}
