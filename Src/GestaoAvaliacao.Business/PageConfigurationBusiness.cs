using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class PageConfigurationBusiness : IPageConfigurationBusiness
    {
        private readonly IPageConfigurationRepository pageConfigurationRepository;

        public PageConfigurationBusiness(IPageConfigurationRepository pageConfigurationRepository)
        {
            this.pageConfigurationRepository = pageConfigurationRepository;
        }

        #region Custom

        private Validate Validate(PageConfiguration entity, ValidateAction action, Validate valid)
        {
            valid.Message = null;
            if (action == ValidateAction.Save || action == ValidateAction.Update)
            {
                if (entity.Category == (short)PageConfigurationCategory.Video && entity.Featured && ExistsFeaturedVideo(entity.Id))
                    valid.Message += "<br/>Já existe um vídeo cadastrado como destaque.";
            }
            else if (action == ValidateAction.Delete)
            {
                PageConfiguration ent = Get(entity.Id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrada a configuração da página a ser excluída.";
                    valid.Code = 404;
                }
            }

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
                valid.IsValid = true;

            return valid;
        }

        #endregion

        #region Read

        public PageConfiguration Get(long id)
        {
            return pageConfigurationRepository.Get(id);
        }
        public PageConfiguration Find(long id)
        {
            return pageConfigurationRepository.Find(id);
        }

        public IEnumerable<PageConfiguration> LoadAll()
        {
            return pageConfigurationRepository.LoadAll();
        }

        public IEnumerable<PageConfiguration> Load(ref Pager pager)
        {
            return pageConfigurationRepository.Load(ref pager);
        }

        public IEnumerable<PageConfiguration> Search(string search, string category, ref Pager pager)
        {
            return pageConfigurationRepository.Search(search, category, ref pager);
        }

        public bool ExistsModelDescription(long id, string description)
        {
            return pageConfigurationRepository.ExistsModelDescription(id, description);
        }

        public bool ExistsFeaturedVideo(long id)
        {
            return pageConfigurationRepository.ExistsFeaturedVideo(id);
        }

        #endregion

        #region Write

        public PageConfiguration Save(PageConfiguration entity)
        {
            entity.Validate = Validate(entity, ValidateAction.Save, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity = pageConfigurationRepository.Save(entity);
                entity.Validate.Type = ValidateType.Save.ToString();
                entity.Validate.Message = "Configuração da página salva com sucesso.";
            }

            return entity;
        }

        public PageConfiguration Update(PageConfiguration entity)
        {
            entity.Validate = Validate(entity, ValidateAction.Update, entity.Validate);
            if (entity.Validate.IsValid)
            {
                pageConfigurationRepository.Update(entity);
                entity.Validate.Type = ValidateType.Update.ToString();
                entity.Validate.Message = "Configuração da página alterada com sucesso.";
            }

            return entity;
        }

        public PageConfiguration Delete(long id)
        {
            PageConfiguration entity = new PageConfiguration { Id = id };
            entity.Validate = Validate(entity, ValidateAction.Delete, entity.Validate);
            if (entity.Validate.IsValid)
            {
                pageConfigurationRepository.Delete(entity);
                entity.Validate.Type = ValidateType.Delete.ToString();
                entity.Validate.Message = "Configuração da página excluída com sucesso.";
            }

            return entity;
        }

        #endregion
    }
}
