using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
