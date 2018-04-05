using MongoDB.Bson.Serialization;
using System;

namespace GestaoAvaliacao.MongoEntities
{
    public class SectionTestGenerator : IIdGenerator
	{
		public object GenerateId(object container, object document)
		{
			throw new NotImplementedException();
		}

		public bool IsEmpty(object id)
		{
			throw new NotImplementedException();
		}
	}
}
