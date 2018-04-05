using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using GestaoEscolar.Entities;
using GestaoEscolar.IRepository;

namespace GestaoAvaliacao.Business
{
    public class AnswerSheetBatchBusiness : IAnswerSheetBatchBusiness
	{
		private readonly IAnswerSheetBatchRepository batchRepository;
		private readonly IESC_EscolaRepository escolaRepository;
		private readonly ITUR_TurmaRepository turmaRepository;

        public AnswerSheetBatchBusiness(IAnswerSheetBatchRepository batchRepository, ITUR_TurmaRepository turmaRepository, IESC_EscolaRepository escolaRepository)
		{
			this.batchRepository = batchRepository;
			this.turmaRepository = turmaRepository;
			this.escolaRepository = escolaRepository;
        }

		#region Read

		public AnswerSheetBatch Get(long id)
		{
			return batchRepository.Get(id);
		}

		public AnswerSheetBatch GetSimple(long id)
		{
			return batchRepository.GetSimple(id);
		}

		public AnswerSheetBatch Find(AnswerSheetBatchFilter filter)
		{
			return batchRepository.Find(filter);
		}

		public AnswerSheetBatchResult GetSchoolSectionInformation(int? SchoolId, long? SectionId)
		{
			AnswerSheetBatchResult result = new AnswerSheetBatchResult();
			result.Validate = new Validate();
			result.SchoolId = SchoolId != null ? (int)SchoolId : 0;
			result.SectionId = SectionId != null ? (long)SectionId : 0;

			if (result.SchoolId > 0 || result.SectionId > 0)
			{
				result = batchRepository.GetSchoolSectionInformation(result.SchoolId, result.SectionId);
				if (result != null)
				{
					result.Validate = new Validate
					{
						IsValid = true
					};
				}
				else
				{
					result.Validate = new Validate
					{
						IsValid = false,
						Type = ValidateType.error.ToString(),
						Message = "Erro ao buscar informação da escola e/ou turma."
					};
				}
			}
			else
			{
				result.Validate = new Validate
				{
					IsValid = false,
					Type = ValidateType.error.ToString(),
					Message = "A escola ou turma não foi informada."
				};
			}

			return result;
		}

        public AnswerSheetBatchResult GetSectionInformation(long SectionId)
        {
            AnswerSheetBatchResult result = new AnswerSheetBatchResult { Validate = new Validate() };
            TUR_Turma turma = turmaRepository.GetWithTurno(SectionId);

            if (turma != null)
            {
                result.SectionId = turma.tur_id;
                ESC_Escola escola = escolaRepository.Get(turma.esc_id);
                if (escola != null)
                {
                    result.SchoolId = escola.esc_id;
                    if (escola.uad_idSuperiorGestao.HasValue)
                    {
                        result.SupAdmUnitId = escola.uad_idSuperiorGestao.Value;
                    }
                    result.SectionName = string.Format("{0} - {1}", turma.tur_codigo, turma.ACA_TipoTurno.ttn_nome);
                    result.SectionId = turma.tur_id;
                    result.Validate.IsValid = true;
                }
                else {
                    result.Validate.IsValid = false;
                    result.Validate.Type = ValidateType.error.ToString();
                    result.Validate.Message = "Erro ao buscar informação da escola e/ou turma.";
                }
            }
            else {
                result.Validate.IsValid = false;
                result.Validate.Type = ValidateType.error.ToString();
                result.Validate.Message = "Erro ao buscar informação da escola e/ou turma.";
            }

            return result;
        }

		public long GetStudentId(long SectionId, int? mtu_numeroChamada, long? alu_id)
		{
			return batchRepository.GetStudentId(SectionId, mtu_numeroChamada, alu_id);
		}

        #endregion

        #region Write

        public AnswerSheetBatch Save(AnswerSheetBatch entity)
        {
            entity = batchRepository.Save(entity);
            entity.Validate.Type = ValidateType.Save.ToString();
            entity.Validate.Message = "Lote de folhas de respostas salvo com sucesso.";

            return entity;
        }

        public AnswerSheetBatch Update(long Id, AnswerSheetBatch entity)
        {
            entity.Id = Id;
            batchRepository.Update(entity);
            entity.Validate.Type = ValidateType.Update.ToString();
            entity.Validate.Message = "Lote de folhas de respostas alterado com sucesso.";

            return entity;
		}

        public AnswerSheetBatch UpdateOwnerEntities(long Id, AnswerSheetBatch entity)
        {
            entity.Id = Id;
            batchRepository.UpdateOwnerEntities(entity);
            entity.Validate.Type = ValidateType.Update.ToString();
            entity.Validate.Message = "Lote de folhas de respostas alterado com sucesso.";

            return entity;
        }

		#endregion
	}
}
