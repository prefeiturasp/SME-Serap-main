using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IReportStudiesRepository
    {
        bool Save(ReportStudies entity);
        IEnumerable<ReportStudies> ListAll();
        IEnumerable<ReportStudies> ListPaginated(ref Pager pager);
        IEnumerable<ReportStudies> ListWithFilter(string searchFilter);
        bool DeleteById(long id);
        void Delete(long id);
        ReportStudies GetById(long id);
        bool Update(ReportStudies entity);
    }
}
