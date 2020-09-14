using GestaoAvaliacao.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities
{
    public class ItemFile : EntityBase
    {
        public virtual Item Item { get; set; }

        public virtual File File { get; set; }

        public virtual File ConvertedFile { get; set; }
        public long? ConvertedFile_Id { get; set; }

        public virtual File Thumbnail { get; set; }    
        public long? Thumbnail_Id { get; set; }

        [NotMapped]
        public long ItemFileId { get; set; }

        [NotMapped]
        public string Name { get; set; }

        [NotMapped]
        public string Path { get; set; }

        [NotMapped]
        public long FileId { get; set; }

        [NotMapped]
        public long ThumbnailId { get; set; }

        [NotMapped]
        public string ThumbnailName { get; set; }

        [NotMapped]
        public string ThumbnailPath { get; set; }

        [NotMapped]
        public long Item_Id { get; set; }

        [NotMapped]
        public long? ConvertedFileId { get; set; }

        [NotMapped]
        public string ConvertedFileName { get; set; }

        [NotMapped]
        public string ConvertedFilePath { get; set; }
    }
}
