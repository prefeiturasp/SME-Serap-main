using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using GestaoEscolar.IRepository;
using System.Collections.Generic;

namespace GestaoEscolar.Business
{
    public class ACA_TipoModalidadeEnsinoBusiness : IACA_TipoModalidadeEnsinoBusiness
    {
        private readonly IACA_TipoModalidadeEnsinoRepository modalityRepository;

        public ACA_TipoModalidadeEnsinoBusiness(IACA_TipoModalidadeEnsinoRepository modalityRepository)
        {
            this.modalityRepository = modalityRepository;
        }

        #region Read

        public IEnumerable<ACA_TipoModalidadeEnsino> Load()
        {
            return modalityRepository.Load();
        }

        public ACA_TipoModalidadeEnsino Get(long id)
        {
            ACA_TipoModalidadeEnsino result = modalityRepository.Get(id);
            if (result != null)
                return result;
            else
                return new ACA_TipoModalidadeEnsino();
        }

        public object GetCustom(long id)
        {
            ACA_TipoModalidadeEnsino result = modalityRepository.Get(id);
            if (result != null)
            {
                return new
                {
                    Id = result.tme_id,
                    Description = result.tme_nome
                };
            }
            else
            {
                return new
                {
                    Id = 0,
                    Description = string.Empty
                };
            }
        }

        #endregion
    }
}
