using System;

namespace GestaoAvaliacao.Entities.DTO
{
    public class DeficiencyDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool Selected { get; set; }
    }
}