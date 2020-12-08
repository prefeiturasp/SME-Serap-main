using GestaoAvaliacao.Worker.Domain.Base.Notifications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GestaoAvaliacao.Worker.Domain.Base
{
    public class EntityWorkerBase : EntityNotifiable
    {
        public EntityWorkerBase()
            : base()
        {
            CreateDate = DateTime.Now;
            UpdateDate = DateTime.Now;
            State = (byte)EnumState.ativo;
        }

        public virtual long Id { get; set; }

        public virtual DateTime CreateDate { get; set; }

        public virtual DateTime UpdateDate { get; set; }

        public virtual byte State { get; set; }

        public bool IsValid => !Errors?.Any() ?? true;
    }

    public enum EnumState
    {
        [Description("Não definido")]
        naoDefinido = 0,
        [Description("Ativo")]
        ativo = 1,
        [Description("Inativo")]
        inativo = 2,
        [Description("Excluído")]
        excluido = 3
    }
}
