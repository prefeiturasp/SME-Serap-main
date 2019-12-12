using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using ProvaSP.Data.Funcionalidades;
using ProvaSP.Model.Entidades;
namespace ProvaSP.Data
{
    public static class DataAcompanhamentoAplicacao
    {
        #region AtributosDasDimensoes
        private static string AtributosAcompanhamentoEscola()
        {
            var sbAtributos = new StringBuilder();
            sbAtributos.Append((int)Atributo.NumeroDeQuestionariosDeDiretor_ParaPreencher);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeQuestionariosDeDiretor_TotalPreenchidos);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeQuestionariosDeCoordenador_ParaPreencher);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeQuestionariosDeCoordenador_TotalPreenchidos);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeQuestionariosDeProfessor_ParaPreencher);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeQuestionariosDeProfessor_TotalPreenchidos);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeFichasDeDiretor_ParaPreencher);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeFichasDeDiretor_TotalPreenchidos);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeFichasDeCoordenador_ParaPreencher);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeFichasDeCoordenador_TotalPreenchidos);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeFichasDeAplicacao_ParaPreencherPorDia);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeFichasDeAplicacao_TotalPreenchidasDia1);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeFichasDeAplicacao_TotalPreenchidasDia2);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeFichasDeAplicacao_TotalPreenchidasDia3);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeQuestionariosDeAssistenteDiretoria_ParaPreencher);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeQuestionariosDeAssistenteDiretoria_TotalPreenchidos);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeQuestionariosAlunos4AnoAo6Ano_ParaPreencher);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeQuestionariosAlunos4AnoAo6Ano_TotalPreenchidos);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeQuestionariosAlunos7AnoAo9Ano_ParaPreencher);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.NumeroDeQuestionariosAlunos7AnoAo9Ano_TotalPreenchidos);
            return sbAtributos.ToString();
        }

        private static string OrdenacaoAtributosAcompanhamentoEscola()
        {
            return @"CASE
                        WHEN a.Nome = 'NumeroDeQuestionariosDeDiretor_ParaPreencher' THEN 1
                        WHEN a.Nome = 'NumeroDeQuestionariosDeDiretor_TotalPreenchidos' THEN 2
                        WHEN a.Nome = 'NumeroDeFichasDeDiretor_TotalPreenchidos' THEN 3
                        WHEN a.Nome = 'NumeroDeQuestionariosDeCoordenador_ParaPreencher' THEN 4
                        WHEN a.Nome = 'NumeroDeQuestionariosDeCoordenador_TotalPreenchidos' THEN 5
                        WHEN a.Nome = 'NumeroDeFichasDeCoordenador_TotalPreenchidos' THEN 6
                        WHEN a.Nome = 'NumeroDeQuestionariosDeAssistenteDiretoria_ParaPreencher' THEN 7
                        WHEN a.Nome = 'NumeroDeQuestionariosDeAssistenteDiretoria_TotalPreenchidos' THEN 8
                        WHEN a.Nome = 'NumeroDeQuestionariosDeProfessor_ParaPreencher' THEN 9
                        WHEN a.Nome = 'NumeroDeQuestionariosDeProfessor_TotalPreenchidos' THEN 10
                        WHEN a.Nome = 'NumeroDeFichasDeAplicacao_ParaPreencherPorDia' THEN 11
                        WHEN a.Nome = 'NumeroDeFichasDeAplicacao_TotalPreenchidasDia1' THEN 12
                        WHEN a.Nome = 'NumeroDeFichasDeAplicacao_TotalPreenchidasDia2' THEN 13
                        WHEN a.Nome = 'NumeroDeFichasDeAplicacao_TotalPreenchidasDia3' THEN 14
                        WHEN a.Nome = 'NumeroDeQuestionariosAlunos4AnoAo6Ano_ParaPreencher' THEN 15
                        WHEN a.Nome = 'NumeroDeQuestionariosAlunos4AnoAo6Ano_TotalPreenchidos' THEN 16
                        WHEN a.Nome = 'NumeroDeQuestionariosAlunos7AnoAo9Ano_ParaPreencher' THEN 17
                        WHEN a.Nome = 'NumeroDeQuestionariosAlunos7AnoAo9Ano_TotalPreenchidos' THEN 18
                        ELSE 99
                    END";
        }

        private static string AtributosAcompanhamentoTurma()
        {
            var sbAtributos = new StringBuilder();
            sbAtributos.Append((int)Atributo.FichaAplicacaoPreenchidaDia1);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.FichaAplicacaoPreenchidaDia2);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.FichaAplicacaoPreenchidaDia3);
            return sbAtributos.ToString();
        }

        private static string AtributosAcompanhamentoPessoa()
        {
            var sbAtributos = new StringBuilder();
            sbAtributos.Append((int)Atributo.QuestionarioDeDiretorPreenchido);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.FichaAplicacaoDeDiretorPreenchida);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.QuestionarioDeCoordenadorPreenchido);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.FichaAplicacaoDeCoordenadorPreenchida);
            sbAtributos.Append(",");
            sbAtributos.Append((int)Atributo.QuestionarioDeProfessorPreenchido);
            return sbAtributos.ToString();
        }
        #endregion

        #region AcompanhamentoEscola

        public static List<RelatorioItem> RecuperarAcompanhamentoEscolaNivelSME(string Edicao)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                try
                {
                    List<RelatorioItem> retorno = conn.Query<RelatorioItem>(
                sql: @"
                        SELECT '' AS Titulo, a.AtributoID, a.Nome AS Atributo, SUM(CAST(aae.Valor AS int)) AS Valor
                        FROM Escola e WITH (NOLOCK)
                        JOIN AcompanhamentoAplicacaoEscola aae WITH (NOLOCK) ON e.esc_codigo=aae.esc_codigo
                        JOIN Atributo a WITH (NOLOCK) ON a.AtributoID=aae.AtributoID
                        WHERE aae.Edicao=@Edicao AND a.AtributoID IN (" + AtributosAcompanhamentoEscola() + @")
                        GROUP BY a.AtributoID, a.Nome
                        ORDER BY " + OrdenacaoAtributosAcompanhamentoEscola(),
                param: new
                {
                    Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 }
                }
                ).AsList<RelatorioItem>();
                    return retorno;
                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }

        }

        public static List<RelatorioItem> RecuperarAcompanhamentoEscolaNivelSME_PorDRE(string Edicao)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                try
                {
                    List<RelatorioItem> retorno = conn.Query<RelatorioItem>(
                sql: @"
                        SELECT e.uad_codigo AS Titulo, a.AtributoID, a.Nome AS Atributo, SUM(CAST(aae.Valor AS int)) AS Valor
                        FROM Escola e WITH (NOLOCK)
                        JOIN AcompanhamentoAplicacaoEscola aae WITH (NOLOCK) ON e.esc_codigo=aae.esc_codigo
                        JOIN Atributo a WITH (NOLOCK) ON a.AtributoID=aae.AtributoID
                        WHERE aae.Edicao=@Edicao AND a.AtributoID IN (" + AtributosAcompanhamentoEscola() + @")
                        GROUP BY e.uad_codigo, a.AtributoID, a.Nome
                        ORDER BY e.uad_codigo, 
                                " + OrdenacaoAtributosAcompanhamentoEscola(),
                param: new
                {
                    Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 }
                }
                ).AsList<RelatorioItem>();
                    return retorno;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        public static List<RelatorioItem> RecuperarAcompanhamentoEscolaNivelDRE(string Edicao, string uad_codigo)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                try
                {
                    List<RelatorioItem> retorno = conn.Query<RelatorioItem>(
                sql: @"
                        SELECT '' AS Titulo, a.AtributoID, a.Nome AS Atributo, SUM(CAST(aae.Valor AS int)) AS Valor
                        FROM Escola e WITH (NOLOCK)
                        JOIN AcompanhamentoAplicacaoEscola aae WITH (NOLOCK) ON e.esc_codigo=aae.esc_codigo
                        JOIN Atributo a WITH (NOLOCK) ON a.AtributoID=aae.AtributoID
                        WHERE aae.Edicao=@Edicao AND a.AtributoID IN (" + AtributosAcompanhamentoEscola() + @") AND e.uad_codigo=@uad_codigo
                        GROUP BY a.AtributoID, a.Nome
                        ORDER BY " + OrdenacaoAtributosAcompanhamentoEscola(),
                param: new
                {
                    Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                    uad_codigo = new DbString() { Value = uad_codigo, IsAnsi = true, Length = 20 }
                }
                ).AsList<RelatorioItem>();
                    return retorno;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        public static List<RelatorioItem> RecuperarAcompanhamentoEscolaNivelDRE_PorEscola(string Edicao, string uad_codigo)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                try
                {
                    List<RelatorioItem> retorno = conn.Query<RelatorioItem>(
                sql: @"
                        SELECT e.esc_codigo AS Chave, e.esc_nome AS Titulo, a.AtributoID, a.Nome AS Atributo, SUM(CAST(aae.Valor AS int)) AS Valor
                        FROM Escola e WITH (NOLOCK)
                        JOIN AcompanhamentoAplicacaoEscola aae WITH (NOLOCK) ON e.esc_codigo=aae.esc_codigo
                        JOIN Atributo a WITH (NOLOCK) ON a.AtributoID=aae.AtributoID
                        WHERE aae.Edicao=@Edicao AND a.AtributoID IN (" + AtributosAcompanhamentoEscola() + @") AND e.uad_codigo=@uad_codigo
                        GROUP BY e.esc_codigo,e.esc_nome, a.AtributoID, a.Nome
                        ORDER BY e.esc_nome,
                                " + OrdenacaoAtributosAcompanhamentoEscola(),
                param: new
                {
                    Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                    uad_codigo = new DbString() { Value = uad_codigo, IsAnsi = true, Length = 20 }
                }
                ).AsList<RelatorioItem>();
                    return retorno;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        public static List<RelatorioItem> RecuperarAcompanhamentoEscolaNivelEscola(string Edicao, string esc_codigo)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                try
                {
                    List<RelatorioItem> retorno = conn.Query<RelatorioItem>(
                sql: @"
                        SELECT '' AS Titulo, a.AtributoID, a.Nome AS Atributo, aae.Valor
                        FROM AcompanhamentoAplicacaoEscola aae WITH (NOLOCK)
                        JOIN Atributo a WITH (NOLOCK) ON a.AtributoID=aae.AtributoID
                        WHERE aae.Edicao=@Edicao AND a.AtributoID IN (" + AtributosAcompanhamentoEscola() + @") AND aae.esc_codigo=@esc_codigo
                        ORDER BY " + OrdenacaoAtributosAcompanhamentoEscola(),
                param: new
                {
                    Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                    esc_codigo = new DbString() { Value = esc_codigo, IsAnsi = true, Length = 20 }
                }
                ).AsList<RelatorioItem>();
                    return retorno;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        /// <summary>
        /// Cria um agrupamento por Escola ou DRE para exibir as grids com quantidades de respostas de forma agrupada.
        /// Nos casos onde não existe agrupamento, será considerado como um agrupamento único para a Chave="".
        /// </summary>
        public static IList<RelatorioAgrupamento> MontarGridQuantidadeRespondentes(List<RelatorioItem> pIndicadoresRel)
        {
            IList<RelatorioAgrupamento> listaAgrupada = new List<RelatorioAgrupamento>();

            IDictionary<TipoRespondenteQuestionario, RelatorioItemAgrupado> gridRespondentes = null;
            RelatorioAgrupamento agrupamentoAtual = null;

            /* marcos remover for (int i = pIndicadoresRel.Count - 1; i >= 0; i--)
            {
                var relatorioItem = pIndicadoresRel[i];*/
            foreach (var relatorioItem in pIndicadoresRel)
            {
                //se mudou o titulo, cria um novo item de agrupamento
                if (agrupamentoAtual == null || relatorioItem.Titulo != agrupamentoAtual.Titulo)
                {
                    if (agrupamentoAtual != null)
                        agrupamentoAtual.GridIndicadoresEscola = gridRespondentes.Values.OrderBy(r => r.TipoRespondente).ToList();
                    agrupamentoAtual = new RelatorioAgrupamento()
                    {
                        Chave = relatorioItem.Chave,
                        Titulo = relatorioItem.Titulo
                    };
                    listaAgrupada.Add(agrupamentoAtual);
                    gridRespondentes = new Dictionary<TipoRespondenteQuestionario, RelatorioItemAgrupado>();
                }

                TipoRespondenteQuestionario tipoRespondente = TipoRespondenteQuestionario.ALUNO_3_6_ANO;

                Atributo atributo = (Atributo)relatorioItem.AtributoID;
                switch (atributo)
                {
                    case Atributo.NumeroDeQuestionariosDeDiretor_ParaPreencher:
                    case Atributo.NumeroDeQuestionariosDeDiretor_TotalPreenchidos:
                        tipoRespondente = TipoRespondenteQuestionario.DIRETOR;
                        break;
                    case Atributo.NumeroDeQuestionariosDeAssistenteDiretoria_ParaPreencher:
                    case Atributo.NumeroDeQuestionariosDeAssistenteDiretoria_TotalPreenchidos:
                        tipoRespondente = TipoRespondenteQuestionario.ASSISTENTE_DIRETOR;
                        break;
                    case Atributo.NumeroDeQuestionariosDeCoordenador_ParaPreencher:
                    case Atributo.NumeroDeQuestionariosDeCoordenador_TotalPreenchidos:
                        tipoRespondente = TipoRespondenteQuestionario.COORDENADOR;
                        break;
                    case Atributo.NumeroDeQuestionariosDeProfessor_ParaPreencher:
                    case Atributo.NumeroDeQuestionariosDeProfessor_TotalPreenchidos:
                        tipoRespondente = TipoRespondenteQuestionario.PROFESSOR;
                        break;
                    case Atributo.NumeroDeQuestionariosDeSupervisor_ParaPreencher:
                    case Atributo.NumeroDeQuestionariosDeSupervisor_TotalPreenchidos:
                        tipoRespondente = TipoRespondenteQuestionario.SUPERVISOR;
                        break;
                    case Atributo.NumeroDeQuestionariosAlunos4AnoAo6Ano_ParaPreencher:
                    case Atributo.NumeroDeQuestionariosAlunos4AnoAo6Ano_TotalPreenchidos:
                        tipoRespondente = TipoRespondenteQuestionario.ALUNO_3_6_ANO;
                        break;
                    case Atributo.NumeroDeQuestionariosAlunos7AnoAo9Ano_ParaPreencher:
                    case Atributo.NumeroDeQuestionariosAlunos7AnoAo9Ano_TotalPreenchidos:
                        tipoRespondente = TipoRespondenteQuestionario.ALUNO_7_9_ANO;
                        break;
                    case Atributo.NumeroDeQuestionariosDeAuxiliarTecnicoEducacao_ParaPreencher:
                    case Atributo.NumeroDeQuestionariosDeAuxiliarTecnicoEducacao_TotalPreenchidos:
                        tipoRespondente = TipoRespondenteQuestionario.AUXILIAR_TECNICO;
                        break;
                    case Atributo.NumeroDeQuestionariosDeAgenteEscolarMerendeira_ParaPreencher:
                    case Atributo.NumeroDeQuestionariosDeAgenteEscolarMerendeira_TotalPreenchidos:
                        tipoRespondente = TipoRespondenteQuestionario.AGENTE_ESCOLAR_MERENDEIRA;
                        break;
                    case Atributo.NumeroDeQuestionariosDeAgenteEscolarPortaria_ParaPreencher:
                    case Atributo.NumeroDeQuestionariosDeAgenteEscolarPortaria_TotalPreenchidos:
                        tipoRespondente = TipoRespondenteQuestionario.AGENTE_ESCOLAR_PORTARIA;
                        break;
                    case Atributo.NumeroDeQuestionariosDeAgenteEscolarZeladoria_ParaPreencher:
                    case Atributo.NumeroDeQuestionariosDeAgenteEscolarZeladoria_TotalPreenchidos:
                        tipoRespondente = TipoRespondenteQuestionario.AGENTE_ESCOLAR_ZELADORIA;
                        break;

                    default:
                        agrupamentoAtual.IndicadoresEscola.Add(relatorioItem);
                        continue;
                }

                if (!gridRespondentes.TryGetValue(tipoRespondente, out RelatorioItemAgrupado linhaRespondente))
                {
                    linhaRespondente = new RelatorioItemAgrupado()
                    {
                        TipoRespondente = (int)tipoRespondente,
                        DescricaoRespondente = EnumHelper<TipoRespondenteQuestionario>.GetEnumDescription(tipoRespondente),
                    };
                    gridRespondentes.Add(tipoRespondente, linhaRespondente);
                }

                if (EnumHelper<Atributo>.GetEnumDescription(atributo).IndexOf("Total esperado") == 0)
                    linhaRespondente.TotalEsperado = Convert.ToInt32(relatorioItem.Valor);
                else
                    linhaRespondente.QuantidadePreenchido = Convert.ToInt32(relatorioItem.Valor);

                if (linhaRespondente.TotalEsperado > 0)
                    linhaRespondente.PercentualPreenchido = (int)Math.Round((linhaRespondente.QuantidadePreenchido * 100d) / linhaRespondente.TotalEsperado);
            }

            if (agrupamentoAtual != null)
                agrupamentoAtual.GridIndicadoresEscola = gridRespondentes.Values.OrderBy(r => r.TipoRespondente).ToList();

            return listaAgrupada;
        }

        #endregion

        public static List<RelatorioItem> RecuperarAcompanhamentoTurmaNivelEscola(string Edicao, string esc_codigo)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                try
                {
                    List<RelatorioItem> retorno = conn.Query<RelatorioItem>(
                sql: @"
                        SELECT t.tur_codigo AS Titulo, a.AtributoID, a.Nome AS Atributo, CASE WHEN aat.Valor='0' THEN 'NÃO' ELSE 'SIM' END AS Valor
                        FROM AcompanhamentoAplicacaoTurma aat WITH (NOLOCK)
                        JOIN Turma t WITH (NOLOCK) ON t.tur_id=aat.tur_id
                        JOIN Atributo a WITH (NOLOCK) ON a.AtributoID=aat.AtributoID
                        WHERE aat.Edicao=@Edicao AND a.AtributoID IN (" + AtributosAcompanhamentoTurma() + @") AND t.esc_codigo=@esc_codigo
                        ORDER BY REPLACE(t.tur_codigo, 'ANO ',''), a.AtributoID
                    ",
                param: new
                {
                    Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                    esc_codigo = new DbString() { Value = esc_codigo, IsAnsi = true, Length = 20 }
                }
                ).AsList<RelatorioItem>();
                    return retorno;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        public static List<RelatorioItem> RecuperarAcompanhamentoPessoaNivelEscola(string Edicao, string esc_codigo)
        {
            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                try
                {
                    List<RelatorioItem> retorno = conn.Query<RelatorioItem>(
                sql: @"
                        SELECT p.Nome AS Titulo, a.AtributoID, a.Nome AS Atributo, CASE WHEN aap.Valor='0' THEN 'NÃO' ELSE 'SIM' END AS Valor
                        FROM AcompanhamentoAplicacaoPessoa aap WITH (NOLOCK)
                        JOIN Pessoa p WITH (NOLOCK) ON p.usu_id=aap.usu_id
                        JOIN Atributo a WITH (NOLOCK) ON a.AtributoID=aap.AtributoID
                        WHERE aap.Edicao=@Edicao AND a.AtributoID IN (" + AtributosAcompanhamentoPessoa() + @") AND aap.esc_codigo=@esc_codigo
                        ORDER BY CASE
                                    WHEN a.Nome LIKE '%Diretor%' THEN 1
                                    WHEN a.Nome LIKE '%Coordenador%' THEN 2
                                    ELSE 3
                                END, p.Nome
                    ",
                param: new
                {
                    Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                    esc_codigo = new DbString() { Value = esc_codigo, IsAnsi = true, Length = 20 }
                }
                ).AsList<RelatorioItem>();
                    return retorno;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        public static void ProcessarInclusaoQuestionario(string Edicao, DateTime DataInicioAplicacao, QuestionarioUsuario preenchimentoDeQuestionario, SqlTransaction dbContextTransaction, SqlConnection conn)
        {
            Atributo? atributoEscolaIncremento = null;

            TipoQuestionario tipoQuestionario = (TipoQuestionario)preenchimentoDeQuestionario.QuestionarioID;
            if (tipoQuestionario == TipoQuestionario.FichaRegistroAplicadorProva && preenchimentoDeQuestionario.tur_id != null)
            {
                Atributo atributoTurma = Atributo.FichaAplicacaoPreenchidaDia1;

                foreach (var resposta in preenchimentoDeQuestionario.Respostas)
                {
                    //ACREDITAR QUE O USUÁRIO SELECIONOU A DATA CORRETA DA APLICAÇÃO DA PROVA CAUSOU PROBLEMAS. O INDICADOR AGORA SE BASEIA NA PROVA SELECIONADA
                    //if (resposta.Numero == "2") //Data de Aplicação
                    //{
                    //    DataAplicacao = Convert.ToDateTime(resposta.Valor);
                    //}

                    if (resposta.Numero == "3") //Disciplina
                    {
                        if (resposta.Valor == "Português")
                        {
                            atributoTurma = Atributo.FichaAplicacaoPreenchidaDia1;
                            atributoEscolaIncremento = Atributo.NumeroDeFichasDeAplicacao_TotalPreenchidasDia1;
                        }
                        else if (resposta.Valor == "Matemática")
                        {
                            atributoTurma = Atributo.FichaAplicacaoPreenchidaDia2;
                            atributoEscolaIncremento = Atributo.NumeroDeFichasDeAplicacao_TotalPreenchidasDia2;
                        }
                        else if (resposta.Valor == "Ciências")
                        {
                            atributoTurma = Atributo.FichaAplicacaoPreenchidaDia3;
                            atributoEscolaIncremento = Atributo.NumeroDeFichasDeAplicacao_TotalPreenchidasDia3;
                        }

                        break;
                    }
                }

                //ACREDITAR QUE O USUÁRIO SELECIONOU A DATA CORRETA DA APLICAÇÃO DA PROVA CAUSOU PROBLEMAS. O INDICADOR AGORA SE BASEIA NA PROVA SELECIONADA
                //if (DataAplicacao > DateTime.Today)
                //    //No caso do usuário ter selecionado uma data superior a data atual, consideramos a data atual.
                //    DiaDoPreenchimentoDuranteAplicacao = (int)(System.DateTime.Today - DataInicioAplicacao).TotalDays + 1;
                //else
                //    DiaDoPreenchimentoDuranteAplicacao = (int)(DataAplicacao - DataInicioAplicacao).TotalDays + 1;

                //if (DiaDoPreenchimentoDuranteAplicacao <= 0)
                //    DiaDoPreenchimentoDuranteAplicacao = 1;

                if (TurmaAtributoRetornarValor(Edicao, (int)preenchimentoDeQuestionario.tur_id, atributoTurma, dbContextTransaction, conn) == "0")
                {
                    //Registro do recebimento da ficha de aplicação para a Turma
                    DataAcompanhamentoAplicacao.TurmaAtributoMarcarVerdadeiro(Edicao, (int)preenchimentoDeQuestionario.tur_id, atributoTurma, dbContextTransaction, conn);
                }
            }
            else if (tipoQuestionario == TipoQuestionario.FichaRegistroCoordenadorPedagogico && !string.IsNullOrEmpty(preenchimentoDeQuestionario.esc_codigo))
            {
                if (PessoaAtributoRetornarValor(Edicao, preenchimentoDeQuestionario.usu_id, preenchimentoDeQuestionario.esc_codigo,
                    Atributo.FichaAplicacaoDeCoordenadorPreenchida, dbContextTransaction, conn) == "0")
                {
                    DataAcompanhamentoAplicacao.PessoaAtributoMarcarVerdadeiro(Edicao, (int)TipoPerfil.CoordenadorPedagogico, preenchimentoDeQuestionario.usu_id,
                        preenchimentoDeQuestionario.esc_codigo, Atributo.FichaAplicacaoDeCoordenadorPreenchida, dbContextTransaction, conn);
                    atributoEscolaIncremento = Atributo.NumeroDeFichasDeCoordenador_TotalPreenchidos;
                }
            }
            else if (tipoQuestionario == TipoQuestionario.FichaRegistroDiretor && !string.IsNullOrEmpty(preenchimentoDeQuestionario.esc_codigo))
            {
                if (PessoaAtributoRetornarValor(Edicao, preenchimentoDeQuestionario.usu_id, preenchimentoDeQuestionario.esc_codigo,
                    Atributo.FichaAplicacaoDeDiretorPreenchida, dbContextTransaction, conn) == "0")
                {
                    DataAcompanhamentoAplicacao.PessoaAtributoMarcarVerdadeiro(Edicao, (int)TipoPerfil.Diretor, preenchimentoDeQuestionario.usu_id,
                        preenchimentoDeQuestionario.esc_codigo, Atributo.FichaAplicacaoDeDiretorPreenchida, dbContextTransaction, conn);
                    atributoEscolaIncremento = Atributo.NumeroDeFichasDeDiretor_TotalPreenchidos;
                }
            }
            else if (tipoQuestionario == TipoQuestionario.FichaRegistroSupervisor)
            {
                foreach (var resposta in preenchimentoDeQuestionario.Respostas)
                {
                    if (resposta.Numero == "2") //" Escola Supervisionada. [esc_codigo]" (QuestionarioItemID=10)
                    {
                        preenchimentoDeQuestionario.esc_codigo = resposta.Valor;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(preenchimentoDeQuestionario.esc_codigo))
                {
                    DataAcompanhamentoAplicacao.EscolaAtributoMarcarVerdadeiro(Edicao, preenchimentoDeQuestionario.esc_codigo,
                        Atributo.EscolaSupervisionada, dbContextTransaction, conn);
                }
            }
            else if (tipoQuestionario == TipoQuestionario.QuestionarioCoordenadorPedagogico)
            {
                if (PessoaAtributoRetornarValor(Edicao, preenchimentoDeQuestionario.usu_id, preenchimentoDeQuestionario.esc_codigo,
                    Atributo.QuestionarioDeCoordenadorPreenchido, dbContextTransaction, conn) == "0")
                {
                    DataAcompanhamentoAplicacao.PessoaAtributoMarcarVerdadeiro(Edicao, (int)TipoPerfil.CoordenadorPedagogico, preenchimentoDeQuestionario.usu_id,
                        preenchimentoDeQuestionario.esc_codigo, Atributo.QuestionarioDeCoordenadorPreenchido, dbContextTransaction, conn);
                    atributoEscolaIncremento = Atributo.NumeroDeQuestionariosDeCoordenador_TotalPreenchidos;
                }
            }
            else if (tipoQuestionario == TipoQuestionario.QuestionarioDiretor && !string.IsNullOrEmpty(preenchimentoDeQuestionario.esc_codigo))
            {
                if (PessoaAtributoRetornarValor(Edicao, preenchimentoDeQuestionario.usu_id, preenchimentoDeQuestionario.esc_codigo,
                    Atributo.QuestionarioDeDiretorPreenchido, dbContextTransaction, conn) == "0")
                {
                    DataAcompanhamentoAplicacao.PessoaAtributoMarcarVerdadeiro(Edicao, (int)TipoPerfil.Diretor, preenchimentoDeQuestionario.usu_id,
                        preenchimentoDeQuestionario.esc_codigo, Atributo.QuestionarioDeDiretorPreenchido, dbContextTransaction, conn);
                    atributoEscolaIncremento = Atributo.NumeroDeQuestionariosDeDiretor_TotalPreenchidos;
                }
            }
            else if (tipoQuestionario == TipoQuestionario.QuestionarioAssistenteDiretoria && !string.IsNullOrEmpty(preenchimentoDeQuestionario.esc_codigo))
            {
                if (PessoaAtributoRetornarValor(Edicao, preenchimentoDeQuestionario.usu_id, preenchimentoDeQuestionario.esc_codigo,
                    Atributo.QuestionarioAssistenteDiretoriaPreenchido, dbContextTransaction, conn) == "0")
                {
                    DataAcompanhamentoAplicacao.PessoaAtributoMarcarVerdadeiro(Edicao, (int)TipoPerfil.Diretor, preenchimentoDeQuestionario.usu_id,
                        preenchimentoDeQuestionario.esc_codigo, Atributo.QuestionarioAssistenteDiretoriaPreenchido, dbContextTransaction, conn);
                    atributoEscolaIncremento = Atributo.NumeroDeQuestionariosDeAssistenteDiretoria_TotalPreenchidos;
                }
            }
            else if (tipoQuestionario == TipoQuestionario.QuestionarioAuxiliarTecnicoEducacao && !string.IsNullOrEmpty(preenchimentoDeQuestionario.esc_codigo))
            {
                if (PessoaAtributoRetornarValor(Edicao, preenchimentoDeQuestionario.usu_id, preenchimentoDeQuestionario.esc_codigo,
                    Atributo.QuestionarioAuxiliarTecnicoEducacaoPreenchido, dbContextTransaction, conn) == "0")
                {
                    DataAcompanhamentoAplicacao.PessoaAtributoMarcarVerdadeiro(Edicao, (int)TipoPerfil.Diretor, preenchimentoDeQuestionario.usu_id,
                        preenchimentoDeQuestionario.esc_codigo, Atributo.QuestionarioAuxiliarTecnicoEducacaoPreenchido, dbContextTransaction, conn);
                    atributoEscolaIncremento = Atributo.NumeroDeQuestionariosDeAuxiliarTecnicoEducacao_TotalPreenchidos;
                }
            }
            else if (tipoQuestionario == TipoQuestionario.QuestionarioAgenteEscolarMerendeira && !string.IsNullOrEmpty(preenchimentoDeQuestionario.esc_codigo))
            {
                if (PessoaAtributoRetornarValor(Edicao, preenchimentoDeQuestionario.usu_id, preenchimentoDeQuestionario.esc_codigo,
                    Atributo.QuestionarioAgenteEscolarMerendeiraPreenchido, dbContextTransaction, conn) == "0")
                {
                    DataAcompanhamentoAplicacao.PessoaAtributoMarcarVerdadeiro(Edicao, (int)TipoPerfil.Diretor, preenchimentoDeQuestionario.usu_id,
                        preenchimentoDeQuestionario.esc_codigo, Atributo.QuestionarioAgenteEscolarMerendeiraPreenchido, dbContextTransaction, conn);
                    atributoEscolaIncremento = Atributo.NumeroDeQuestionariosDeAgenteEscolarMerendeira_TotalPreenchidos;
                }
            }
            else if (tipoQuestionario == TipoQuestionario.QuestionarioAgenteEscolarPortaria && !string.IsNullOrEmpty(preenchimentoDeQuestionario.esc_codigo))
            {
                if (PessoaAtributoRetornarValor(Edicao, preenchimentoDeQuestionario.usu_id, preenchimentoDeQuestionario.esc_codigo,
                    Atributo.QuestionarioAgenteEscolarPortariaPreenchido, dbContextTransaction, conn) == "0")
                {
                    DataAcompanhamentoAplicacao.PessoaAtributoMarcarVerdadeiro(Edicao, (int)TipoPerfil.Diretor, preenchimentoDeQuestionario.usu_id,
                        preenchimentoDeQuestionario.esc_codigo, Atributo.QuestionarioAgenteEscolarPortariaPreenchido, dbContextTransaction, conn);
                    atributoEscolaIncremento = Atributo.NumeroDeQuestionariosDeAgenteEscolarPortaria_TotalPreenchidos;
                }
            }
            else if (tipoQuestionario == TipoQuestionario.QuestionarioAgenteEscolarZeladoria && !string.IsNullOrEmpty(preenchimentoDeQuestionario.esc_codigo))
            {
                if (PessoaAtributoRetornarValor(Edicao, preenchimentoDeQuestionario.usu_id, preenchimentoDeQuestionario.esc_codigo,
                    Atributo.QuestionarioAgenteEscolarZeladoriaPreenchido, dbContextTransaction, conn) == "0")
                {
                    DataAcompanhamentoAplicacao.PessoaAtributoMarcarVerdadeiro(Edicao, (int)TipoPerfil.Diretor, preenchimentoDeQuestionario.usu_id,
                        preenchimentoDeQuestionario.esc_codigo, Atributo.QuestionarioAgenteEscolarZeladoriaPreenchido, dbContextTransaction, conn);
                    atributoEscolaIncremento = Atributo.NumeroDeQuestionariosDeAgenteEscolarZeladoria_TotalPreenchidos;
                }
            }
            else if (tipoQuestionario == TipoQuestionario.QuestionarioSupervisor)
            {
                if (PessoaAtributoRetornarValor(Edicao, preenchimentoDeQuestionario.usu_id, "0", Atributo.QuestionarioDeSupervisorPreenchido, dbContextTransaction, conn) == "0")
                {
                    preenchimentoDeQuestionario.esc_codigo = "0";
                    DataAcompanhamentoAplicacao.PessoaAtributoMarcarVerdadeiro(Edicao, (int)TipoPerfil.Supervisor, preenchimentoDeQuestionario.usu_id,
                        "0", Atributo.QuestionarioDeSupervisorPreenchido, dbContextTransaction, conn);
                    atributoEscolaIncremento = Atributo.NumeroDeQuestionariosDeSupervisor_TotalPreenchidos;
                }
            }
            else if (tipoQuestionario == TipoQuestionario.QuestionarioProfessor && !string.IsNullOrEmpty(preenchimentoDeQuestionario.esc_codigo))
            {
                if (PessoaAtributoRetornarValor(Edicao, preenchimentoDeQuestionario.usu_id, preenchimentoDeQuestionario.esc_codigo,
                    Atributo.QuestionarioDeProfessorPreenchido, dbContextTransaction, conn) == "0")
                {
                    DataAcompanhamentoAplicacao.PessoaAtributoMarcarVerdadeiro(Edicao, (int)TipoPerfil.Professor, preenchimentoDeQuestionario.usu_id,
                        preenchimentoDeQuestionario.esc_codigo, Atributo.QuestionarioDeProfessorPreenchido, dbContextTransaction, conn);
                    atributoEscolaIncremento = Atributo.NumeroDeQuestionariosDeProfessor_TotalPreenchidos;
                }
            }
            /* Edição 2k18
            else if (tipoQuestionario == TipoQuestionario.QuestionarioAlunos3Ano && !string.IsNullOrEmpty(preenchimentoDeQuestionario.esc_codigo))
            {
                if (PessoaAtributoRetornarValor(Edicao, preenchimentoDeQuestionario.usu_id, preenchimentoDeQuestionario.esc_codigo, "QuestionarioAlunos3AnoPreenchido", dbContextTransaction, conn) == "0")
                {
                    DataAcompanhamentoAplicacao.PessoaAtributoMarcarVerdadeiro(Edicao, (int)TipoPerfil.Aluno, preenchimentoDeQuestionario.usu_id, preenchimentoDeQuestionario.esc_codigo, "QuestionarioAlunos3AnoPreenchido", dbContextTransaction, conn);
                    nomeAtributoEscolaIncremento = "NumeroDeQuestionariosAlunos3Ano_TotalPreenchidos";
                }
            }
            else if (tipoQuestionario == TipoQuestionario.QuestionarioAlunos4AnoAo6Ano && !string.IsNullOrEmpty(preenchimentoDeQuestionario.esc_codigo))
            {
                if (PessoaAtributoRetornarValor(Edicao, preenchimentoDeQuestionario.usu_id, preenchimentoDeQuestionario.esc_codigo, "QuestionarioAlunos4AnoAo6AnoPreenchido", dbContextTransaction, conn) == "0")
                {
                    DataAcompanhamentoAplicacao.PessoaAtributoMarcarVerdadeiro(Edicao, (int)TipoPerfil.Aluno, preenchimentoDeQuestionario.usu_id, preenchimentoDeQuestionario.esc_codigo, "QuestionarioAlunos4AnoAo6AnoPreenchido", dbContextTransaction, conn);
                    nomeAtributoEscolaIncremento = "NumeroDeQuestionariosAlunos4AnoAo6Ano_TotalPreenchidos";
                }
            }*/
            else if (tipoQuestionario == TipoQuestionario.QuestionarioAlunos3AnoAo6Ano && !string.IsNullOrEmpty(preenchimentoDeQuestionario.esc_codigo))
            {
                if (PessoaAtributoRetornarValor(Edicao, preenchimentoDeQuestionario.usu_id, preenchimentoDeQuestionario.esc_codigo,
                    Atributo.QuestionarioAlunos4AnoAo6AnoPreenchido, dbContextTransaction, conn) == "0")
                {
                    DataAcompanhamentoAplicacao.PessoaAtributoMarcarVerdadeiro(Edicao, (int)TipoPerfil.Aluno, preenchimentoDeQuestionario.usu_id,
                        preenchimentoDeQuestionario.esc_codigo, Atributo.QuestionarioAlunos4AnoAo6AnoPreenchido, dbContextTransaction, conn);
                    atributoEscolaIncremento = Atributo.NumeroDeQuestionariosAlunos4AnoAo6Ano_TotalPreenchidos;
                }
            }
            else if (tipoQuestionario == TipoQuestionario.QuestionarioAlunos7AnoAo9Ano && !string.IsNullOrEmpty(preenchimentoDeQuestionario.esc_codigo))
            {
                if (PessoaAtributoRetornarValor(Edicao, preenchimentoDeQuestionario.usu_id, preenchimentoDeQuestionario.esc_codigo,
                    Atributo.QuestionarioAlunos7AnoAo9AnoPreenchido, dbContextTransaction, conn) == "0")
                {
                    DataAcompanhamentoAplicacao.PessoaAtributoMarcarVerdadeiro(Edicao, (int)TipoPerfil.Aluno, preenchimentoDeQuestionario.usu_id,
                        preenchimentoDeQuestionario.esc_codigo, Atributo.QuestionarioAlunos7AnoAo9AnoPreenchido, dbContextTransaction, conn);
                    atributoEscolaIncremento = Atributo.NumeroDeQuestionariosAlunos7AnoAo9Ano_TotalPreenchidos;
                }
            }

            if (atributoEscolaIncremento.HasValue)
            {
                //Contabilização do preenchimento do questionário/ficha para a Escola
                DataAcompanhamentoAplicacao.EscolaAtributoIncrementar(Edicao, preenchimentoDeQuestionario.esc_codigo, atributoEscolaIncremento.Value, dbContextTransaction, conn);
            }
        }

        private static void EscolaAtributoMarcarVerdadeiro(string Edicao, string esc_codigo, Atributo atributoID, SqlTransaction dbContextTransaction, SqlConnection conn)
        {
            conn.Execute(
                        sql: @"
                    IF (NOT EXISTS(SELECT * FROM AcompanhamentoAplicacaoEscola WITH (NOLOCK) WHERE Edicao=@Edicao AND esc_codigo=@esc_codigo AND AtributoID=@AtributoID))
                        INSERT INTO AcompanhamentoAplicacaoEscola (Edicao, esc_codigo, AtributoID, Valor) VALUES (@Edicao, @esc_codigo, @AtributoID, '1')
                    ELSE
                        UPDATE AcompanhamentoAplicacaoEscola SET Valor = '1' WHERE Edicao=@Edicao AND esc_codigo=@esc_codigo AND AtributoID=@AtributoID
                    ",
                        param: new
                        {
                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                            esc_codigo = new DbString() { Value = esc_codigo, IsAnsi = true, Length = 20 },
                            AtributoID = (int)atributoID
                        },
                        transaction: dbContextTransaction);
        }

        private static void EscolaAtributoIncrementar(string Edicao, string esc_codigo, Atributo atributoID, SqlTransaction dbContextTransaction, SqlConnection conn)
        {
            conn.Execute(
                        sql: @"
                    IF (NOT EXISTS(SELECT * FROM AcompanhamentoAplicacaoEscola WITH (NOLOCK) WHERE Edicao=@Edicao AND esc_codigo=@esc_codigo AND AtributoID=@AtributoID))
                        INSERT INTO AcompanhamentoAplicacaoEscola (Edicao, esc_codigo, AtributoID, Valor) VALUES (@Edicao, @esc_codigo, @AtributoID, '1')
                    ELSE
                        UPDATE AcompanhamentoAplicacaoEscola SET Valor = CAST(CAST(Valor AS INT)+1 AS varchar(150)) WHERE Edicao=@Edicao AND esc_codigo=@esc_codigo AND AtributoID=@AtributoID
                    ",
                        param: new
                        {
                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                            esc_codigo = new DbString() { Value = esc_codigo, IsAnsi = true, Length = 20 },
                            AtributoID = (int)atributoID
                        },
                        transaction: dbContextTransaction);
        }

        private static void TurmaAtributoMarcarVerdadeiro(string Edicao, int tur_id, Atributo atributoID, SqlTransaction dbContextTransaction, SqlConnection conn)
        {
            conn.Execute(
                        sql: @"
                    IF (NOT EXISTS(SELECT * FROM AcompanhamentoAplicacaoTurma WITH (NOLOCK) WHERE Edicao=@Edicao AND tur_id=@tur_id AND AtributoID=@AtributoID))
                        INSERT INTO AcompanhamentoAplicacaoTurma (Edicao, tur_id, AtributoID, Valor) VALUES (@Edicao, @tur_id, @AtributoID, '1')
                    ELSE
                        UPDATE AcompanhamentoAplicacaoTurma SET Valor = '1' WHERE Edicao=@Edicao AND tur_id=@tur_id AND AtributoID=@AtributoID
                    ",
                        param: new
                        {
                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                            tur_id = tur_id,
                            AtributoID = (int)atributoID
                        },
                        transaction: dbContextTransaction);
        }

        private static string TurmaAtributoRetornarValor(string Edicao, int tur_id, Atributo atributoID, SqlTransaction dbContextTransaction, SqlConnection conn)
        {
            return conn.ExecuteScalar<string>(
                        sql: @"
                                IF EXISTS(SELECT * FROM AcompanhamentoAplicacaoTurma WITH (NOLOCK) WHERE Edicao=@Edicao AND tur_id=@tur_id AND AtributoID=@AtributoID)
                                    SELECT Valor FROM AcompanhamentoAplicacaoTurma WITH (NOLOCK) WHERE Edicao=@Edicao AND tur_id=@tur_id AND AtributoID=@AtributoID
                                ELSE
                                    SELECT '0'
",
                        param: new
                        {
                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                            tur_id = tur_id,
                            AtributoID = (int)atributoID
                        },
                        transaction: dbContextTransaction);
        }

        private static string PessoaAtributoRetornarValor(string Edicao, string usu_id, string esc_codigo, Atributo atributoID, SqlTransaction dbContextTransaction, SqlConnection conn)
        {
            return conn.ExecuteScalar<string>(
                        sql: @"
                                IF EXISTS(SELECT Valor FROM AcompanhamentoAplicacaoPessoa WITH (NOLOCK) WHERE Edicao=@Edicao AND esc_codigo=@esc_codigo AND usu_id=@usu_id AND AtributoID=@AtributoID)
                                    SELECT Valor FROM AcompanhamentoAplicacaoPessoa WITH (NOLOCK) WHERE Edicao=@Edicao AND esc_codigo=@esc_codigo AND usu_id=@usu_id AND AtributoID=@AtributoID
                                ELSE
                                    SELECT '0'
",
                        param: new
                        {
                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                            esc_codigo = new DbString() { Value = esc_codigo, IsAnsi = true, Length = 20 },
                            usu_id = usu_id,
                            AtributoID = (int)atributoID
                        },
                        transaction: dbContextTransaction);
        }

        private static void PessoaAtributoMarcarVerdadeiro(string Edicao, int PerfilID, string usu_id, string esc_codigo, Atributo atributoID, SqlTransaction dbContextTransaction, SqlConnection conn)
        {
            conn.Execute(
                        sql: @"
                    IF (NOT EXISTS(SELECT Valor FROM AcompanhamentoAplicacaoPessoa WITH (NOLOCK) WHERE Edicao=@Edicao AND esc_codigo=@esc_codigo AND usu_id=@usu_id AND AtributoID=@AtributoID))
                        INSERT INTO AcompanhamentoAplicacaoPessoa (Edicao, esc_codigo, usu_id, PerfilID, AtributoID, Valor) VALUES (@Edicao, @esc_codigo, @usu_id, @PerfilID, @AtributoID, '1')
                    ELSE
                        UPDATE AcompanhamentoAplicacaoPessoa SET Valor = '1' WHERE Edicao=@Edicao AND esc_codigo=@esc_codigo AND usu_id=@usu_id AND AtributoID=@AtributoID
                    ",
                        param: new
                        {
                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                            esc_codigo = new DbString() { Value = esc_codigo, IsAnsi = true, Length = 20 },
                            usu_id = usu_id,
                            AtributoID = (int)atributoID,
                            PerfilID = PerfilID
                        },
                        transaction: dbContextTransaction);
        }
    }
}
