using System.ComponentModel;

namespace GestaoAvaliacao.API.Models
{
    public class PictureModel
    {
        public string Tag { get; set; }
        public string Extension { get; set; }
        public int ContentLength { get; internal set; }
        public string ContentType { get; internal set; }
        public string InputStream { get; internal set; }
        public string FileName { get; internal set; }
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
}
