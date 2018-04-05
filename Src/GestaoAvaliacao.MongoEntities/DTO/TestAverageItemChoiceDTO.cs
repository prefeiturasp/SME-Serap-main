using System.Collections.Generic;

namespace GestaoAvaliacao.MongoEntities.DTO
{
    public class TestAverageItemChoiceDTO
    {
        public List<Alternatives> alternatives { get; set; }        
        public List<Itens> itens { get; set; }
        public long? Item_id { get; set; }
    }

    public class Itens
    {
        public string correctAlternative { get; set; }
        public List<ChosenAlternatives> chosenAlternatives { get; set; }
    }

    public class ChosenAlternatives
    {
        public string name { get; set; }
        public decimal percentageChoice { get; set; }
    }

    public class Alternatives
    {
        public string description { get; set; }
    }
}
