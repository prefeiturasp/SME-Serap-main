using GestaoAvaliacao.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface IReportStudiesBusiness
    {
        ReportStudies Save(ReportStudies entity);
        IEnumerable<ReportStudies> ListAll();
        void Delete(long id);
    }
}
