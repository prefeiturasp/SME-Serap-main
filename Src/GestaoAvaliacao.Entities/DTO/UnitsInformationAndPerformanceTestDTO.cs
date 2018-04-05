namespace GestaoAvaliacao.Entities.DTO
{
    public class UnitsInformationAndPerformanceTestDTO
    {
        public string Esc_nome { get; set; }
        public string Uad_nome { get; set; }
        public string Tur_codigo { get; set; }
        public string TestDescription { get; set; }
        public string TestDiscipline { get; set; }        
        public double? AvgSME { get; set; }
        public double? AvgDRE { get; set; }
        public double? AvgESC { get; set; }
        public double? AvgTeam { get; set; }
        public double? AvgHitsSME { get; set; }
        public double? AvgHitsDRE { get; set; }
        public double? AvgHitsESC { get; set; }
        public double? AvgHitsTeam { get; set; }
    }
}
