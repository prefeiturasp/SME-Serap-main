namespace GestaoAvaliacao.Entities.DTO
{
    public class AdherenceDTO
	{
		public long tur_id { get; set; }
		public string dre_nome { get; set; }
        public string dre_sigla { get; set; }
        public string esc_nome { get; set; }
		public int esc_id { get; set; }
		public string tur_codigo { get; set; }
		public string ttn_nome { get; set; }
	}

	public class AdheredEntityDTO
	{
		public string EntityId { get; set; }
		public string Description { get; set; }
        public long Test_Id { get; set; }
	}
}
