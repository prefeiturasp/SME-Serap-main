using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Entities.DTO.Abstractions
{
    public class NotificableDto
    {
        public NotificableDto()
        {
            Errors = new List<string>();
        }

        public List<string> Errors { get; private set; }

        public void AddError(string error) => Errors.Add(error);

        public void AddError(IEnumerable<string> errors) => Errors.AddRange(errors);

        public void AddError(ValidationResult validationResult) => Errors.AddRange(validationResult.Errors.Select(x => x.ErrorMessage));

        public void AddError(Exception ex) => Errors.Add(ex.InnerException?.Message ?? ex.Message);

        public bool IsValid => !Errors?.Any() ?? true;
    }
}