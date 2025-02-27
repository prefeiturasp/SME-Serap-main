namespace GestaoAvaliacao.Entities.DTO
{
    public class DadosProvaTaiDTO
    {
        public long ProvaId { get; set; }
        public int? DisciplinaId { get; set; }
        public int NumeroItensAmostra { get; set; }
        public bool AvancarSemResponder { get; set; }
        public bool VoltarAoItemAnterior { get; set; }
    }
}
