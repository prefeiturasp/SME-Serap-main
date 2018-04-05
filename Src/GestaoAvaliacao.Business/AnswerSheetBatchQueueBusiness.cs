using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GestaoAvaliacao.Business
{
    public class AnswerSheetBatchQueueBusiness : IAnswerSheetBatchQueueBusiness
    {
        private readonly IAnswerSheetBatchQueueRepository answerSheetBatchQueueRepository;
        private readonly IAnswerSheetBatchFilesRepository answerSheetBatchFilesRepository;


        public AnswerSheetBatchQueueBusiness(IAnswerSheetBatchQueueRepository answerSheetBatchQueueRepository, IAnswerSheetBatchFilesRepository answerSheetBatchFilesRepository)
        {
            this.answerSheetBatchQueueRepository = answerSheetBatchQueueRepository;
            this.answerSheetBatchFilesRepository = answerSheetBatchFilesRepository;
        }
        public AnswerSheetBatchQueue Save(AnswerSheetBatchQueue entity)
        {
            entity = answerSheetBatchQueueRepository.Save(entity);
            entity.Validate.Type = ValidateType.Save.ToString();
            entity.Validate.Message = "Arquivo salvo na fila de descompactação com sucesso.";
            return entity;
        }

        public AnswerSheetBatchQueue Update(long Id, AnswerSheetBatchQueue entity)
        {
            entity = answerSheetBatchQueueRepository.Update(Id, entity);
            entity.Validate.Type = ValidateType.Update.ToString();
            entity.Validate.Message = "Arquivo atualizado com sucesso.";
            return entity;
        }

        public void UpdateZipProcessing(AnswerSheetBatchQueue entity)
        {
            answerSheetBatchQueueRepository.UpdateZipProcessing(entity);
        }

        public IEnumerable<AnswerSheetBatchQueueResult> Search(AnswerSheetBatchQueueFilter filter, ref Pager pager)
        {
            EnumSYS_Visao visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), filter.VisionId.ToString());
            IEnumerable<string> uads = null;
            switch (visao)
            {
                case EnumSYS_Visao.Gestao:
                case EnumSYS_Visao.UnidadeAdministrativa:
                    uads = GetUads(filter);
                    break;
                default:
                    break;
            }
            filter.uads = uads;
            return answerSheetBatchQueueRepository.Search(filter, ref pager);
        }
        public IEnumerable<AnswerSheetBatchQueue> GetAnswerSheetBatchBySituation(EnumBatchQueueSituation Situation, int rows)
        {
            return answerSheetBatchQueueRepository.GetAnswerSheetBatchBySituation(Situation, rows);
        }
        public IEnumerable<AnswerSheetBatchQueueResult> GetTop(AnswerSheetBatchQueueFilter filter)
        {
            EnumSYS_Visao visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), filter.VisionId.ToString());

            

            IEnumerable<string> uads = null;
            switch (visao)
            {
                case EnumSYS_Visao.Gestao:
                case EnumSYS_Visao.UnidadeAdministrativa:
                    uads = GetUads(filter);
                    break;
                default:
                    break;
            }
            filter.uads = uads;
            return answerSheetBatchQueueRepository.GetTop(filter);
        }

        private IEnumerable<string> GetUads(AnswerSheetBatchQueueFilter filter)
        {
            DataTable dt = MSTech.CoreSSO.BLL.SYS_UsuarioGrupoUABO.GetSelect(filter.UserId, filter.GroupId);

            return dt.AsEnumerable().Select(x => string.Concat("'", x.Field<Guid>("uad_id"), "'"));
        }

        public AnswerSheetBatchQueue Delete(long id, Guid ent_id)
        {
            AnswerSheetBatchQueue entity = new AnswerSheetBatchQueue { Id = id };
            if (entity.Validate.IsValid)
            {
                answerSheetBatchQueueRepository.Delete(entity);
                entity.Validate.Type = ValidateType.Delete.ToString();
                entity.Validate.Message = "Arquivos excluídos com sucesso.";
            }

            return entity;
        }

        public AnswerSheetBatchQueue DeleteError(long id, Guid ent_id)
        {
            AnswerSheetBatchQueue entity = new AnswerSheetBatchQueue { Id = id };
            if (entity.Validate.IsValid)
            {
                IEnumerable<AnswerSheetBatchFiles> listFilesError = answerSheetBatchQueueRepository.SelectFilesError(entity);
                answerSheetBatchFilesRepository.DeleteList(listFilesError.ToList());
                entity.Validate.Type = ValidateType.Delete.ToString();
                entity.Validate.Message = "Arquivos excluídos com sucesso.";
            }

            return entity;
        }
    }
}
