using System.Collections.Generic;

namespace GestaoAvaliacao.Entities.DTO.Tests
{
    public class TestModelDto
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public int? NumberItem { get; set; }
        public int QuantDiasRestantes { get; set; }
        public string FrequencyApplication { get; set; }
        public string ApplicationEndDate { get; set; }
        public IEnumerable<ItemModelDto> Itens { get; set; }
    }
}