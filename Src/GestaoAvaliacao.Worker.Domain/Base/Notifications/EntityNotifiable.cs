using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Worker.Domain.Base.Notifications
{
    public abstract class EntityNotifiable
    {
        public EntityNotifiable()
        {
            Errors = new List<string>();
        }

        [NotMapped]
        public List<string> Errors { get; private set; }

        public void AddError(string error) => Errors.Add(error);

        public void AddError(IEnumerable<string> errors) => Errors.AddRange(errors);

        public void AddError(Exception ex) => Errors.Add(ex.InnerException?.Message ?? ex.Message);
    }
}