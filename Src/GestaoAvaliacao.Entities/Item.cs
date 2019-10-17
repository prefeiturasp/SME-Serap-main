using GestaoAvaliacao.Entities.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities
{
    public class Item : EntityBase
	{
		public Item ()
		{
			ItemSkills = new List<ItemSkill>();
			Alternatives = new List<Alternative>();
			ItemCurriculumGrades = new List<ItemCurriculumGrade>();
			IsRestrict = false;
			BlockItems = new List<BlockItem>();
            ItemFiles = new List<ItemFile>();
            ItemAudios = new List<ItemAudio>();
        }

        public Item ShalowCopy()
        {
            return (Item)this.MemberwiseClone();
        }

        public string ItemCode { get; set; }
        public int ItemCodeVersion { get; set; }
        public int ItemVersion { get; set; }
        public bool LastVersion { get; set; }
        public string Statement { get; set; }
        public string Keywords { get; set; }
        public string Tips { get; set; }
        public decimal? TRIDiscrimination { get; set; }
        public decimal? TRIDifficulty { get; set; }
        public decimal? TRICasualSetting { get; set; }
        public bool IsRestrict { get; set; }
        public virtual BaseText BaseText { get; set; }
        public long? BaseText_Id { get; set; }
        public virtual EvaluationMatrix EvaluationMatrix { get; set; }
        public long EvaluationMatrix_Id { get; set; }
        public virtual ItemLevel ItemLevel { get; set; }
        public long? ItemLevel_Id { get; set; }
        public virtual ItemSituation ItemSituation { get; set; }
        public long ItemSituation_Id { get; set; }
        public virtual ItemType ItemType { get; set; }
        public long ItemType_Id { get; set; }
        public int? proficiency { get; set; }
        public string descriptorSentence { get; set; }
        public virtual List<ItemSkill> ItemSkills { get; set; }
        public virtual List<Alternative> Alternatives { get; set; }
        public virtual List<ItemCurriculumGrade> ItemCurriculumGrades { get; set; }
        public virtual List<BlockItem> BlockItems { get; set; }
        public bool? ItemNarrated { get; set; }
        public bool? StudentStatement { get; set; }
        public bool? NarrationStudentStatement { get; set; }
        public bool? NarrationAlternatives { get; set; }
        public bool? Revoked { get; set; }
        public virtual KnowledgeArea KnowledgeArea { get; set; }
        public long? KnowledgeArea_Id { get; set; }
        public virtual SubSubject SubSubject { get; set; }
        public long? SubSubject_Id { get; set; }
        public virtual List<ItemFile> ItemFiles { get; set; }
        public virtual List<ItemAudio> ItemAudios { get; set; }

        [NotMapped]
        public int ItemOrder { get; set; }
        [NotMapped]
        public string KnowledgeArea_Description { get; set; }
        [NotMapped]
        public int KnowledgeArea_Order { get; set; }
    }
}