using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GestaoAvaliacao.MongoEntities.DTO
{
    public class ItemAverageDTO
    {
        [BsonRepresentation(BsonType.Int64, AllowTruncation = true)]
        public long Item_Id { get; set; }

        [BsonRepresentation(BsonType.Int32, AllowTruncation = true)]
        public int TotalItensSME { get; set; }

        [BsonRepresentation(BsonType.Int32, AllowTruncation = true)]
        public int TotalItensCorretosSME { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public double MediaSME { get; set; }

        [BsonRepresentation(BsonType.Int32, AllowTruncation = true)]
        public int TotalItensDRE { get; set; }

        [BsonRepresentation(BsonType.Int32, AllowTruncation = true)]
        public int TotalItensCorretosDRE { get; set; }

        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public double MediaDRE { get; set; }

        [BsonRepresentation(BsonType.Boolean, AllowTruncation = true)]
        public bool Correct { get; set; }
    }
}
