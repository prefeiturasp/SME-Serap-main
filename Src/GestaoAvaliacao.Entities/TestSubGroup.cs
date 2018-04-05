using GestaoAvaliacao.Entities.Base;
using System;

namespace GestaoAvaliacao.Entities
{
    public class TestSubGroup : EntityBase
    {
        public virtual string Description { get; set; }

        public virtual int Order { get; set; }

        public virtual TestGroup TestGroup { get; set; }        
    }
}
