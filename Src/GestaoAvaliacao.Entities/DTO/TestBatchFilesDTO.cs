using System.Collections.Generic;

namespace GestaoAvaliacao.Entities.DTO
{
    public class TestBatchFilesDTO
    {
        public long Test_Id { get; set; }
        public IEnumerable<File> Files { get; set; }
    }
}
