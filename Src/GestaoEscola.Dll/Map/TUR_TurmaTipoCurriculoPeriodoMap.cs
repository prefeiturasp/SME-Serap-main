using GestaoEscolar.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GestaoEscolar.Repository.Map
{
    public class TUR_TurmaTipoCurriculoPeriodoMap : EntityTypeConfiguration<TUR_TurmaTipoCurriculoPeriodo>
	{
		public TUR_TurmaTipoCurriculoPeriodoMap()
		{
			ToTable("TUR_TurmaTipoCurriculoPeriodo");
			HasKey(p => new { p.tur_id, p.cur_id, p.tme_id, p.tne_id, p.crp_ordem });

		}
	}
}
