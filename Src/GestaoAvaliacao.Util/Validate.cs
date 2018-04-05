using System.ComponentModel;

namespace GestaoAvaliacao.Util
{
    public class Validate
	{
		public Validate()
		{
			IsValid = true;
		}

		[DefaultValue(true)]
		public bool IsValid { get; set; }

		public int Code { get; set; }

		public string Type { get; set; }

		public string Message { get; set; }
	}
}
