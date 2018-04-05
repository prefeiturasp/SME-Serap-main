namespace GestaoAvaliacao.API.Models
{
    public class CorrectionModel
	{
		public long alu_id { get; set; }
		public long alternative_id { get; set; }
		public long item_id { get; set; }
		public bool n { get; set; }
		public bool r { get; set; }
		public bool manual { get; set; }
	}
}