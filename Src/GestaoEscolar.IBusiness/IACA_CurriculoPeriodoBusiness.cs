using GestaoEscolar.Entities;
using System;
using System.Collections.Generic;

namespace GestaoEscolar.IBusiness
{
    public interface IACA_CurriculoPeriodoBusiness
	{
		IEnumerable<ACA_CurriculoPeriodo> Load(Guid ent_id, int cur_id);
		ACA_CurriculoPeriodo Get(long id);
		object GetCustom(long id);
		IEnumerable<ACA_CurriculoPeriodo> GetCurriculumGradeByesc_id(int esc_id);
	}
}
