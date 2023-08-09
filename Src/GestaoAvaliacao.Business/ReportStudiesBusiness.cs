using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business
{
        public class ReportStudiesBusiness : IReportStudiesBusiness
        {
            private readonly IReportStudiesRepository reportStudiesRepository;

            public ReportStudiesBusiness(IReportStudiesRepository reportStudiesRepository)
            {
                this.reportStudiesRepository = reportStudiesRepository;
            }

        private Validate Validate(ReportStudies entity, long evaluationMatrixId, ValidateAction action, Validate valid)
        {
            valid.Message = null;

            if (action == ValidateAction.Save)
            {
                if (entity == null || string.IsNullOrEmpty(entity.Name) || string.IsNullOrEmpty(entity.Addressee))
                    valid.Message = "Não foram preenchidos todos os campos obrigatórios.";

                // Adicione outras validações específicas para a entidade ReportStudies, se necessário.
            }

            //if (action == ValidateAction.Delete)
            //{
            //    ReportStudies ent = Get(entity.Id);
            //    if (ent == null)
            //    {
            //        valid.Message = "Não foi encontrado o registro a ser excluído.";
            //        valid.Code = 404;
            //    }

            //    // Adicione as condições de validação para a ação de exclusão.
            //}

            if (!string.IsNullOrEmpty(valid.Message))
            {
                string br = "<br/>";
                valid.Message = valid.Message.TrimStart(br.ToCharArray());

                valid.IsValid = false;

                if (valid.Code <= 0)
                    valid.Code = 400;

                valid.Type = ValidateType.alert.ToString();
            }
            else
            {
                valid.IsValid = true;
            }

            return valid;
        }
        public ReportStudies Save(ReportStudies entity)
        {
            return reportStudiesRepository.Save(entity);
        }

        public IEnumerable<ReportStudies> ListAll()
        {
            return reportStudiesRepository.ListAll();
        }

        public void Delete(long id)
        {
            reportStudiesRepository.Delete(id);
        }
    }
}
