using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface IReportStudiesBusiness
    {
        bool Save(ReportStudies entity);
        IEnumerable<ReportStudies> ListAll();
        IEnumerable<ReportStudies> ListPaginated(ref Pager pager, string searchFilter);
        void Delete(long id);
        bool DeleteById(long id);
        Validate Validate(ReportStudies entity, long evaluationMatrixId, ValidateAction action, Validate valid);
    }
}
