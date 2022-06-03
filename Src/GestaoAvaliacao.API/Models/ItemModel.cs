namespace GestaoAvaliacao.API.Models
{
    public class ItemModel
    {
        public long Id { get; set; }
        public long AlternativeId { get; set; }
        public int Order { get; set; }
        public int AlternativeOrder { get; set; }
        public bool EmptyAlternative { get; set; }
        public bool DuplicateAlternative { get; set; }


        //-------- PASSO 1
        //disciplina **
        //modeloMatriz **
        //matriz **

        //-------- PASSO 2
        //textobase.Description
        //textobase.BaseTextOrientation(Fonte)

        //-------- PASSO 3
        //Código do item

        //Eixo.Id **
        //Habilidade.Id **

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
}