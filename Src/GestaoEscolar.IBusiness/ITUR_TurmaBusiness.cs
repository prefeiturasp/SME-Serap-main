using GestaoAvaliacao.Util;
using GestaoEscolar.Entities;
using GestaoEscolar.Entities.DTO;
using System;
using System.Collections.Generic;
using CoreSSO = MSTech.CoreSSO.Entities;


namespace GestaoEscolar.IBusiness
{
    public interface ITUR_TurmaBusiness
	{
		TUR_Turma Get(long id);
		IEnumerable<TUR_Turma> Load(int esc_id, int ttn_id, Guid ent_id, Guid pes_id, EnumSYS_Visao vis_id);
		int GetTotalSection(CoreSSO.SYS_Usuario user, CoreSSO.SYS_Grupo grupo);
		TUR_Turma GetWithTurno(long tur_id);
        bool ValidateTeacherSection(long tur_id, Guid pes_id);
		IEnumerable<TUR_Turma> LoadByGrade(int esc_id, int ttn_id, Guid ent_id, Guid pes_id, EnumSYS_Visao vis_id, IEnumerable<int> years);
        TUR_TurmaDTO GetWithTurnoAndModality(long tur_id);

    }
}
