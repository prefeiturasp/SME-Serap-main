using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class NumberItemsAplicationTai : EntityBase
    {
        public virtual string Name { get; set; }
        public virtual string Value { get; set; }
        public virtual bool AdvanceWithoutAnswering { get; set; }
 
        public virtual bool BackToPreviousItem { get; set; }
    }
}
