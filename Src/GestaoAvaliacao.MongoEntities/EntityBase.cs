using GestaoAvaliacao.Util;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace GestaoAvaliacao.MongoEntities
{
    public class EntityBase
    {
        public EntityBase()
        {
            this._id = Guid.NewGuid().ToString();
            this.Validate = new Validate();
        }

        public virtual string _id { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime UpdateDate { get; set; }
        public virtual byte State { get; set; }

        [BsonIgnore]
        public virtual Validate Validate { get; set; }
    }
}