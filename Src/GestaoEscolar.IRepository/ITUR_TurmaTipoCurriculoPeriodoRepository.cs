using System.Collections.Generic;

namespace GestaoEscolar.IRepository
{
    public interface ITUR_TurmaTipoCurriculoPeriodoRepository
	{
		IEnumerable<int> GetYearsBySchool(int esc_id);
	}
}
