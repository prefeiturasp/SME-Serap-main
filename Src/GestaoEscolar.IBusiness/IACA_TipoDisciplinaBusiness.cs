using GestaoEscolar.Entities;
using System.Collections.Generic;

namespace GestaoEscolar.IBusiness
{
    public interface IACA_TipoDisciplinaBusiness
	{
        IEnumerable<ACA_TipoDisciplina> Load(int typeLevelEducation);
        ACA_TipoDisciplina Get(long id);
        object GetCustom(long id);
	}
}
