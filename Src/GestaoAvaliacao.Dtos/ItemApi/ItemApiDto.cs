using System.Collections.Generic;
using System.ComponentModel;

namespace GestaoAvaliacao.Dtos.ItemApi
{
    public class ItemApiDto
    {
        public int ItemCodeVersion { get; set; }
        public string Statement { get; set; }
        public string DescriptorSentence { get; set; }
        public int? Proficiency { get; set; }
        public long EvaluationMatrix_Id { get; set; }
        public string Keywords { get; set; }
        public string Tips { get; set; }
        public decimal? TRICasualSetting { get; set; }
        public decimal? TRIDifficulty { get; set; }
        public decimal? TRIDiscrimination { get; set; }
        public BaseTextDto BaseText { get; set; }
        public long ItemSituation_Id { get; set; }
        public long ItemType_Id { get; set; }
        public long? ItemLevel_Id { get; set; }
        public string ItemCode { get; set; }
        public int ItemVersion { get; set; }
        public int TypeCurriculumGradeId { get; set; }
        public int Axle_Id { get; set; }
        public int Ability_Id { get; set; }
        public List<AlternativeDto> Alternatives { get; set; }
        public bool IsRestrict { get; set; }
        public long? KnowledgeArea_Id { get; set; }
        public long? SubSubject_Id { get; set; }

        public List<PictureDto> Pictures { get; set; }
        public List<VideoDto> Videos { get; set; }
    }

    public class BaseTextDto
    {
        public string Description { get; set; }
        public string Source { get; set; }
    }

    public class AlternativeDto
    {
        public string Description { get; set; }

        public bool Correct { get; set; }

        public int Order { get; set; }

        public string Justificative { get; set; }

        public string Numeration { get; set; }
    }

    public class PictureDto
    {
        public string Tag { get; set; }
        public int ContentLength { get; set; }
        public string ContentType { get; set; }
        public string InputStream { get; set; }
        public string FileName { get; set; }
        public PictureType Type { get; set; }
    }

    public enum PictureType
    {
        [Description("Texto_Base")]
        BaseText = 1,
        [Description("Alternativa")]
        Alternative = 2,
        [Description("Justificativa")]
        Justificative = 3,
        [Description("Enunciado")]
        Statement = 4,
        [Description("Thumbnail_Video")]
        ThumbnailVideo = 30,
        [Description("Audio")]
        Audio = 31
    }

    public class VideoDto
    {
        public int ContentLength { get; set; }
        public string ContentType { get; set; }
        public string InputStream { get; set; }
        public string FileName { get; set; }

        public int? ThumbnailContentLength { get; set; }
        public string ThumbnailContentType { get; set; }
        public string ThumbnailInputStream { get; set; }
        public string ThumbnailFileName { get; set; }
    }
}
