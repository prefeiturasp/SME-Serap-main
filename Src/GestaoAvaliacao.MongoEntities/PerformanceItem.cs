using GestaoAvaliacao.MongoEntities.Attribute;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.MongoEntities
{
    [CollectionName("ReportItem")]
	public class PerformanceItem : EntityBase
	{
		public PerformanceItem()
		{

		}

		public PerformanceItem(Guid uad_id, long test_id, int esc_id, int tne_id, int crp_ordem)
		{
			this._id = string.Format("{0}_{1}_{2}_{3}_{4}", uad_id, test_id, esc_id, tne_id, crp_ordem);
		}
		public string AdministrativeUnityId { get; set; }
		public long TestId { get; set; }
		public int SchoolId { get; set; }

		public int QtStudents { get; set; }

		public Dictionary<long, int> ItemsValues { get; set; }
		public EnumReportTypeEntity TypeEntity { get; set; }
	}
}
