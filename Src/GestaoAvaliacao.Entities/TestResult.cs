
using GestaoAvaliacao.Util;
using System;
namespace GestaoAvaliacao.Entities
{
	public class TestResult
	{
		public virtual long TestId { get; set; }

		public virtual string TestDescription { get; set; }

		public virtual string TestTypeDescription { get; set; }

		public virtual bool Global { get; set; }

		public virtual string CreateDate { get; set; }
        public virtual string UpdateDate { get; set; }

        public virtual string Discipline { get; set; }

		public virtual int TestFrequencyApplication { get; set; }
		public virtual int TestTypeFrequencyApplication { get; set; }
		public virtual string FrequencyApplicationDescription
		{
			get
			{
				if (TestTypeFrequencyApplication > 0)
				{
					return EnumHelper.GetFrenquencyApplication(TestFrequencyApplication, TestTypeFrequencyApplication, true, false);
				}
				else
				{
					return " - ";
				}
			}
		}

		public virtual string ApplicationStartDate { get; set; }

		public virtual string ApplicationEndDate { get; set; }

		public virtual string CorrectionStartDate { get; set; }

		public virtual string CorrectionEndDate { get; set; }

		public virtual bool Bib { get; set; }

		public virtual bool Desempenho { get; set; }

		public virtual int TestSituation { get; set; }

		public virtual string Path { get; set; }

		public virtual long? ItemType_Id { get; set; }

		public virtual Guid UsuId { get; set; }
		public bool Visible { get; set; }
		public virtual long Order { get; set; }
		public long? TestSubGroup_Id { get; set; }
		public TestGroup TestGroup { get; set; }

        public int TestGroupId { get; set; }
        public int TestSubGroupId { get; set; }
        public int OrderSubGroup { get; set; }
        public string TestGroupName { get; set; }
		public string TestSubGroupName { get; set; }
        public DateTime? TestGroupCreateDate { get; set; }

        public bool AllAdhered { get; set; }
        public bool ShowOnSerapEstudantes { get; set; }
		public bool SynchronizedInSerapStudents { get; set; }

        public bool HasAdhered { get; set; }
    }
}
