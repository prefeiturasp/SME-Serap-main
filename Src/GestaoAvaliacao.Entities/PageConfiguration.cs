using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Util;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities
{
    public class PageConfiguration : EntityBase
    {
        public short Category { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ButtonDescription { get; set; }
        public string Link { get; set; }
        public virtual File FileIllustrativeImage { get; set; }
        public long? FileIllustrativeImage_Id { get; set; }
        public virtual File FileVideo { get; set; }
        public long? FileVideo_Id { get; set; }
        public bool Featured { get; set; }

        [NotMapped]
        public string CaminhoIcone { get; set; }

        [NotMapped]
        public string CaminhoVideo { get; set; }
    }
}
