using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.API.Models
{
    public class BatchResultModel
    {
        public Guid Ent_Id { get; set; }
        public long Test_Id { get; set; }
        public long Section_Id { get; set; }
        public long Batch_Id { get; set; }
        public List<StudentModel> Students { get; set; }
        public bool exclusionLogic { get; set; }
    }

    public class BatchFileResultModel
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
    }

	public class BatchStudentResultModel
	{
		public long Id { get; set; }
		public int Status { get; set; }
		public string Description { get; set; }
		public long? StudentID { get; set; }
		public long? SectionId { get; set; }
        public long? TestId { get; set; }
    }
}