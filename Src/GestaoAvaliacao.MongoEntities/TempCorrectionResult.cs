using GestaoAvaliacao.MongoEntities.Attribute;
using System;

namespace GestaoAvaliacao.MongoEntities
{
    [CollectionName("TempCorrectionResult")]
    public class TempCorrectionResult : EntityBase
    {
        
        public long Test_id { get; set; }
        public long Tur_id { get; set; }
        public bool Processed { get; set; }

        public TempCorrectionResult()
        {

        }

        public TempCorrectionResult(Guid ent_id, long test_id, long tur_id)
        {
            _id = string.Format("{0}_{1}_{2}", ent_id, test_id, tur_id);
            Tur_id = tur_id;
            Test_id = test_id;
            Processed = false;
        }
    }
}
