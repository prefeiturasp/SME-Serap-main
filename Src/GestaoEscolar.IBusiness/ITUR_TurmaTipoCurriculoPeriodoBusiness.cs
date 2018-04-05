using System.Collections.Generic;

namespace GestaoEscolar.IBusiness
{
    public interface ITUR_TurmaTipoCurriculoPeriodoBusiness
	{
		IEnumerable<int> GetYearsBySchool(int esc_id);
	}
}
