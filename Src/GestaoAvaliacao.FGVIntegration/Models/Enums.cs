namespace GestaoAvaliacao.FGVIntegration.Models
{
    public static class Enums
    {

        /// <summary>
        /// https://cidades.ibge.gov.br
        /// </summary>
        public enum CidadeIBGE
        {
            SP_SAO_PAULO = 3550308,
        }

        public enum UnidadeFederativa
        {
            AC,
            AL,
            AM,
            AP,
            BA,
            CE,
            DF,
            ES,
            GO,
            MA,
            MG,
            MS,
            MT,
            PA,
            PB,
            PE,
            PI,
            PR,
            RJ,
            RN,
            RO,
            RR,
            RS,
            SC,
            SE,
            SP,
            TO,
        }

        public enum TipoEscola
        {

            FEDERAL = 1,
            ESTADUAL = 2,
            MUNICIPAL = 3,
            PRIVADA = 4,
        }

        public enum Serie
        {
            _1 = 1, 
            _2 = 2, 
            _3 = 3,
        }

        public enum Turno
        {
            MANHA = 'M',
            TARDE = '2',
            NOITE = '3',
        }

        public enum TipoCoordenador
        {

            COORDENADOR_REDE = 1,
            COORDENADOR_ESCOLA = 2,
        }

        public enum Sexo
        {
            MASCULINO = 'M',
            FEMININO = 'F',
        }

        public enum Disciplina
        {
            ART,
            ESP,
            FIS,
            HIS,
            LIT,
            PORT,
            QUI,
            SOC,
            BIO,
            FIL,
            GEO,
            ING,
            MAT,
            PRED,
            RED,
        }

        public enum SituacaoRegistro
        {
            ATIVO = 1,
            EXCLUIDO = 3,
        }

        public enum CargoEscola
        {
            DIRETOR = 3360,
            COORDENADOR = 3379,
        }

        public enum TipoNivelEnsino
        {
            ENSINO_MEDIO = 3
        }

    }
}
