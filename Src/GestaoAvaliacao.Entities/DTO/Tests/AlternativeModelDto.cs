namespace GestaoAvaliacao.Entities.DTO.Tests
{
    public class AlternativeModelDto
    {
        public long Id { get; set; }
        public virtual string Description { get; set; }

        public virtual int Order { get; set; }

        public virtual string Justificative { get; set; }

        public virtual string Numeration { get; set; }

        public long Item_Id { get; set; }

        public bool Selected { get; set; }

        public string NumerationSem { get; set; }
    }
}