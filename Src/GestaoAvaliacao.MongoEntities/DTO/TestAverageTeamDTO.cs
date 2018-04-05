using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.MongoEntities.DTO
{
	public class TestAverageTeamDTO
	{
		private Guid? dre_id { get; set; }
		private int? esc_id { get; set; }
		private long? tur_id { get; set; }
        private string tur_codigo { get; set; }
        private long? test_id { get; set; }
		private long discipline_Id { get; set; }
        private int totalStudents { get; set; }
        private int numberAnswers { get; set; }
        private int totalItems { get; set; }
		private int totalCorretItems { get; set; }
		private List<TestAverageTeamResult> lResultSME = new List<TestAverageTeamResult>();

		public TestAverageTeamDTO(List<TestAverageTeamResult> SME, Guid? dre, int? esc, long? tur)
		{
			lResultSME = SME;
			dre_id = dre;
			esc_id = esc;
			tur_id = tur;
        }


		//public TestAverageTeamDTO(Guid? Dre_id, int? Esc_id, long? Tur_id, int TotalItems, int TotalCorretItems)
		//{
		//	dre_id = Dre_id;
		//	esc_id = Esc_id;
		//	tur_id = Tur_id;
		//	totalItems = TotalItems;
		//	totalCorretItems = TotalCorretItems;
		//}
		//public TestAverageTeamDTO(Guid? Dre_id, int? Esc_id, long? Tur_id)
		//{
		//	dre_id = Dre_id;
		//	esc_id = Esc_id;
		//	tur_id = Tur_id;
		//}

		public int TotalCorretItems
		{
			get { return totalCorretItems; }
			set { totalCorretItems = value; }
		}
		public int TotalItems
		{
			get { return totalItems; }
			set { totalItems = value; }
		}

        public int TotalStudents
        {
            get { return totalStudents; }
            set { totalStudents = value; }
        }

        public int NumberAnswers
        {
            get { return numberAnswers; }
            set { numberAnswers = value; }
        }

        public Guid? Dre_id
		{
			get { return dre_id; }
			set { dre_id = value; }
		}

		public int? Esc_id
		{
			get { return esc_id; }
			set { esc_id = value; }
		}

		public long? Tur_id
		{
			get { return tur_id; }
			set { tur_id = value; }
		}


		public List<TestAverageTeamResult> lResultDre
		{
			get
			{
				if (dre_id == null)
					return lResultSME;
				return lResultSME.FindAll(p => dre_id != null && p.Dre_id == dre_id);
			}

		}
		public List<TestAverageTeamResult> lResultSchool
		{
			get
			{
				if (dre_id == null && esc_id == null)
					return lResultSME;
				else if (dre_id != null && esc_id == null)
					return lResultDre;
				else
					return lResultSME.FindAll(p => dre_id != null && esc_id != null && p.Dre_id == dre_id && p.Esc_id == esc_id);
			}
		}
		public List<TestAverageTeamResult> lResultTeam
		{
			get
			{
				if (dre_id == null && esc_id == null && tur_id == null)
					return lResultSME;
				else if (dre_id != null && esc_id == null && tur_id == null)
					return lResultDre;
				else if (dre_id != null && esc_id != null && tur_id == null)
					return lResultSchool;
				else if (dre_id == null && esc_id == null && tur_id != null)
					return lResultSME.FindAll(p => tur_id != null && p.Tur_id == tur_id);
				else if (dre_id != null && esc_id == null && tur_id != null)
					return lResultSME.FindAll(p => dre_id != null && tur_id != null && p.Dre_id == dre_id && p.Tur_id == tur_id);
				else
					return lResultSME.FindAll(p => dre_id != null && esc_id != null && tur_id != null && p.Dre_id == dre_id && p.Esc_id == esc_id && p.Tur_id == tur_id);
			}
		}
		public TestAverageTeamResult resultSME
		{
			get
			{
				return new TestAverageTeamResult
				{
					Dre_id = dre_id,
					Esc_id = esc_id,
					Tur_id = tur_id,
                    TotalStudents = lResultSME.Sum(p => p.TotalStudents),
                    NumberAnswers = lResultSME.First().NumberAnswers,
                    TotalItems = lResultSME.Sum(p => p.TotalItems),
					TotalCorretItems = lResultSME.Sum(p => p.TotalCorretItems)
				};
			}

		}
		public TestAverageTeamResult resultDRE
		{
			get
			{
				return new TestAverageTeamResult
				{
					Dre_id = dre_id,
					Esc_id = esc_id,
					Tur_id = tur_id,
                    TotalStudents = lResultDre.Sum(p => p.TotalStudents),
                    NumberAnswers = lResultDre.First().NumberAnswers,
                    TotalItems = lResultDre.Sum(p => p.TotalItems),
					TotalCorretItems = lResultDre.Sum(p => p.TotalCorretItems)
				};
			}
		}

		public TestAverageTeamResult resultSchool
		{
			get
			{
				return new TestAverageTeamResult
				{
					Dre_id = dre_id,
					Esc_id = esc_id,
					Tur_id = tur_id,
                    TotalStudents = lResultSchool.Sum(p => p.TotalStudents),
                    NumberAnswers = lResultSchool.First().NumberAnswers,
                    TotalItems = lResultSchool.Sum(p => p.TotalItems),
					TotalCorretItems = lResultSchool.Sum(p => p.TotalCorretItems)
				};
			}
		}
		public TestAverageTeamResult resultTeam
		{
			get
			{
                return new TestAverageTeamResult
                {
                    Dre_id = dre_id,
                    Esc_id = esc_id,
                    Tur_id = tur_id,
                    TotalStudents = lResultTeam.First().TotalStudents,
                    NumberAnswers = lResultTeam.First().NumberAnswers,
                    TotalItems = lResultTeam.First().TotalItems,
                    TotalCorretItems = lResultTeam.First().TotalCorretItems
				};
			}
		}
		public List<TestAverageTeamResult> resultTeams
		{
			get
			{
				return lResultTeam
							.GroupBy(l => new { l.Tur_id, l.Test_id })
							.Select(cl => new TestAverageTeamResult
							{
								Dre_id = cl.First().Dre_id,
								Esc_id = cl.First().Esc_id,
								Tur_id = cl.First().Tur_id,
                                Test_id = cl.First().Test_id,
                                TotalStudents = cl.First().TotalStudents,
                                NumberAnswers = cl.First().NumberAnswers,
                                TotalItems = cl.Sum(c => c.TotalItems),
								TotalCorretItems = cl.Sum(c => c.TotalCorretItems)
							}).ToList();
			}
		}
	}
}
