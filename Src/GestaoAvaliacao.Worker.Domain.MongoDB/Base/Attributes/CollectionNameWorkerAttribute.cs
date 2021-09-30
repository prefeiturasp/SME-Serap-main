using System;

namespace GestaoAvaliacao.Worker.Domain.MongoDB.Base.Attributes
{
    public class CollectionNameWorkerAttribute : Attribute
    {
        public string Name { get; set; }

        public CollectionNameWorkerAttribute(string name)
        {
            this.Name = name;
        }
    }
}