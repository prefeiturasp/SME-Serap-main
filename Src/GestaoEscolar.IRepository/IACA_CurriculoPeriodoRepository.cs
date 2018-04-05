using GestaoEscolar.Entities;
using System;
using System.Collections.Generic;

namespace GestaoEscolar.IRepository
{
    public interface IACA_CurriculoPeriodoRepository
	{
		IEnumerable<ACA_CurriculoPeriodo> Load(int cur_id, Guid ent_id);
		ACA_CurriculoPeriodo Get(long id);
		IEnumerable<ACA_CurriculoPeriodo> GetCurriculumGradeByesc_id(int esc_id);
	}
}
