using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IBaseTextRepository
    {
        BaseText Save(BaseText entity);
        void Update(BaseText entity);
        BaseText Get(long id);
        IEnumerable<BaseText> Load(ref Pager pager);
        void Delete(long id);
    }
}
