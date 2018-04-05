using GestaoEscolar.Entities;
using GestaoEscolar.Entities.DTO;
using System;
using System.Collections.Generic;

namespace GestaoEscolar.IRepository
{
    public interface IACA_TipoCurriculoPeriodoRepository
	{
		IEnumerable<ACA_TipoCurriculoPeriodo> GetSimple(int esc_id);
        ACA_TipoCurriculoPeriodo Get(int tcp_id);
        IEnumerable<ACA_TipoCurriculoPeriodo> Load();
        IEnumerable<ACA_TipoCurriculoPeriodo> GetAllTypeCurriculumGrades();
        IEnumerable<ACA_TipoCurriculoPeriodo> LoadByLevelEducationModality(int tne_id, int tme_id);
        string GetDescription(int tcp_id, int tne_id, int tme_id, int tcp_ordem);
        int GetId(int tne_id, int tme_id, int tcp_ordem);
		IEnumerable<TUR_TurmaTipoCurriculoPeriodoDTO> LoadWithNivelEnsino(Guid ent_id, Guid? pes_id = null, IEnumerable<string> esc_id = null, IEnumerable<string> dre_id = null);
		IEnumerable<TUR_TurmaTipoCurriculoPeriodoDTO> LoadWithNivelEnsino();
	}
}
