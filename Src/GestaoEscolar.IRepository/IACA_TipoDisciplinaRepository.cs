using GestaoEscolar.Entities;
using System.Collections.Generic;

namespace GestaoEscolar.IRepository
{
    public interface IACA_TipoDisciplinaRepository
    {
        IEnumerable<ACA_TipoDisciplina> Load(int typeLevelEducation);
        ACA_TipoDisciplina Get(long id);
    }
}
