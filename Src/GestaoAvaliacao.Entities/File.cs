using GestaoAvaliacao.Entities.Base;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Entities
{
    public class File : EntityBase
    {
        public File()
        {
            this.TestFiles = new List<TestFiles>();
            ItemFiles = new List<ItemFile>();
        }

        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string ContentType { get; set; }
        public string Path { get; set; }
        public long OwnerId { get; set; }
        public byte OwnerType { get; set; }
        public long ParentOwnerId { get; set; }
        public Guid? CreatedBy_Id { get; set; }
        public Guid? DeletedBy_Id { get; set; }
        public float? HorizontalResolution { get; set; }
        public float? VerticalResolution { get; set; }
        public virtual List<TestFiles> TestFiles { get; set; }
        public virtual List<ItemFile> ItemFiles { get; set; }
    }
}
