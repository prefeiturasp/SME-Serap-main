using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvaSP.Data
{
    public enum TipoQuestionario : int
    {
        QuestionarioSupervisor = 1,
        QuestionarioDiretor = 2,
        QuestionarioCoordenadorPedagogico = 3,
        FichaRegistroAplicadorProva = 8,
        FichaRegistroSupervisor = 9,
        FichaRegistroDiretor = 10,
        FichaRegistroCoordenadorPedagogico = 11,
        QuestionarioProfessor = 12
    };

    public enum TipoPerfil : int
    {
        Supervisor = 1,
        Diretor = 2,
        CoordenadorPedagogico = 3,
        Professor = 4,
        AdministradorSERAp = 5
    };

    public enum DRE 
    {
        [Description("CAPELA DO SOCORRO")]
        CS,

        [Description("CAMPO LIMPO")]
        CL,

        [Description("ITAQUERA")]
        IQ,

        [Description("FREGUESIA/BRASILANDIA")]
        FO,

        [Description("SÃO MATEUS")]
        SM,

        [Description("GUAIANASES")]
        G,

        [Description("PENHA")]
        PE,

        [Description("SANTO AMARO")]
        SA,

        [Description("PIRITUBA/JARAGUA")]
        PJ,

        [Description("SÃO MIGUEL")]
        MP,

        [Description("BUTANTÃ")]
        BT,

        [Description("IPIRANGA")]
        IP,

        [Description("JAÇANÃ/TREMEMBE")]
        JT
    };

    public enum Atributo : int
    {
        [Description("Total esperado de Diretores que devem preencher questionário e ficha:")]
        NumeroDeQuestionariosDeDiretor_ParaPreencher = 1,

        [Description("Número de Diretores que preencheram o questionário:")]
        NumeroDeQuestionariosDeDiretor_TotalPreenchidos = 2,

        [Description("Total esperado de Coordenadores que devem preencher questionário e ficha:")]
        NumeroDeQuestionariosDeCoordenador_ParaPreencher = 3,

        [Description("Número de Coordenadores que preencheram o questionário:")]
        NumeroDeQuestionariosDeCoordenador_TotalPreenchidos = 4,

        [Description("Total esperado de Professores que devem preencher o questionário:")]
        NumeroDeQuestionariosDeProfessor_ParaPreencher = 5,

        [Description("Número de professores que preencheram o questionário:")]
        NumeroDeQuestionariosDeProfessor_TotalPreenchidos = 6,

        [Description("Total esperado de Diretores que devem preencher a ficha de aplicação:")]
        NumeroDeFichasDeDiretor_ParaPreencher = 7,

        [Description("Número de Diretores que preencheram a ficha de aplicação:")]
        NumeroDeFichasDeDiretor_TotalPreenchidos = 8,

        [Description("Total esperado de Coordenadores que devem preencher a ficha de aplicação:")]
        NumeroDeFichasDeCoordenador_ParaPreencher = 9,

        [Description("Número de Coordenadores que preencheram a ficha de aplicação:")]
        NumeroDeFichasDeCoordenador_TotalPreenchidos = 10,

        [Description("Total esperado de fichas de aplicação de prova por dia:")]
        NumeroDeFichasDeAplicacao_ParaPreencherPorDia = 11,

        [Description("Turmas com fichas preenchidas (Prova de Língua Portuguesa):")]
        NumeroDeFichasDeAplicacao_TotalPreenchidasDia1 = 12,

        [Description("Turmas com fichas preenchidas (Prova de Matemática):")]
        NumeroDeFichasDeAplicacao_TotalPreenchidasDia2 = 13,

        [Description("Turmas com fichas preenchidas (Prova de Ciências da Natureza):")]
        NumeroDeFichasDeAplicacao_TotalPreenchidasDia3 = 14,

        [Description("Ficha preenchida (Prova de Língua Portuguesa)?")]
        FichaAplicacaoPreenchidaDia1 = 21,

        [Description("Ficha preenchida (Prova de Matemática)?")]
        FichaAplicacaoPreenchidaDia2 = 22,

        [Description("Ficha preenchida (Prova de Ciências da Natureza)?")]
        FichaAplicacaoPreenchidaDia3 = 23,

        [Description("Questionário de Diretor preenchido?")]
        QuestionarioDeDiretorPreenchido = 30,

        [Description("Questionário de Coordenador preenchido?")]
        QuestionarioDeCoordenadorPreenchido = 31,

        [Description("Questionário de Professor preenchido?")]
        QuestionarioDeProfessorPreenchido = 32,

        [Description("Ficha de aplicação de Diretor preenchida?")]
        FichaAplicacaoDeDiretorPreenchida = 33,

        [Description("Ficha de aplicação de Coordenador preenchida?")]
        FichaAplicacaoDeCoordenadorPreenchida = 34,

        [Description("Escola supervisionada?")]
        EscolaSupervisionada = 35,

        [Description("Questionário de Supervisor preenchido?")]
        QuestionarioDeSupervisorPreenchido = 53,

        [Description("Total esperado de Supervisores que devem preencher o questionário:")]
        NumeroDeQuestionariosDeSupervisor_ParaPreencher = 54,

        [Description("Número de Supervisores que preencheram o questionário:")]
        NumeroDeQuestionariosDeSupervisor_TotalPreenchidos = 55,
    };

    public static class EnumHelper<T>
    {
        public static string GetEnumDescription(string value)
        {
            Type type = typeof(T);
            var name = Enum.GetNames(type).Where(f => f.Equals(value, StringComparison.CurrentCultureIgnoreCase)).Select(d => d).FirstOrDefault();

            if (name == null)
            {
                return string.Empty;
            }
            var field = type.GetField(name);
            var customAttribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return customAttribute.Length > 0 ? ((DescriptionAttribute)customAttribute[0]).Description : name;
        }
    }

}
