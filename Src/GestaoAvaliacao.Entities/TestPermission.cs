using GestaoAvaliacao.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities
{
    public class TestPermission : EntityBase
    {
        public Guid gru_id { get; set; }
        public virtual Test Test { get; set; }
        public long Test_Id { get; set; }
        public bool AllowAnswer { get; set; }
        public bool ShowResult { get; set; }
        public bool TestHide { get; set; }

        [NotMapped]
        public string gru_nome { get; set; }
    }
}
