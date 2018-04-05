using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IPageConfigurationRepository
    {
        PageConfiguration Get(long id);
        PageConfiguration Find(long id);
        IEnumerable<PageConfiguration> LoadAll();
        IEnumerable<PageConfiguration> Load(ref Pager pager);
        IEnumerable<PageConfiguration> Search(string search, string category, ref Pager pager);
        bool ExistsModelDescription(long id, string description);
        PageConfiguration Save(PageConfiguration entity);
        PageConfiguration Update(PageConfiguration entity);
        void Delete(PageConfiguration entity);
        bool ExistsFeaturedVideo(long id);
    }
}
