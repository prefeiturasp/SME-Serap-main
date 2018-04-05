using GestaoAvaliacao.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities
{
    public class AdministrativeUnitType : EntityBase
    {
        public Guid AdministrativeUnitTypeId { get; set; }

        public string Name { get; set; }
    }
}
