using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.MongoEntities.DTO
{
    public class TestAveragePerformanceDTO
    {
        public Guid DreId { get; set; }
        public string DreName { get; set; }
        public int EscId { get; set; }
        public string EscName { get; set; }
        public double Media { get; set; }
    }

    public class TestAverageViewModel
    {
        public TestAveragePerformanceDTO MediaSME { get; set; }
        public IEnumerable<TestAveragePerformanceDTO> lista { get; set; }
        public bool success { get; set; }
    }
}
