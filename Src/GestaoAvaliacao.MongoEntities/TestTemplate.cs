using GestaoAvaliacao.MongoEntities.Attribute;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.MongoEntities
{
    [CollectionName("TestTemplate")]
	public class TestTemplate : EntityBase
	{
		public TestTemplate()
		{
			this.Items = new List<Item>();
		}

		public TestTemplate(Guid ent_id, long test_id)
		{
			this.Items = new List<Item>();
			this._id = string.Format("{0}_{1}", ent_id, test_id);
		}

		public long Test_Id { get; set; }

		public List<Item> Items { get; set; }
	}

	public class Item
	{
		public int Order { get; set; }
		public long Item_Id { get; set; }
		public long Alternative_Id { get; set; }
		public string Numeration { get; set; }
		public bool Revoked { get; set; }
    }
}
