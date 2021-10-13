using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities
{
    [Serializable]
    public class Test : EntityBase
	{
        public Test()
        {
            TestCurriculumGrades = new List<TestCurriculumGrade>();
            TestPerformanceLevels = new List<TestPerformanceLevel>();
            TestItemLevels = new List<TestItemLevel>();
            Visible = true;
        }

		public string Description { get; set; }

		public TestType TestType { get; set; }

		public long TestType_Id { get; set; }

		public Discipline Discipline { get; set; }

        public long? Discipline_Id { get; set; }

		public bool Bib { get; set; }

		public int NumberItemsBlock { get; set; }

		public int NumberBlock { get; set; }

		public FormatType FormatType { get; set; }

        public long FormatType_Id { get; set; }

		public int? NumberItem { get; set; }

		public DateTime ApplicationStartDate { get; set; }

		public DateTime ApplicationEndDate { get; set; }
        

        [NotMapped]
        public bool ApplicationActiveOrDone => TestSituation != EnumTestSituation.Pending && DateTime.Today >= ApplicationStartDate;

        public DateTime CorrectionStartDate { get; set; }

		public DateTime CorrectionEndDate { get; set; }

		public Guid UsuId { get; set; }

        public int FrequencyApplication { get; set; }

		public EnumTestSituation TestSituation { get; set; }

		public virtual List<TestCurriculumGrade> TestCurriculumGrades { get; set; }

		public virtual List<TestPerformanceLevel> TestPerformanceLevels { get; set; }

		public virtual List<TestItemLevel> TestItemLevels { get; set; }

		public bool AllAdhered { get; set; }

        public bool PublicFeedback { get; set; }

        public bool ProcessedCorrection { get; set; }

        public DateTime? ProcessedCorrectionDate { get; set; }

        [DefaultValue(true)]
        public bool Visible { get; set; }

        public bool Multidiscipline { get; set; }

        public TestSubGroup TestSubGroup { get; set; }

        public long? TestSubGroup_Id { get; set; }

        public TestTime TestTime { get; set; }

        public long? TestTime_Id { get; set; }

        [DefaultValue(0)]
        public long Order { get; set; }

        [DefaultValue(false)]
        public bool KnowledgeAreaBlock { get; set; }

        [DefaultValue(false)]
        public bool ElectronicTest { get; set; }

        [NotMapped]
        public int quantDiasRestantes { get; set; }

        public bool ShowVideoFiles { get; set; }
        public bool ShowAudioFiles { get; set; }
        public bool ShowJustificate { get; set; }
        public bool ShowOnSerapEstudantes { get; set; }
        public bool IsSerapEstudantesBIB { get; set; }
        public int SerapEstudantesBIBQuantity { get; set; }

    }
}
