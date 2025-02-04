﻿using GestaoAvaliacao.Entities.Base;
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
            TestContexts = new List<TestContext>();
            BlockChains = new List<BlockChain>();
            Blocks = new List<Block>();
            NumberItemsTestTai = new List<NumberItemTestTai>();
        }

		public string Description { get; set; }
        public string Password { get; set; }

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

        public virtual List<TestContext> TestContexts { get; set; }
        public virtual List<BlockChain> BlockChains { get; set; }
        public virtual List<Block> Blocks { get; set; }
        public virtual List<NumberItemTestTai> NumberItemsTestTai { get; set; }

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

        public bool ShowTestContext { get; set; }

        public bool TestTai { get; set; }
        public bool ProvaComProficiencia { get; set; }
        public bool ApresentarResultados { get; set; }
        public bool ApresentarResultadosPorItem { get; set; }
        public int? NumberSynchronizedResponseItems { get; set; }

        [NotMapped]
        public bool AdvanceWithoutAnswering { get; set; }
        [NotMapped]
        public bool BackToPreviousItem { get; set; }
        [NotMapped]
        public NumberItemsAplicationTai NumberItemsAplicationTai { get; set; }
        [NotMapped]
        public bool RemoveBlockChain { get; set; }
        [NotMapped]
        public bool RemoveBlockChainBlock { get; set; }
        [NotMapped]
        public bool RemoveTaiCurriculumGrade { get; set; }

        public DateTime? DownloadStartDate { get; set; }
        public bool? BlockChain { get; set; }
        public int? BlockChainNumber { get; set; }
        public int? BlockChainItems { get; set; }
        public int? BlockChainForBlock { get; set; }
    }
}
