namespace GestaoAvaliacao.Entities.Projections
{
    public class ItemWithSkillAndAlternativeProjection
    {
        public long Item_Id { get; set; }
        public int Order { get; set; }
        public string CorrectAlternativeNumeration { get; set; }
        public string SkillCode { get; set; }
        public string SkillDescription { get; set; }
        public bool Revoked { get; set; }
    }
}