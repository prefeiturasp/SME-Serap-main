using GestaoEscolar.Entities;
using GestaoEscolar.Entities.DTO;
using System;
using System.Collections.Generic;

namespace GestaoEscolar.IRepository
{
    public interface ITUR_TurmaRepository
	{
		IEnumerable<TUR_Turma> Load(int esc_id, int ttn_id, Guid? pes_id = null, Guid? ent_id = null);
		int GetTotalSection(Guid ent_id, IEnumerable<string> uad_id = null);
		TUR_Turma Get(long tur_id);
		TUR_Turma GetWithTurno(long tur_id);
        bool ValidateTeacherSection(long tur_id, Guid pes_id);
		IEnumerable<TUR_Turma> LoadByGrade(int esc_id, int ttn_id, IEnumerable<int> years, Guid? pes_id = null, Guid? ent_id = null);
        TUR_TurmaDTO GetWithTurnoAndModality(long tur_id);

    }
}
