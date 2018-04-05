using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Entities
{
    public class StudentCorrectionAnswerGrid : ICloneable
	{
		public int Order { get; set; }
		public long Item_Id { get; set; }
		public List<StudentCorrectionAnswerGridAlternatives> Alternatives { get; set; }
		public bool Null { get; set; }
		public bool StrikeThrough { get; set; }

		public object Clone()
		{
			StudentCorrectionAnswerGrid retorno = (StudentCorrectionAnswerGrid)this.MemberwiseClone();
			retorno.Alternatives = this.Alternatives.Select(a => (StudentCorrectionAnswerGridAlternatives)a.Clone()).ToList();

			return retorno;
		}
	}

	public class StudentCorrectionAnswerGridAlternatives : ICloneable
	{
		public long Id { get; set; }
		public int Order { get; set; }
		public string Numeration { get; set; }
		public bool Selected { get; set; }

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
