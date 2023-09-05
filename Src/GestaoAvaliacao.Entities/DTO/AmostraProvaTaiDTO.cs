namespace GestaoAvaliacao.Entities.DTO
{
    public class AmostraProvaTaiDTO
    {
        public long ProvaLegadoId { get; set; }
        public int DisciplinaId { get; set; }
        public int MatrizId { get; set; }
        public int NumeroItensAmostra { get; set; }
        public bool AvancarSemResponder { get; set; }
        public bool VoltarAoItemAnterior { get; set; }
    }
}
