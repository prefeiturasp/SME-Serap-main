using GestaoAvaliacao.Entities.Base;
using System;

namespace GestaoAvaliacao.Entities
{
    public class Parameter : EntityBase
	{
		public virtual string Key { get; set; }

		public virtual string Value { get; set; }

		public virtual string Description { get; set; }

		public virtual Guid EntityId { get; set; }

		public virtual DateTime StartDate { get; set; }

		public virtual DateTime? EndDate { get; set; }

		public virtual Boolean? Obligatory { get; set; }

		public virtual Boolean? Versioning { get; set; }

		public virtual ParameterPage ParameterPage { get; set; }
		public virtual long ParameterPage_Id { get; set; }

		public virtual ParameterCategory ParameterCategory { get; set; }

		public virtual ParameterType ParameterType { get; set; }
	}
}
