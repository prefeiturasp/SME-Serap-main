using GestaoEscolar.IBusiness;
using GestaoEscolar.IRepository;
using System.Collections.Generic;

namespace GestaoEscolar.Business
{
    public class TUR_TurmaTipoCurriculoPeriodoBusiness : ITUR_TurmaTipoCurriculoPeriodoBusiness
	{
		#region Dependences
		readonly ITUR_TurmaTipoCurriculoPeriodoRepository turmaTipoCurriculoPeriodoRepository;

		public TUR_TurmaTipoCurriculoPeriodoBusiness(ITUR_TurmaTipoCurriculoPeriodoRepository turmaTipoCurriculoPeriodoRepository)
		{
			this.turmaTipoCurriculoPeriodoRepository = turmaTipoCurriculoPeriodoRepository;
		}
		#endregion
		public IEnumerable<int> GetYearsBySchool(int esc_id)
		{
			return turmaTipoCurriculoPeriodoRepository.GetYearsBySchool(esc_id);
		}
	}
}
