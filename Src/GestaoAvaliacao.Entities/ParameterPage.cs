using GestaoAvaliacao.Entities.Base;
using System;

namespace GestaoAvaliacao.Entities
{
    public class ParameterPage : EntityBase
    {
        public virtual string Description { get; set; }

        public virtual Boolean pageVersioning { get; set; }

        public virtual Boolean pageObligatory { get; set; } 
    }
}
