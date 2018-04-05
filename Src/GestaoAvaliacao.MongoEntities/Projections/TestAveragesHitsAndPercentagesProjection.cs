using GestaoAvaliacao.MongoEntities.DTO;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.MongoEntities.Projections
{
	public class TestAveragesHitsAndPercentagesProjection
	{
		private double? avgSME;
		private double? avgDRE;
		private double? avgESC;
		private double? avgTeam;
		private double? avgHitsSME;
		private double? avgHitsDRE;
		private double? avgHitsESC;
		private double? avgHitsTeam;

		public List<TestAverageTeamResult> avgTeams = new List<TestAverageTeamResult>();

		public long? Test_Id { get; set; }

		public double? AvgSME
		{
			get { return RoundValues(avgSME); }
			set { avgSME = value; }
		}

		public double? AvgDRE
		{
			get { return RoundValues(avgDRE); }
			set { avgDRE = value; }
		}

		public double? AvgESC
		{
			get { return RoundValues(avgESC); }
			set { avgESC = value; }
		}

		public double? AvgTeam
		{
			get { return RoundValues(avgTeam); }
			set { avgTeam = value; }
		}
		public List<TestAverageTeamResult> AvgTeams
		{
			get { return avgTeams; }
			set { avgTeams = value; }
		}

		public double? AvgHitsSME
		{
			get { return RoundValues(avgHitsSME); }
			set { avgHitsSME = value; }
		}

		public double? AvgHitsDRE
		{
			get { return RoundValues(avgHitsDRE); }
			set { avgHitsDRE = value; }
		}

		public double? AvgHitsESC
		{
			get { return RoundValues(avgHitsESC); }
			set { avgHitsESC = value; }
		}

		public double? AvgHitsTeam
		{
			get { return RoundValues(avgHitsTeam); }
			set { avgHitsTeam = value; }
		}


		public double? RoundValues(double? value)
		{
			return value.HasValue ? (double?)Math.Round((double)value, 2) : null;

		}
	}

}
