namespace GestaoAvaliacao.Dtos.SimuladorSerapEstudantes
{
    public class ResponseUploadArquivoAudioDto
    {
        public bool Success { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string FileLink { get; set; }
        public long IdFile { get; set; }
    }
}
