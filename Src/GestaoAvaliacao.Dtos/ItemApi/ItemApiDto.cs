using System.Collections.Generic;
using System.ComponentModel;

namespace GestaoAvaliacao.Dtos.ItemApi
{
    public enum Dificuldade
    {
        Nenhum = 0,
        MuitoFacil = 1,
        Facil = 2,
        Medio = 3,
        Dificil = 4,
        MuitoDificil = 5
    }

    public class ItemApiDto
    {
        public long AreaConhecimentoId { get; set; }
        public long MatrizId { get; set; }
        public string TextoBase { get; set; }
        public string Fonte { get; set; }
        public string CodigoItem { get; set; }
        public int CompetenciaId { get; set; }
        public int HabilidadeId { get; set; }
        public int TipoGradeCurricularId { get; set; }
        public Dificuldade Dificuldade { get; set; }
        public long SubassuntoId { get; set; }
        public string Observacao { get; set; }
        public long TipoItemId { get; set; }
        public bool Sigiloso { get; set; }
        public decimal? TRIDiscrimicacao { get; set; }
        public decimal? TRIAcertoCasual { get; set; }
        public decimal? TRIDificuldade { get; set; }
        public string PalavrasChave { get; set; }
        public int? Proficiencia { get; set; }
        public string Enunciado { get; set; }

        public List<AlternativeDto> Alternativas { get; set; }
        public List<PictureDto> Imagens { get; set; }
        public List<VideoDto> Videos { get; set; }
        public List<AudioDto> Audios { get; set; }
    }

    public class ItemConsultaApiPaginadoDto
    {
        public int Pagina { get; set; }
        public int QtdePorPagina { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalItems { get; set; }
        public List<ItemConsultaApiDto> Items { get; set; }
    }

    public class ItemConsultaApiDto
    {
        public long Id { get; set; }
        public long AreaConhecimentoId { get; set; }
        public long MatrizId { get; set; }
        public string TextoBase { get; set; }
        public string Fonte { get; set; }
        public string CodigoItem { get; set; }
        public int CompetenciaId { get; set; }
        public int HabilidadeId { get; set; }
        public int TipoGradeCurricularId { get; set; }
        public Dificuldade Dificuldade { get; set; }
        public long SubassuntoId { get; set; }
        public string Observacao { get; set; }
        public long TipoItemId { get; set; }
        public bool Sigiloso { get; set; }
        public decimal? TRIDiscrimicacao { get; set; }
        public decimal? TRIAcertoCasual { get; set; }
        public decimal? TRIDificuldade { get; set; }
        public string PalavrasChave { get; set; }
        public int? Proficiencia { get; set; }
        public string Enunciado { get; set; }

        public List<AlternativeDto> Alternativas { get; set; }
        public List<ArquivoConsultaDto> Imagens { get; set; }
        public List<ArquivoConsultaDto> Videos { get; set; }
        public List<ArquivoConsultaDto> Audios { get; set; }
    }

    public class ArquivosItemConsultaApiDto
    {
        public List<ArquivoConsultaDto> Imagens { get; set; }
        public List<ArquivoConsultaDto> Videos { get; set; }
        public List<ArquivoConsultaDto> Audios { get; set; }
    }

    public class AlternativeDto
    {
        public string Descricao { get; set; }

        public bool Correta { get; set; }

        public int Ordem { get; set; }

        public string Justificativa { get; set; }

        public string Numeracao { get; set; }
    }

    public class PictureDto
    {
        public string Tag { get; set; }
        public int Tamanho { get; set; }
        public string TipoConteudo { get; set; }
        public string Base64 { get; set; }
        public string NomeArquivo { get; set; }
        public PictureType Tipo { get; set; }
    }

    public enum PictureType
    {
        [Description("Texto_Base")]
        BaseText = 1,
        [Description("Alternativa")]
        Alternative = 2,
        [Description("Justificativa")]
        Justificative = 3,
        [Description("Enunciado")]
        Statement = 4
    }

    public class VideoDto
    {
        public int Tamanho { get; set; }
        public string TipoConteudo { get; set; }
        public string Base64 { get; set; }
        public string NomeArquivo { get; set; }

        public int MiniaturaTamanho { get; set; }
        public string MiniaturaTipoConteudo { get; set; }
        public string MiniaturaBase64 { get; set; }
        public string MiniaturaNomeArquivo { get; set; }
    }

    public class ArquivoConsultaDto
    {
        public long Id { get; set; }
        public string Base64 { get; set; }
        public string NomeArquivo { get; set; }
    }

    public class AudioDto
    {
        public int Tamanho { get; set; }
        public string TipoConteudo { get; set; }
        public string Base64 { get; set; }
        public string NomeArquivo { get; set; }
    }
}
