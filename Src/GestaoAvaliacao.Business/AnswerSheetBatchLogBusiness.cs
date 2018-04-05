using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;

namespace GestaoAvaliacao.Business
{
    public class AnswerSheetBatchLogBusiness : IAnswerSheetBatchLogBusiness
	{
		private readonly IAnswerSheetBatchLogRepository batchLogRepository;

        public AnswerSheetBatchLogBusiness(IAnswerSheetBatchLogRepository batchLogRepository)
		{
            this.batchLogRepository = batchLogRepository;
		}

		#region Custom

        private Validate Validate(Validate valid)
		{
			valid.Message = null;

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

        public AnswerSheetBatchLog Get(long id)
        {
            return batchLogRepository.Get(id);
        }

        public AnswerSheetBatchLog GetByBatchFile_Id(long id)
        {
            return batchLogRepository.GetByBatchFile_Id(id);
        }

		#endregion

		#region Write

        public AnswerSheetBatchLog Save(AnswerSheetBatchLog entity)
        {
            entity.Validate = Validate(entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity = batchLogRepository.Save(entity);
                entity.Validate.Type = ValidateType.Save.ToString();
                entity.Validate.Message = "Log salvo com sucesso.";
            }

            return entity;
        }

        public AnswerSheetBatchLog Update(long Id, AnswerSheetBatchLog entity)
        {
            entity.Validate = Validate(entity.Validate);
            if (entity.Validate.IsValid)
            {
                entity.Id = Id;
                batchLogRepository.Update(entity);
                entity.Validate.Type = ValidateType.Update.ToString();
                entity.Validate.Message = "Log alterado com sucesso.";
            }

            return entity;
        }

		#endregion
	}
}
