namespace GestaoAvaliacao.MongoEntities.DTO
{
    public class TestAveragesPercentagesDTO
    {
        public long? Test_id { get; set; }
        public double? AvgSME { get; set; }
        public double? AvgDRE { get; set; }
        public double? AvgESC { get; set; }
        public double? AvgTeam { get; set; }
    }
}
