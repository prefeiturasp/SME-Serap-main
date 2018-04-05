using GestaoAvaliacao.Util;
using System;

namespace GestaoAvaliacao.Entities
{
    public class AdherenceTest
	{
		public Guid UsuId { get; set; }
		public string TestDescription { get; set; }
        public virtual int TestFrequencyApplication { get; set; }
        public virtual int TestTypeFrequencyApplication { get; set; }
        public virtual string FrequencyApplicationDescription
        {
            get
            {
                if (TestTypeFrequencyApplication > 0)
                {
                    return EnumHelper.GetFrenquencyApplication(TestFrequencyApplication, TestTypeFrequencyApplication, true, true);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string DisciplineDescription { get; set; }
		public long Id { get; set; }
		public bool  AllAdhered { get; set; }
        public bool Global { get; set; }
        public bool AnswerSheetBlocked { get; set; }
		public DateTime ApplicationStartDate { get; set; }
		public DateTime ApplicationEndDate { get; set; }
	}
}
