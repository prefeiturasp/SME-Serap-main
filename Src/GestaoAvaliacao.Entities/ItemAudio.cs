using GestaoAvaliacao.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities
{
    public class ItemAudio : EntityBase
    {
        public virtual Item Item { get; set; }

        public virtual File File { get; set; }

        [NotMapped]
        public long ItemFileId { get; set; }

        [NotMapped]
        public string Name { get; set; }

        [NotMapped]
        public string Path { get; set; }

        [NotMapped]
        public long FileId { get; set; }

        [NotMapped]
        public long Item_Id { get; set; }
    }
}
