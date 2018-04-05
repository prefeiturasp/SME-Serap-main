
using GestaoEscolar.Entities;
using GestaoEscolar.Entities.DTO;
using System.Collections.Generic;
namespace GestaoEscolar.IBusiness
{
    public interface IACA_TipoCurriculoPeriodoBusiness
	{
		IEnumerable<ACA_TipoCurriculoPeriodo> GetSimple(int esc_id);
        ACA_TipoCurriculoPeriodo Get(int tcp_id);
        IEnumerable<ACA_TipoCurriculoPeriodo> Load();
        IEnumerable<ACA_TipoCurriculoPeriodo> GetAllTypeCurriculumGrades();
        IEnumerable<ACA_TipoCurriculoPeriodo> LoadByLevelEducationModality(int tne_id, int tme_id);
        string GetDescription(int tcp_id, int tne_id, int tme_id, int tcp_ordem);
        int GetId(int tne_id, int tme_id, int tcp_ordem);
		IEnumerable<TUR_TurmaTipoCurriculoPeriodoDTO> LoadWithNivelEnsino();
		IEnumerable<TUR_TurmaTipoCurriculoPeriodoDTO> LoadWithNivelEnsino(MSTech.CoreSSO.Entities.SYS_Usuario user, MSTech.CoreSSO.Entities.SYS_Grupo group);
	}
}
