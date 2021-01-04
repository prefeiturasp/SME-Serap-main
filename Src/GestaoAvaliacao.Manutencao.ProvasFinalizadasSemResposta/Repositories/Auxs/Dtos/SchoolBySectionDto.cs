namespace GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta.Repositories.Auxs.Dtos
{
    internal class SchoolBySectionDto
    {
        public long SectionId { get; set; }
        public int SchoolId { get; set; }
        public string Name { get; set; }
        public string DreName { get; set; }
    }
}