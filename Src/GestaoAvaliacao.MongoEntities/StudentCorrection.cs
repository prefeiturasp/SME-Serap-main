using GestaoAvaliacao.MongoEntities.Attribute;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.MongoEntities
{
    [CollectionName("StudentCorrection")]
	public class StudentCorrection : EntityBase
	{
		public long Test_Id { get; set; }
		public long alu_id { get; set; }
		public long tur_id { get; set; }
		public double Grade { get; set; }
		public int Hits { get; set; }
		public List<Answer> Answers { get; set; }
		public bool Automatic { get; set; }
        public Guid dre_id { get; set; }
        public int esc_id { get; set; }
        public int NumberAnswers { get; set; }
        public int? OrdemUltimaResposta { get; set; }
        public bool? provaFinalizada { get; set; }

        public StudentCorrection()
		{
		}

		public StudentCorrection(long test_id, long tur_id, long alu_id, Guid ent_id, Guid dre_id, int esc_id)
		{
			this._id = string.Format("{0}_{1}_{2}_{3}", ent_id, test_id, tur_id, alu_id);
			this.Test_Id = test_id;
			this.tur_id = tur_id;
			this.alu_id = alu_id;
            this.dre_id = dre_id;
            this.esc_id = esc_id;
            this.Answers = new List<Answer>();
		}
	}

	public class Answer
	{
		public long Item_Id { get; set; }
		public long AnswerChoice { get; set; }
		public bool Correct { get; set; }
		public bool Empty { get; set; }
		public bool StrikeThrough { get; set; }
        public bool Automatic { get; set; }

        public override bool Equals(object obj)
        {
			if (obj.GetType() != typeof(Answer)) return false;
			if (Item_Id == ((Answer)obj).Item_Id) return true;
			return false;
        }

        public override int GetHashCode() => Item_Id.GetHashCode();
    }
}
