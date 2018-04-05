using GestaoAvaliacao.MongoEntities.DTO;
using System.Collections.Generic;

namespace GestaoAvaliacao.API.Models
{
    public class StudentModel
    {
        public long Id { get; set; }
        public int NumberId { get; set; }
        public long File_Id { get; set; }
        public bool Absent { get; set; }
        public List<ItemModelDTO> Items { get; set; }
    }
}