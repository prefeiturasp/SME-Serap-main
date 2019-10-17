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
        QuestionarioProfessor = 12,
        QuestionarioAssistenteDiretoria = 13,
        QuestionarioAuxiliarTecnicoEducacao = 14,
        QuestionarioAgenteEscolarMerendeira = 15,
        QuestionarioAgenteEscolarPortaria = 16,
        QuestionarioAgenteEscolarZeladoria = 17,

        /* Edição 2018
        QuestionarioAlunos3Ano = 18,
        QuestionarioAlunos4AnoAo6Ano = 19,
        QuestionarioAlunos7AnoAo9Ano = 20 */

        //Edição 2019
        QuestionarioAlunos3AnoAo6Ano = 21,
        QuestionarioAlunos7AnoAo9Ano = 22
    };

    public enum TipoPerfil : int
    {
        Supervisor = 1,
        Diretor = 2,
        CoordenadorPedagogico = 3,
        Professor = 4,
        AdministradorSERAp = 5,
        Aluno = 6
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

        [Description("Número de Assistentes de Diretores que devem preencher o questionário:")]
        NumeroDeQuestionariosDeAssistenteDiretoria_ParaPreencher = 81,

        [Description("Número de Assistentes de Diretores que preencheram o questionário:")]
        NumeroDeQuestionariosDeAssistenteDiretoria_TotalPreenchidos = 82,

        [Description("Questionário de Assistente de Diretor preenchido?")]
        QuestionarioAssistenteDiretoriaPreenchido = 83,

        [Description("Número de Assistentes de Diretores que devem preencher o questionário:")]
        NumeroDeQuestionariosAlunos3Ano_ParaPreencher = 84,

        [Description("Número de Assistentes de Diretores que preencheram o questionário:")]
        NumeroDeQuestionariosAlunos3Ano_TotalPreenchidos = 85,

        [Description("Questionário de Assistente de Diretor preenchido?")]
        QuestionarioAlunos3AnoPreenchido = 86,

        [Description("Número de Assistentes de Diretores que devem preencher o questionário:")]
        NumeroDeQuestionariosAlunos4AnoAo6Ano_ParaPreencher = 87,

        [Description("Número de Assistentes de Diretores que preencheram o questionário:")]
        NumeroDeQuestionariosAlunos4AnoAo6Ano_TotalPreenchidos = 88,

        [Description("Questionário de Assistente de Diretor preenchido?")]
        QuestionarioAlunos4AnoAo6AnoPreenchido = 89,

        [Description("Número de Assistentes de Diretores que devem preencher o questionário:")]
        NumeroDeQuestionariosAlunos7AnoAo9Ano_ParaPreencher = 90,

        [Description("Número de Assistentes de Diretores que preencheram o questionário:")]
        NumeroDeQuestionariosAlunos7AnoAo9Ano_TotalPreenchidos = 91,

        [Description("Questionário de Assistente de Diretor preenchido?")]
        QuestionarioAlunos7AnoAo9AnoPreenchido = 92,

        [Description("Número de Auxiliar Técnico da Educação que devem preencher o questionário:")]
        NumeroDeQuestionariosDeAuxiliarTecnicoEducacao_ParaPreencher = 93,

        [Description("Número de Auxiliar Técnico da Educação que preencheram o questionário:")]
        NumeroDeQuestionariosDeAuxiliarTecnicoEducacao_TotalPreenchidos = 94,

        [Description("Questionário de Auxiliar Técnico da Educação preenchido?")]
        QuestionarioAuxiliarTecnicoEducacaoPreenchido = 95,

        [Description("Número de Agente Escolar - Merendeira que devem preencher o questionário:")]
        NumeroDeQuestionariosDeAgenteEscolarMerendeira_ParaPreencher = 96,

        [Description("Número de Agente Escolar - Merendeira que preencheram o questionário:")]
        NumeroDeQuestionariosDeAgenteEscolarMerendeira_TotalPreenchidos = 97,

        [Description("Questionário de Agente Escolar - Merendeira preenchido?")]
        QuestionarioAgenteEscolarMerendeiraPreenchido = 98,

        [Description("Número de Agente Escolar - Portaria que devem preencher o questionário:")]
        NumeroDeQuestionariosDeAgenteEscolarPortaria_ParaPreencher = 99,

        [Description("Número de Agente Escolar - Portaria que preencheram o questionário:")]
        NumeroDeQuestionariosDeAgenteEscolarPortaria_TotalPreenchidos = 100,

        [Description("Questionário de Agente Escolar - Portaria preenchido?")]
        QuestionarioAgenteEscolarPortariaPreenchido = 101,

        [Description("Número de Agente Escolar - Zeladoria que devem preencher o questionário:")]
        NumeroDeQuestionariosDeAgenteEscolarZeladoria_ParaPreencher = 102,

        [Description("Número de Agente Escolar - Zeladoria que preencheram o questionário:")]
        NumeroDeQuestionariosDeAgenteEscolarZeladoria_TotalPreenchidos = 103,

        [Description("Questionário de Agente Escolar - Zeladoria preenchido?")]
        QuestionarioAgenteEscolarZeladoriaPreenchido = 104
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
