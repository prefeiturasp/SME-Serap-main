using GestaoAvaliacao.Entities.Base;
using System;

namespace GestaoAvaliacao.Entities
{
    public class TestFiles : EntityBase
    {
        public virtual File File { get; set; }
        public long File_Id { get; set; }

        public virtual Test Test { get; set; }
        public long Test_Id { get; set; }

        public Guid? UserId { get; set; }
    }
}
