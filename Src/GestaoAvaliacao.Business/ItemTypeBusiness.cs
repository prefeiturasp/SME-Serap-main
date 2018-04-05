using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class ItemTypeBusiness : IItemTypeBusiness
	{
		private readonly IItemTypeRepository itemTypeRepository;

		public ItemTypeBusiness(IItemTypeRepository itemTypeRepository)
		{
			this.itemTypeRepository = itemTypeRepository;
		}

        #region Custom

        private Validate Validate(ItemType entity, ValidateAction action, Guid ent_id, Validate valid)
        {
            if (action == ValidateAction.Update)
            {
                ItemType ent = itemTypeRepository.Get(entity.Id);
                if (ent == null)
                {
                    valid.Message = "Não foi encontrado o tipo de item a ser alterado.";
                    valid.Code = 404;
                }
                else
                {
                    if (string.IsNullOrEmpty(entity.Description))
                        valid.Message = "Não foi preenchido o campo obrigatório.";

                    if (itemTypeRepository.ExistsDescriptionNamed(entity.Description, entity.Id, ent_id))
                        valid.Message += "<br/>Esse tipo de item já existe.";

                    if (entity.IsDefault && !entity.State.Equals(Convert.ToByte(EnumState.ativo)))
                        valid.Message += "<br/>Não é permitido inativar o tipo de item padrão.";

                    if (ent.IsDefault && !entity.IsDefault)
                        valid.Message += "<br/>Altere primeiro o tipo de item que deseja ser o item padrão.";

                    if (!entity.EntityId.Equals(ent.EntityId))
                        valid.Message = "<br/>Não foi encontrado o tipo de item a ser alterado.";
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

        public IEnumerable<ItemType> Search(ref Pager pager, Guid EntityId, string search)
        {
            return itemTypeRepository.Search(ref pager, EntityId, search);
        }

        public IEnumerable<ItemType> FindSimple(Guid EntityId)
        {
            return itemTypeRepository.FindSimple(EntityId);
        }

        public IEnumerable<ItemType> FindForTestType(Guid EntityId)
        {
            return itemTypeRepository.FindForTestType(EntityId);
        }

        public ItemType Get(long id)
        {
            return itemTypeRepository.Get(id);
        }

        #endregion

		#region Write

		public ItemType Update(ItemType entity)
		{
			entity.Validate = Validate(entity, ValidateAction.Update, entity.EntityId, entity.Validate);

			if (entity.Validate.IsValid)
			{
				if (entity.IsDefault)
					VerifyDefault(entity.EntityId);

				itemTypeRepository.Update(entity);

				entity.Validate.Type = ValidateType.Update.ToString();
				entity.Validate.Message = "Tipo de item alterado com sucesso.";
			}

			return entity;
		}

        private void VerifyDefault(Guid guid)
        {
            itemTypeRepository.VerifyDefault(guid);
        }

		#endregion
	}
}
