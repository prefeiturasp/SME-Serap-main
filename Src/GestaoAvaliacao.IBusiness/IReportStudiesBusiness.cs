using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Util;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GestaoAvaliacao.IBusiness
{
    public interface IReportStudiesBusiness
    {
        bool Save(ReportStudies entity, UploadModel upload);
        IEnumerable<ReportStudies> ListAll();
        IEnumerable<ReportStudies> ListPaginated(ref Pager pager, string searchFilter);
        void Delete(long id);
        bool DeleteById(long id);
        Validate Validate(ReportStudies entity, long evaluationMatrixId, ValidateAction action, Validate valid);
        void ImportCsv(HttpPostedFileBase arquivo, SYS_Usuario usuario, SYS_Grupo sysGrupo, out CsvImportDTO retornoCsv);
    }
}
