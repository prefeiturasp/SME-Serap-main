using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IPageConfigurationBusiness
    {
        PageConfiguration Get(long id);
        PageConfiguration Find(long id);
        IEnumerable<PageConfiguration> LoadAll();
        IEnumerable<PageConfiguration> Load(ref Pager pager);
        IEnumerable<PageConfiguration> Search(string search, string category, ref Pager pager);
        PageConfiguration Save(PageConfiguration entity);
        PageConfiguration Update(PageConfiguration entity);
        PageConfiguration Delete(long id);
        bool ExistsModelDescription(long id, string description);
        bool ExistsFeaturedVideo(long id);
        PageConfiguration ObterLinkAdminSeraEstudantes();
        bool VerificaPerfilAcessoAdminSerapEstudantes(Guid grupo);
    }
}
