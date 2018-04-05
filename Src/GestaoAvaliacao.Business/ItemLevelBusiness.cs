using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class ItemLevelBusiness : IItemLevelBusiness
    {
        private readonly IItemLevelRepository itemLevelRepository;

        public ItemLevelBusiness(IItemLevelRepository itemLevelRepository)
        {
            this.itemLevelRepository = itemLevelRepository;
        }

        #region Custom

        private Validate Validate(ItemLevel entity, ValidateAction action, Guid ent_id, Validate valid)
        {
            valid.Message = null;
            if (action == ValidateAction.Save)
            {
                if (entity == null || string.IsNullOrEmpty(entity.Description) || (entity.Value < 0))
                    valid.Message = "Não foram preenchidos todos os campos obrigatórios.";

                if (entity.Value <= 0)
                    valid.Message += "<br/>Somente é aceito nível superior a 0 (zero)";

                if (itemLevelRepository.ExistsValue(entity.Value, ent_id))
                    valid.Message += "<br/>Já existe um nível de dificuldade do item com esse valor cadastrado.";
            }

            if (action == ValidateAction.Update)
            {                
                if (entity == null)
                {
                    valid.Message = "Não foi encontrado o nível de dificuldade do item a ser atualizado.";
                    valid.Code = 404;
                }

                if (string.IsNullOrEmpty(entity.Description) || (entity.Value < 0))
                    valid.Message += "<br/>Não foram preenchidos todos os campos obrigatórios.";

                if (itemLevelRepository.ExistsValueAlter(entity.Value, entity.Id, ent_id))
                    valid.Message += "<br/>Já existe um nível de dificuldade do item com esse valor cadastrado.";
            }

            if (action == ValidateAction.Delete)
            {
                if (entity == null)
                {
                    valid.Message = "Não foi encontrado o nível de dificuldade do item a ser excluído.";
                    valid.Code = 404;
                }

                if (itemLevelRepository.ExistsTestType(entity.Id, ent_id))
                    valid.Message += "<br/>Não é possível excluir o nível de dificuldade do item, pois existe(m) tipos de provas relacionado(s) a ele.";
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

        public ItemLevel Get(long id)
        {
            return itemLevelRepository.Get(id);
        }

        public IEnumerable<ItemLevel> Search(string search, ref Pager pager, Guid EntityId)
        {
            return itemLevelRepository.Search(search, ref pager, EntityId);
        }

        public IEnumerable<ItemLevel> Load(ref Pager pager, Guid EntityId)
        {
            return itemLevelRepository.Load(ref pager, EntityId);
        }

        public IEnumerable<ItemLevel> LoadLevels(Guid EntityId)
        {
            return itemLevelRepository.LoadLevels(EntityId);
        }

        #endregion

        #region Write

        public ItemLevel Save(ItemLevel entity, Guid ent_id)
        {
            entity.Validate = Validate(entity, ValidateAction.Save, ent_id, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity.EntityId = ent_id;
                entity = itemLevelRepository.Save(entity);
                entity.Validate.Type = ValidateType.Save.ToString();
                entity.Validate.Message = "Nível de dificuldade do item salvo com sucesso.";
            }

            return entity;
        }

        public ItemLevel Update(long id, ItemLevel entity, Guid ent_id)
        {
            entity.Validate = Validate(entity, ValidateAction.Update, ent_id, entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity.Id = id;
                itemLevelRepository.Update(entity);
                entity.Validate.Type = ValidateType.Update.ToString();
                entity.Validate.Message = "Nível de dificuldade do item alterado com sucesso.";
            }

            return entity;
        }

        public ItemLevel Delete(long id, Guid ent_id)
        {
            ItemLevel entity = new ItemLevel { Id = id };
            entity.Validate = Validate(entity, ValidateAction.Delete, ent_id, entity.Validate);
            if (entity.Validate.IsValid)
            {
                itemLevelRepository.Delete(entity);
                entity.Validate.Type = ValidateType.Delete.ToString();
                entity.Validate.Message = "Nível de dificuldade do item excluído com sucesso.";
            }

            return entity;
        }

        #endregion
    }
}
