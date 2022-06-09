namespace GestaoAvaliacao.API.Models
{
    public class ItemModel
    {

        public long MatrizId { get; set; }

        public string ItemCodigo { get; set; }
        public TextoBase TextoBase { get; set; }

        public long HabilidadeId { get; set; }
        public long AssuntoId { get; set; }
        public long SubassuntoId { get; set; }
        public long SituacaoId { get; set; }
        public long TipoItemId { get; set; }
        public bool SigiloItem { get; set; }
        public string[] PalavrasChave { get; set; }
        public int Proficiencia { get; set; }
        public int[] Series { get; set; }
        public long DificuldadeId { get; set; }
        public string Observacao { get; set; }

        public decimal? TriDiscriminacao { get; set; }
        public decimal? TriDificuldade { get; set; }
        public decimal? TriAcertoCasual { get; set; }

        public string Enunciado { get; set; }

        public Alternativa[] Alternativas { get; set; }




        //public long Id { get; set; }
        //public long AlternativeId { get; set; }
        //public int Order { get; set; }
        //public int AlternativeOrder { get; set; }
        //public bool EmptyAlternative { get; set; }
        //public bool DuplicateAlternative { get; set; }

        //public long DisciplinaId { get; set; }
        //public long MatrizId { get; set; }
        //public long ModeloMatrizId { get; set; }

        //public string TextoBaseDescricao { get; set; }
        //public string TextoBaseOrientacaoFonte { get; set; }



        //-------- PASSO 1
        //disciplina **
        //modeloMatriz **
        //matriz **

        //-------- PASSO 2
        //textobase.Description
        //textobase.BaseTextOrientation(Fonte)

        //-------- PASSO 3
        //Código do item

        //EixoId **
        //HabilidadeId **

        //assunto.Id **
        //subassunto.Id **

        //situacaoId(Situação do item)**
        //tipoItemId(Tipo do item)**
        //sigiloItem(boleano) **

        //palavrasChave(array de palavras) **
        //proficiencia (Valor de 100 a 500)

        //series(ANOS ESCOLARES) **
        //dificuldadeId [ITEMLEVEL](Dificuldade sugerida) **

        //observacao(Observação) **

        //--TRI
        //Discriminação(0 até 10)
        //Dificuldade(texto)
        //Acerto casual(0 até 1)

        //video

        //audio

        //Enunciado(texto) **

        //-- Alternativas (o número de alternatinas tem que ser igual ao tipo do item que informa a quantidade de alternativas **VALIDAR NA HORA DE SALVAR**)
        //texto **
        //justificativa

    }

    public class Alternativa
    {
        public string Texto { get; set; }
        public string Justificativa { get; set; }

        public bool Correta { get; set; }

        public int Ordem { get; set; }

        public string Numeracao { get; set; }

        public decimal? TCTCoeficienteBisserial { get; set; }

        public decimal? TCTProporcaoAcertos { get; set; }

        public decimal? TCTDiscriminacao { get; set; }
    }

    public class TextoBase
    {
        public string Descricao { get; set; }
        public string Fonte { get; set; }
    }
}