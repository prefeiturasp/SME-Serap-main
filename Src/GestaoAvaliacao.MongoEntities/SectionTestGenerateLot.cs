using GestaoAvaliacao.MongoEntities.Attribute;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.MongoEntities
{
    [CollectionName("SectionTestGenerateLot")]
    public class SectionTestGenerateLot : EntityBase
    {
        public long test_id { get; set; }
        public int esc_id { get; set; }
        public long tur_id { get; set; }
        public List<long> alu_ids { get; set; }
    }
}
