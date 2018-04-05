using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using GestaoEscolar.IRepository;
using System.Collections.Generic;

namespace GestaoEscolar.Business
{
    public class ACA_TipoNivelEnsinoBusiness : IACA_TipoNivelEnsinoBusiness
    {
        private readonly IACA_TipoNivelEnsinoRepository levelEducationRepository;

        public ACA_TipoNivelEnsinoBusiness(IACA_TipoNivelEnsinoRepository levelEducationRepository)
        {
            this.levelEducationRepository = levelEducationRepository;
        }

        #region Read

        public IEnumerable<ACA_TipoNivelEnsino> Load()
        {
          return levelEducationRepository.Load();
        }

        public ACA_TipoNivelEnsino Get(long id)
        {
            ACA_TipoNivelEnsino result = levelEducationRepository.Get(id);
            if (result != null)
                return result;
            else
                return new ACA_TipoNivelEnsino();
        }

        public object GetCustom(long id)
        {
            ACA_TipoNivelEnsino result = levelEducationRepository.Get(id);
            if (result != null)
            {
                return new
                {
                    Id = result.tne_id,
                    Description = result.tne_nome
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
