using System.Collections.Generic;

namespace GestaoAvaliacao.Entities.DTO
{
    public class ItemPercentageChoiceByAlternativeWithOrderResult
    {
        public class Test
        {
            public Test()
            {
                Alternatives = new List<Alternative>();
                Items = new List<Item>();
                DreAndSchoolInformation = new DreAndSchoolInformation();
            }
            public long Test_Id { get; set; }
            public List<Item> Items { get; set; }
            public List<Alternative> Alternatives { get; set; }
            public DreAndSchoolInformation DreAndSchoolInformation { get; set; }
        }

        public class DreAndSchoolInformation
        {
            public string SchoolName { get; set; }
            public string DreName { get; set; }
        }

        public class Item
        {
            public Item()
            {
                Alternatives = new List<Alternative>();
            }
            public long Item_Id { get; set; }
            public int Order { get; set; }
            public bool Revoked { get; set; }
            public List<Alternative> Alternatives { get; set; }

        }

        public class Alternative
        {
            public long Alternative_Id { get; set; }
            public int Order { get; set; }
            public bool Correct { get; set; }
            public string Numeration { get; set; }
            public double Avg { get; set; }
        }
    }
}
