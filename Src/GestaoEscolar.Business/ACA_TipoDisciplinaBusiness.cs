using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using GestaoEscolar.IRepository;
using System.Collections.Generic;

namespace GestaoEscolar.Business
{
    public class ACA_TipoDisciplinaBusiness : IACA_TipoDisciplinaBusiness
    {
        private readonly IACA_TipoDisciplinaRepository disciplineTypeRepository;

        public ACA_TipoDisciplinaBusiness(IACA_TipoDisciplinaRepository disciplineTypeRepository)
        {
            this.disciplineTypeRepository = disciplineTypeRepository;
        }

        #region Read

        public IEnumerable<ACA_TipoDisciplina> Load(int typeLevelEducation)
        {
            return disciplineTypeRepository.Load(typeLevelEducation);
        }

        public ACA_TipoDisciplina Get(long id)
        {
            ACA_TipoDisciplina result = disciplineTypeRepository.Get(id);
            if (result != null)
                return result;
            else
                return new ACA_TipoDisciplina();
        }

        public object GetCustom(long id)
        {
            ACA_TipoDisciplina result = disciplineTypeRepository.Get(id);
            if (result != null)
            {
                return new
                {
                    Id = result.tds_id,
                    Description = result.tds_nome
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
