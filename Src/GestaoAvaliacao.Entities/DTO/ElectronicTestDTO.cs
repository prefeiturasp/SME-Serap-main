using GestaoAvaliacao.Util;
using System;

namespace GestaoAvaliacao.Entities.DTO
{
    public class ElectronicTestDTO
    {
        public virtual long Id { get; set; }
        public string Description { get; set; }
        public int? NumberItem { get; set; }
        public int quantDiasRestantes { get; set; }
        public long alu_id { get; set; }
        public long tur_id { get; set; }
        public int FrequencyApplication { get; set; }
        public string FrequencyApplicationText => GetFrequenciyApplicationDescription();
        public DateTime ApplicationEndDate { get; set; }
        public long TestTypeId { get; set; }
        public bool TargetToStudentsWithDeficiencies { get; set; }

        private string GetFrequenciyApplicationDescription()
        {
            if (FrequencyApplication <= 0) return string.Empty;
            return EnumHelper.GetDescriptionFromEnumValue((EnumFrenquencyApplication)FrequencyApplication);
        }

        public int esc_id { get; set; }
        public string esc_nome { get; set; }

        public string Disciplina { get; set; }
    }
}