using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.Util;
using System;

namespace GestaoAvaliacao.Entities.Base
{
    public abstract class EntityBase
    {
        public EntityBase()
        {
            CreateDate = DateTime.Now;
            UpdateDate = DateTime.Now;
            State = (byte)EnumState.ativo;
            Validate = new Validate();
        }

        public virtual long Id { get; set;}

        public virtual DateTime CreateDate { get; set; }

        public virtual DateTime UpdateDate { get; set; }

        public virtual byte State { get; set; }

        public Validate Validate { get; set; }
    }
}
