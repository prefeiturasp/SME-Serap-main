namespace GestaoAvaliacao.MongoEntities.Attribute
{
    public class CollectionNameAttribute: System.Attribute
	{
		public string Name { get; set; }

		public CollectionNameAttribute(string name)
		{
			this.Name = name;
		}
	}
}
