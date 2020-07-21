using GestaoAvaliacao.Entities.Base;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Entities
{
    public class TestType : EntityBase
	{
		public TestType(){
			TestTypeItemLevel = new List<TestTypeItemLevel>();
			TestTypeCourses = new List<TestTypeCourse>();
		}

		public string Description { get; set; }

		public virtual FormatType FormatType { get; set; }
		public long? FormatType_Id { get; set; }

		public long AnswerSheet_Id { get; set; }

		public virtual List<TestTypeItemLevel> TestTypeItemLevel { get; set; }
		
		public Guid EntityId { get; set; }

        public int FrequencyApplication { get; set; }

		public bool Bib { get; set; }

		public bool Global { get; set; }

		public virtual List<TestTypeCourse> TestTypeCourses { get; set; }

		public int TypeLevelEducationId { get; set; }

		public virtual ItemType ItemType { get; set; }
		public long? ItemType_Id { get; set; }
		public virtual ModelTest ModelTest { get; set; }
		public Nullable<long> ModelTest_Id { get; set; }
        public bool TargetToStudentsWithDeficiencies { get; set; }
		public virtual List<TestTypeDeficiency> TestTypeDeficiencies { get; set; }

		public void AddTestTypeDeficiency(TestTypeDeficiency testTypeDeficiency)
        {
			testTypeDeficiency.TestType = this;
			TestTypeDeficiencies.Add(testTypeDeficiency);
		}
	}
}
