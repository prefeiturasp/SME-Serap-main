using Dapper;
using ProvaSP.Data.Funcionalidades;
using ProvaSP.Model.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvaSP.Data
{
    public static class DataResultado
    {
        public static Resultado RecuperarResultadoSME(string Edicao, int AreaConhecimentoID, string AnoEscolar)
        {
            var resultado = new Resultado();

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                conn.Open();

                resultado.Agregacao = conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT 'SME' AS Titulo, 
                                                'SME' AS Chave, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoSme WITH (NOLOCK)
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND AnoEscolar=@AnoEscolar",
                                        param: new
                                        {
                                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                                            AreaConhecimentoID = AreaConhecimentoID,
                                            AnoEscolar = new DbString() { Value = AnoEscolar, IsAnsi = true, Length = 3 }
                                        }).ToList<ResultadoItem>();

                resultado.Itens = new List<ResultadoItem>();

                resultado.Itens.AddRange( //NÍVEL SME
                    conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT 'MÉDIA DA SME' AS Titulo, 
                                                'SME' AS Chave, 
                                                AnoEscolar, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos,
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoSme WITH (NOLOCK)
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND AnoEscolar=@AnoEscolar",
                                        param: new
                                        {
                                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                                            AreaConhecimentoID = AreaConhecimentoID,
                                            AnoEscolar = new DbString() { Value = AnoEscolar, IsAnsi = true, Length = 3 }
                                        }
                                    ).ToList<ResultadoItem>()
                );

                var resultadoItens_DRE = conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT NivelProficienciaID, 
                                                uad_sigla AS Chave, 
                                                '' AS Titulo, 
                                                TotalAlunos, 
                                                Valor, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoDre WITH (NOLOCK)
                                            WHERE Edicao=@Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND AnoEscolar=@AnoEscolar",
                                        param: new
                                        {
                                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                                            AreaConhecimentoID = AreaConhecimentoID,
                                            AnoEscolar = new DbString() { Value = AnoEscolar, IsAnsi = true, Length = 3 }

                                        }).ToList<ResultadoItem>();

                foreach (var dre in resultadoItens_DRE)
                {
                    dre.Titulo = DataDRE.RecuperarNome(dre.Chave);
                }

                resultado.Itens.AddRange(resultadoItens_DRE);


                var habilidades = conn.Query<HabilidadeTema, HabilidadeItem, HabilidadeTema>(
                        sql: @"
                                SELECT
                                    sme.HabilidadeCategoria AS Titulo,
                                    'split' AS Split,
                                    sme.HabilidadeCodigo AS Codigo,
                                    sme.HabilidadeDescricao AS Descricao,
                                    sme.PercentualAcertos AS PercentualAcertosNivelSME,
                                    -1 AS PercentualAcertosNivelDRE,
                                    -1 AS PercentualAcertosNivelEscola,
                                    -1 AS PercentualAcertosNivelTurma
                                FROM HabilidadeSme sme
                                WHERE sme.Edicao=@Edicao AND sme.AreaConhecimentoID=@AreaConhecimentoID AND sme.AnoEscolar=@AnoEscolar
                            ",
                        map: (_tema, _item) =>
                        {
                            _tema.Itens.Add(_item);
                            return _tema;
                        },
                        splitOn: "Split",
                        param: new
                        {
                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                            AreaConhecimentoID = AreaConhecimentoID,
                            AnoEscolar = new DbString() { Value = AnoEscolar, IsAnsi = true, Length = 3 }
                        }
                    ).ToList<HabilidadeTema>();


                resultado.Habilidades = OrganizarHabilidades(habilidades);
            }

            return resultado;
        }

        private static List<HabilidadeTema> OrganizarHabilidades(List<HabilidadeTema> habilidades)
        {
            var retorno = new List<HabilidadeTema>();
            foreach (var habilidade in habilidades)
            {
                var habilidadeTema = retorno.Find(x => x.Titulo == habilidade.Titulo);
                if (habilidadeTema == null)
                {
                    retorno.Add(habilidade);
                }
                else
                {
                    habilidadeTema.Itens.Add(habilidade.Itens[0]);
                }
            }
            return retorno;
        }

        public static Resultado RecuperarResultadoDRE(string Edicao, int AreaConhecimentoID, string AnoEscolar, string lista_uad_sigla)
        {
            var resultado = new Resultado();

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                var parametros = new DynamicParameters();

                parametros.Add("Edicao", Edicao, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 10);
                parametros.Add("AnoEscolar", AnoEscolar, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 3);
                parametros.Add("AreaConhecimentoID", AreaConhecimentoID, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                var sbDREs = new StringBuilder();
                var lista_uad_siglaSplit = lista_uad_sigla.Split(',');
                if (lista_uad_siglaSplit.Length == 0)
                {
                    return resultado;
                }

                //Construção dos parâmetros passados pela variável lista_uad_sigla de modo a evitar sql injection.
                foreach (var uad_sigla in lista_uad_siglaSplit)
                {
                    if (sbDREs.Length > 0)
                    {
                        sbDREs.Append(",");
                    }
                    string parameterName = "@p_" + uad_sigla;
                    sbDREs.Append(parameterName);
                    parametros.Add(parameterName, uad_sigla, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 4);
                }

                conn.Open();

                resultado.Agregacao = conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT '' AS Titulo, 
                                                uad_sigla AS Chave, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoDre WITH (NOLOCK)
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND AnoEscolar=@AnoEscolar AND uad_sigla IN(" + sbDREs.ToString() + @") ",
                                        param: parametros
                                    ).ToList<ResultadoItem>();


                resultado.Itens = new List<ResultadoItem>();

                resultado.Itens.AddRange( //NÍVEL SME
                    conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT 'MÉDIA DA SME' AS Titulo, 
                                                'SME' AS Chave, 
                                                AnoEscolar, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoSme WITH (NOLOCK)
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND AnoEscolar=@AnoEscolar",
                                        param: parametros
                                    ).ToList<ResultadoItem>()
                );

                var resultadoItens_DRE = conn.Query<ResultadoItem>(
                                            sql: @"
                                            SELECT NivelProficienciaID, 
                                                uad_sigla AS Chave, 
                                                AnoEscolar, 
                                                '' AS Titulo, 
                                                TotalAlunos, 
                                                Valor, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoDre WITH (NOLOCK)
                                            WHERE Edicao=@Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND AnoEscolar=@AnoEscolar AND uad_sigla IN(" + sbDREs.ToString() + @")
                                            ",
                                            param: parametros
                                        ).ToList<ResultadoItem>();
                foreach (var dre in resultadoItens_DRE)
                {
                    dre.Titulo = "MÉDIA DA DRE " + DataDRE.RecuperarNome(dre.Chave);
                }
                resultado.Itens.AddRange( //NÍVEL DRE
                        resultadoItens_DRE
                );

                resultado.Itens.AddRange(
                    conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT r.NivelProficienciaID, 
                                                r.esc_codigo AS Chave, 
                                                '(' + r.uad_sigla + ') ' +e.esc_nome AS Titulo, 
                                                r.TotalAlunos, 
                                                r.Valor, 
                                                r.PercentualAbaixoDoBasico, 
                                                r.PercentualBasico, 
                                                r.PercentualAdequado, 
                                                r.PercentualAvancado, 
                                                r.PercentualAlfabetizado
                                            FROM ResultadoEscola r (NOLOCK)
                                            JOIN Escola (NOLOCK) e ON e.esc_codigo = r.esc_codigo 
                                            WHERE r.Edicao=@Edicao AND r.AreaConhecimentoID=@AreaConhecimentoID AND r.AnoEscolar=@AnoEscolar AND r.uad_sigla IN(" + sbDREs.ToString() + @") 
                                            ORDER BY r.uad_sigla, e.esc_nome
                                            ",
                                        param: parametros
                                    ).ToList<ResultadoItem>()
                );


                var habilidades = conn.Query<HabilidadeTema, HabilidadeItem, HabilidadeTema>(
                        sql: @"
                                SELECT
                                    sme.HabilidadeCategoria AS Titulo,
                                    'split' AS Split,
                                    dre.uad_sigla AS OrigemTitulo,
                                    sme.HabilidadeCodigo AS Codigo,
                                    sme.HabilidadeDescricao AS Descricao,
                                    sme.PercentualAcertos AS PercentualAcertosNivelSME,
                                    dre.PercentualAcertos AS PercentualAcertosNivelDRE,
                                    -1 AS PercentualAcertosNivelEscola,
                                    -1 AS PercentualAcertosNivelTurma
                                FROM HabilidadeSme sme
                                JOIN HabilidadeDre dre    ON sme.edicao=dre.edicao AND sme.AreaConhecimentoID=dre.AreaConhecimentoID AND sme.AnoEscolar=dre.AnoEscolar AND sme.HabilidadeCodigo=dre.HabilidadeCodigo
                                WHERE sme.Edicao=@Edicao AND sme.AreaConhecimentoID=@AreaConhecimentoID AND sme.AnoEscolar=@AnoEscolar AND dre.uad_sigla IN(" + sbDREs.ToString() + @") 
                            ",
                        map: (_tema, _item) =>
                        {
                            _tema.Itens.Add(_item);
                            return _tema;
                        },
                        splitOn: "Split",
                        param: parametros
                    ).ToList<HabilidadeTema>();

                resultado.Habilidades = OrganizarHabilidades(habilidades);

            }

            foreach (var dre in resultado.Agregacao)
            {
                dre.Titulo = DataDRE.RecuperarNome(dre.Chave);
            }

            if (lista_uad_sigla.Split(',').Length > 1)
            {
                foreach (var habilidadeTema in resultado.Habilidades)
                {
                    foreach (var habilidade in habilidadeTema.Itens)
                    {
                        habilidade.OrigemTitulo = DataDRE.RecuperarNome(habilidade.OrigemTitulo);
                    }
                }
            }

            return resultado;
        }

        public static Resultado RecuperarResultadoEscola(string Edicao, int AreaConhecimentoID, string AnoEscolar, string lista_esc_codigo)
        {
            var resultado = new Resultado();

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                var parametros = new DynamicParameters();

                parametros.Add("Edicao", Edicao, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 10);
                parametros.Add("AnoEscolar", AnoEscolar, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 3);
                parametros.Add("AreaConhecimentoID", AreaConhecimentoID, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                var sbEscolas = new StringBuilder();
                var lista_esc_codigoSplit = lista_esc_codigo.Split(',');
                if (lista_esc_codigoSplit.Length == 0)
                {
                    return resultado;
                }

                //Construção dos parâmetros passados pela variável lista_esc_codigo de modo a evitar sql injection.
                foreach (var esc_codigo in lista_esc_codigoSplit)
                {
                    if (sbEscolas.Length > 0)
                    {
                        sbEscolas.Append(",");
                    }
                    string parameterName = "@p_" + esc_codigo;
                    sbEscolas.Append(parameterName);
                    parametros.Add(parameterName, esc_codigo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 20);
                }

                conn.Open();

                resultado.Agregacao = conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT e.esc_nome AS Titulo, 
                                                r.esc_codigo AS Chave, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoEscola r (NOLOCK)
                                            JOIN Escola (NOLOCK) e ON e.esc_codigo = r.esc_codigo 
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND AnoEscolar=@AnoEscolar AND r.esc_codigo IN(" + sbEscolas.ToString() + @") ",
                                        param: parametros
                                    ).ToList<ResultadoItem>();

                //resultado.NivelProficienciaID = RetornarNivelProficienciaID(AreaConhecimentoID, AnoEscolar, resultado.Valor);


                string titulo = "r.tur_codigo AS Titulo";
                if (lista_esc_codigoSplit.Length > 1)
                {
                    titulo = "'(' + r.uad_sigla + ') ' +e.esc_nome + ' - ' + r.tur_codigo AS Titulo";
                }

                resultado.Itens = new List<ResultadoItem>();

                resultado.Itens.AddRange( //NÍVEL SME
                    conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT 'MÉDIA DA SME' AS Titulo, 
                                                'SME' AS Chave, 
                                                AnoEscolar, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoSme WITH (NOLOCK)
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND AnoEscolar=@AnoEscolar",
                                        param: parametros
                                    ).ToList<ResultadoItem>()
                );

                var resultadoItens_DRE = conn.Query<ResultadoItem>(
                                            sql: @"
                                            SELECT NivelProficienciaID, 
                                                uad_sigla AS Chave, 
                                                AnoEscolar, 
                                                '' AS Titulo, 
                                                TotalAlunos, 
                                                Valor, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoDre WITH (NOLOCK)
                                            WHERE Edicao=@Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND AnoEscolar=@AnoEscolar AND uad_sigla IN(SELECT DISTINCT uad_codigo FROM Escola WHERE esc_codigo IN(" + sbEscolas.ToString() + @"))
                                            ",
                                            param: parametros
                                        ).ToList<ResultadoItem>();
                foreach (var dre in resultadoItens_DRE)
                {
                    dre.Titulo = "MÉDIA DA DRE " + DataDRE.RecuperarNome(dre.Chave);
                }
                resultado.Itens.AddRange( //NÍVEL DRE
                        resultadoItens_DRE
                );

                resultado.Itens.AddRange(conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT r.NivelProficienciaID, 
                                                r.esc_codigo AS Chave, "
                                                + titulo + @", 
                                                r.TotalAlunos, 
                                                r.Valor, 
                                                r.PercentualAbaixoDoBasico, 
                                                r.PercentualBasico, 
                                                r.PercentualAdequado, 
                                                r.PercentualAvancado, 
                                                r.PercentualAlfabetizado
                                            FROM ResultadoTurma r (NOLOCK)
                                            JOIN Escola (NOLOCK) e ON e.esc_codigo = r.esc_codigo 
                                            WHERE r.Edicao=@Edicao AND r.AreaConhecimentoID=@AreaConhecimentoID AND r.AnoEscolar=@AnoEscolar AND r.esc_codigo IN(" + sbEscolas.ToString() + @") 
                                            ORDER BY r.uad_sigla, e.esc_nome
                                            ",
                                        param: parametros
                                    ).ToList<ResultadoItem>()
                );

                var habilidades = conn.Query<HabilidadeTema, HabilidadeItem, HabilidadeTema>(
                        sql: @"
                                SELECT
                                    sme.HabilidadeCategoria AS Titulo,
                                    'split' AS Split,
                                    e.esc_nome AS OrigemTitulo,
                                    sme.HabilidadeCodigo AS Codigo,
                                    sme.HabilidadeDescricao AS Descricao,
                                    sme.PercentualAcertos AS PercentualAcertosNivelSME,
                                    dre.PercentualAcertos AS PercentualAcertosNivelDRE,
                                    esc.PercentualAcertos AS PercentualAcertosNivelEscola,
                                    -1 AS PercentualAcertosNivelTurma
                                FROM HabilidadeSme sme
                                JOIN HabilidadeDre dre    ON sme.edicao=dre.edicao AND sme.AreaConhecimentoID=dre.AreaConhecimentoID AND sme.AnoEscolar=dre.AnoEscolar AND sme.HabilidadeCodigo=dre.HabilidadeCodigo
                                JOIN HabilidadeEscola esc ON dre.edicao=esc.edicao AND dre.AreaConhecimentoID=esc.AreaConhecimentoID AND dre.AnoEscolar=esc.AnoEscolar AND dre.HabilidadeCodigo=esc.HabilidadeCodigo AND dre.uad_sigla=esc.uad_sigla
                                JOIN Escola e ON e.esc_codigo=esc.esc_codigo
                                WHERE sme.Edicao=@Edicao AND sme.AreaConhecimentoID=@AreaConhecimentoID AND sme.AnoEscolar=@AnoEscolar AND esc.esc_codigo IN(" + sbEscolas.ToString() + @") 
                            ",
                        map: (_tema, _item) =>
                        {
                            _tema.Itens.Add(_item);
                            return _tema;
                        },
                        splitOn: "Split",
                        param: parametros
                    ).ToList<HabilidadeTema>();

                resultado.Habilidades = OrganizarHabilidades(habilidades);
            }

            return resultado;
        }

        public static Resultado RecuperarResultadoTurma(string Edicao, int AreaConhecimentoID, string AnoEscolar, string lista_esc_codigo, string lista_turmas)
        {
            var resultado = new Resultado();

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                var parametros = new DynamicParameters();

                parametros.Add("Edicao", Edicao, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 10);
                parametros.Add("AnoEscolar", AnoEscolar, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 3);
                parametros.Add("AreaConhecimentoID", AreaConhecimentoID, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                var sbEscolas = new StringBuilder();
                var lista_esc_codigoSplit = lista_esc_codigo.Split(',');
                if (lista_esc_codigoSplit.Length == 0)
                {
                    return resultado;
                }

                //Construção dos parâmetros passados pela variável lista_esc_codigo de modo a evitar sql injection.
                foreach (var esc_codigo in lista_esc_codigoSplit)
                {
                    if (sbEscolas.Length > 0)
                    {
                        sbEscolas.Append(",");
                    }
                    string parameterName = "@p_" + esc_codigo;
                    sbEscolas.Append(parameterName);
                    parametros.Add(parameterName, esc_codigo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 20);
                }

                var sbTurmas = new StringBuilder();
                var lista_turmasSplit = lista_turmas.Split(',');
                if (lista_turmasSplit.Length == 0)
                {
                    return resultado;
                }

                //Construção dos parâmetros passados pela variável lista_turmas de modo a evitar sql injection.
                foreach (var tur_codigo in lista_turmasSplit)
                {
                    if (sbTurmas.Length > 0)
                    {
                        sbTurmas.Append(",");
                    }
                    string parameterName = "@p_" + tur_codigo;
                    sbTurmas.Append(parameterName);
                    parametros.Add(parameterName, tur_codigo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 20);
                }

                conn.Open();



                resultado.Agregacao = conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT e.esc_nome + ' - ' + r.tur_codigo AS Titulo, 
                                                r.tur_codigo AS Chave, 
                                                NivelProficienciaID, 
                                                COALESCE(Valor,0) AS Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoTurma r WITH (NOLOCK)
                                            JOIN Escola (NOLOCK) e ON e.esc_codigo = r.esc_codigo 
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND AnoEscolar=@AnoEscolar AND r.esc_codigo IN(" + sbEscolas.ToString() + ") AND tur_codigo IN(" + sbTurmas.ToString() + @") ",
                                        param: parametros
                                    ).ToList<ResultadoItem>();

                //resultado.NivelProficienciaID = RetornarNivelProficienciaID(AreaConhecimentoID, AnoEscolar, resultado.Valor);

                string titulo = "'(' + tur_codigo + ') ' + r.alu_nome + ' (' + r.alu_matricula + ')' AS Titulo";
                if (lista_esc_codigoSplit.Length > 1)
                {
                    titulo = "'(' + LEFT(e.esc_nome, 20) + '... ' + tur_codigo + ') ' + r.alu_nome + ' (' + r.alu_matricula + ')' AS Titulo";
                }

                resultado.Itens = new List<ResultadoItem>();

                resultado.Itens.AddRange( //NÍVEL SME
                    conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT 'MÉDIA DA SME' AS Titulo, 
                                                'SME' AS Chave, 
                                                AnoEscolar, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoSme WITH (NOLOCK)
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND AnoEscolar=@AnoEscolar",
                                        param: parametros
                                    ).ToList<ResultadoItem>()
                );

                var resultadoItens_DRE = conn.Query<ResultadoItem>(
                                            sql: @"
                                            SELECT NivelProficienciaID, 
                                                uad_sigla AS Chave, 
                                                AnoEscolar, 
                                                '' AS Titulo, 
                                                TotalAlunos, 
                                                Valor, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoDre WITH (NOLOCK)
                                            WHERE Edicao=@Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND AnoEscolar=@AnoEscolar AND uad_sigla IN(SELECT DISTINCT uad_codigo FROM Escola WHERE esc_codigo IN(" + sbEscolas.ToString() + @"))
                                            ",
                                            param: parametros
                                        ).ToList<ResultadoItem>();
                foreach (var dre in resultadoItens_DRE)
                {
                    dre.Titulo = "MÉDIA DA DRE " + DataDRE.RecuperarNome(dre.Chave);
                }
                resultado.Itens.AddRange( //NÍVEL DRE
                        resultadoItens_DRE
                );

                resultado.Itens.AddRange( //NÍVEL ESCOLA
                    conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT 'MÉDIA DA ESCOLA ' + e.esc_nome AS Titulo, 
                                                r.esc_codigo AS Chave, 
                                                r.AnoEscolar, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoEscola r (NOLOCK)
                                            JOIN Escola (NOLOCK) e ON e.esc_codigo = r.esc_codigo 
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND AnoEscolar=@AnoEscolar AND r.esc_codigo IN(" + sbEscolas.ToString() + @")
                                            ",
                                        param: parametros
                                    ).ToList<ResultadoItem>()
                    );

                resultado.Itens.AddRange( //NÍVEL TURMA
                                    conn.Query<ResultadoItem>(
                                                        sql: @"
                                            SELECT 'MÉDIA DA TURMA ' + e.esc_nome + ' (' + r.tur_codigo + ')' AS Titulo, 
                                                r.tur_codigo AS Chave, 
                                                r.AnoEscolar, 
                                                NivelProficienciaID, 
                                                COALESCE(Valor,0) AS Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoTurma r WITH (NOLOCK)
                                            JOIN Escola (NOLOCK) e ON e.esc_codigo = r.esc_codigo 
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND AnoEscolar=@AnoEscolar AND r.esc_codigo IN(" + sbEscolas.ToString() + ") AND r.tur_codigo IN(" + sbTurmas.ToString() + @")
                                            ",
                                                        param: parametros
                                                    ).ToList<ResultadoItem>()
                );

                resultado.Itens.AddRange( //NÍVEL ALUNO
                    conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT r.NivelProficienciaID, 
                                                r.esc_codigo AS Chave, "
                                                + titulo + @", 
                                                0 AS TotalAlunos, 
                                                COALESCE(r.Valor,-1) AS Valor, 
                                                0 AS PercentualAbaixoDoBasico, 
                                                0 AS PercentualBasico, 
                                                0 AS PercentualAdequado, 
                                                0 AS PercentualAvancado, 
                                                0 AS PercentualAlfabetizado, 
                                                r.REDQ1, r.REDQ2, r.REDQ3, r.REDQ4, r.REDQ5
                                            FROM ResultadoAluno r (NOLOCK)
                                            JOIN Escola (NOLOCK) e ON e.esc_codigo = r.esc_codigo 
                                            WHERE r.Edicao=@Edicao AND r.AreaConhecimentoID=@AreaConhecimentoID AND r.AnoEscolar=@AnoEscolar AND r.esc_codigo IN(" + sbEscolas.ToString() + ") AND r.tur_codigo IN(" + sbTurmas.ToString() + @")
                                            ORDER BY e.esc_nome, tur_codigo, r.alu_nome
                                            ",
                                        param: parametros
                                    ).ToList<ResultadoItem>()
                );

                var habilidades = conn.Query<HabilidadeTema, HabilidadeItem, HabilidadeTema>(
                        sql: @"
                                SELECT
                                    sme.HabilidadeCategoria AS Titulo,
                                    'split' AS Split,
                                    e.esc_nome + ' - ' + tur.tur_codigo AS OrigemTitulo,
                                    sme.HabilidadeCodigo AS Codigo,
                                    sme.HabilidadeDescricao AS Descricao,
                                    sme.PercentualAcertos AS PercentualAcertosNivelSME,
                                    dre.PercentualAcertos AS PercentualAcertosNivelDRE,
                                    esc.PercentualAcertos AS PercentualAcertosNivelEscola,
                                    tur.PercentualAcertos AS PercentualAcertosNivelTurma
                                FROM HabilidadeSme sme
                                JOIN HabilidadeDre dre    ON sme.edicao=dre.edicao AND sme.AreaConhecimentoID=dre.AreaConhecimentoID AND sme.AnoEscolar=dre.AnoEscolar AND sme.HabilidadeCodigo=dre.HabilidadeCodigo
                                JOIN HabilidadeEscola esc ON dre.edicao=esc.edicao AND dre.AreaConhecimentoID=esc.AreaConhecimentoID AND dre.AnoEscolar=esc.AnoEscolar AND dre.HabilidadeCodigo=esc.HabilidadeCodigo AND dre.uad_sigla=esc.uad_sigla
                                JOIN HabilidadeTurma  tur ON esc.edicao=tur.edicao AND esc.AreaConhecimentoID=tur.AreaConhecimentoID AND esc.AnoEscolar=tur.AnoEscolar AND esc.HabilidadeCodigo=tur.HabilidadeCodigo AND esc.esc_codigo=tur.esc_codigo
                                JOIN Escola e ON e.esc_codigo=esc.esc_codigo
                                WHERE sme.Edicao=@Edicao AND sme.AreaConhecimentoID=@AreaConhecimentoID AND sme.AnoEscolar=@AnoEscolar AND esc.esc_codigo IN(" + sbEscolas.ToString() + @") AND tur_codigo IN(" + sbTurmas.ToString() + @")
                            ",
                        map: (_tema, _item) =>
                        {
                            _tema.Itens.Add(_item);
                            return _tema;
                        },
                        splitOn: "Split",
                        param: parametros
                    ).ToList<HabilidadeTema>();

                resultado.Habilidades = OrganizarHabilidades(habilidades);

            }

            return resultado;
        }

        public static Resultado RecuperarResultadoEnturmacaoAtual(string Edicao, int AreaConhecimentoID, string AnoEscolarCorrente, string lista_turmas)
        {
            var resultado = new Resultado();

            if (String.IsNullOrEmpty(lista_turmas) || lista_turmas.Split(',').Length == 0)
            {
                return resultado;
            }

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                var parametros = new DynamicParameters();

                parametros.Add("AreaConhecimentoID", AreaConhecimentoID, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                var listaAlunos = DataAluno.RecuperarAlunos(Edicao, AreaConhecimentoID, "", "", lista_turmas);

                if (listaAlunos.Count == 0)
                {
                    throw new Exception("Nenhum aluno encontrado nessa turma");
                }

                var sbAlunos = new StringBuilder();

                //Construção dos parâmetros passados pela variável lista_alu_matricula de modo a evitar sql injection.
                foreach (var aluno in listaAlunos)
                {
                    if (sbAlunos.Length > 0)
                    {
                        sbAlunos.Append(",");
                    }
                    string parameterName = "@p_" + aluno.alu_matricula;
                    sbAlunos.Append(parameterName);
                    parametros.Add(parameterName, aluno.alu_matricula, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 50);
                }

                conn.Open();

                var ultimaEdicao = conn.ExecuteScalar<string>(
                        sql: @"
                            SELECT TOP 1 Edicao
                            FROM ResultadoAluno
                            ORDER BY Edicao DESC
                        "
                    );
                parametros.Add("Edicao", ultimaEdicao, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 10);

                resultado.Itens = conn.Query<ResultadoItem>(
                        sql: @"
                                SELECT r.AnoEscolar, 
                                    r.NivelProficienciaID, 
                                    r.alu_matricula AS Chave, 
                                    r.alu_nome + ' (' + r.alu_matricula + ')' AS Titulo, 
                                    0 AS TotalAlunos, 
                                    COALESCE(r.Valor,0) AS Valor, 
                                    0 AS PercentualAbaixoDoBasico, 
                                    0 AS PercentualBasico, 
                                    0 AS PercentualAdequado, 
                                    0 AS PercentualAvancado, 
                                    0 AS PercentualAlfabetizado, 
                                    r.REDQ1, r.REDQ2, r.REDQ3, r.REDQ4, r.REDQ5
                                FROM ResultadoAluno r
                                WHERE Edicao=@Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND r.alu_matricula IN(" + sbAlunos.ToString() + @")
                            ",
                            param: parametros
                    ).ToList<ResultadoItem>();


                int quantidadeEscolasEnvolvidas = listaAlunos.GroupBy(x => x.esc_codigo).Count();

                int menorAnoEscolar = 10;

                foreach (var resultadoItem in resultado.Itens)
                {
                    var aluno = listaAlunos.First(x => x.alu_matricula == resultadoItem.Chave);
                    resultadoItem.Chave = aluno.alu_matricula;
                    resultadoItem.ChavePai = "{ 'esc_codigo':'" + aluno.esc_codigo + "', 'tur_codigo':'" + aluno.tur_codigo + "'}";
                    string repetente = "";

                    if (resultadoItem.AnoEscolar == AnoEscolarCorrente)
                        repetente = " *";

                    if (menorAnoEscolar < int.Parse(resultadoItem.AnoEscolar))
                    {
                        menorAnoEscolar = int.Parse(resultadoItem.AnoEscolar);
                    }

                    if (quantidadeEscolasEnvolvidas > 1)
                    {
                        //Quando quantidadeEscolasEnvolvidas>1, inclui o nome da Escola seguido do código da Turma no título 
                        string esc_nome = aluno.esc_nome;
                        if (esc_nome.Length > 20)
                            esc_nome = aluno.esc_nome.Substring(0, 20);

                        resultadoItem.Titulo = "(" + esc_nome + "... " + aluno.tur_codigo + ") " + abreviarNome(resultadoItem.Titulo) + repetente;
                    }
                    else
                    {
                        //Inclui o código da Turma no título 
                        resultadoItem.Titulo = "(" + aluno.tur_codigo + ") " + abreviarNome(resultadoItem.Titulo) + repetente;
                    }
                }


                foreach (var aluno in listaAlunos)
                {
                    if (!resultado.Itens.Any(x => x.Titulo.IndexOf(aluno.alu_matricula) > 0))
                    {
                        if (quantidadeEscolasEnvolvidas > 1)
                        {
                            string esc_nome = aluno.esc_nome;
                            if (esc_nome.Length > 20)
                                esc_nome = aluno.esc_nome.Substring(0, 20);
                            resultado.Itens.Add(new ResultadoItem() { Valor = -1, Titulo = "(" + esc_nome + "... " + aluno.tur_codigo + ") " + abreviarNome(aluno.Nome) + "(" + aluno.alu_matricula + ")" });
                        }

                        else
                            resultado.Itens.Add(new ResultadoItem() { Valor = -1, Titulo = "(" + aluno.tur_codigo + ") " + abreviarNome(aluno.Nome) + "(" + aluno.alu_matricula + ")" });
                    }
                }

                resultado.Itens = resultado.Itens.OrderBy(x => x.Titulo).ToList();

                resultado.Agregacao = new List<ResultadoItem>();
            }

            return resultado;
        }

        private static string abreviarNome(string nome)
        {
            nome = nome.Replace("(", " (").Replace("  ", " ");
            string retorno = nome;
            if (nome.Length > 0) //>25
            {
                string[] nomeSplit = nome.Split(' ');
                var sb = new StringBuilder();
                int length = nomeSplit.Length;
                string[] ignorar = { "DA", "DAS", "DE", "DO", "DOS" };
                for (var i = 0; i < length; i++)
                {
                    string parte = nomeSplit[i];
                    if (i > 0)
                        sb.Append(" ");

                    if (i == 0 || i >= length - 2 || ignorar.Contains(parte))
                        sb.Append(parte);
                    else
                    {
                        sb.Append(parte.Substring(0, 1));
                        sb.Append(".");
                    }
                }
                retorno = sb.ToString();
            }

            return retorno;
        }

        public static Resultado RecuperarResultadoAluno(string Edicao, int AreaConhecimentoID, string AnoEscolar, string lista_alu_matricula, bool ExcluirSme_e_Dre)
        {
            var resultado = new Resultado();



            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                var parametros = new DynamicParameters();

                parametros.Add("Edicao", Edicao, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 10);
                parametros.Add("AreaConhecimentoID", AreaConhecimentoID, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                var sbAlunos = new StringBuilder();
                var lista_alu_matriculaSplit = lista_alu_matricula.Split(',');
                if (lista_alu_matriculaSplit.Length == 0)
                {
                    return resultado;
                }

                //Construção dos parâmetros passados pela variável lista_alu_matricula de modo a evitar sql injection.
                foreach (var alu_matricula in lista_alu_matriculaSplit)
                {
                    if (sbAlunos.Length > 0)
                    {
                        sbAlunos.Append(",");
                    }
                    string parameterName = "@p_" + alu_matricula;
                    sbAlunos.Append(parameterName);
                    parametros.Add(parameterName, alu_matricula, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 50);
                }

                conn.Open();

                resultado.Agregacao = new List<ResultadoItem>();

                resultado.Itens = new List<ResultadoItem>();

                var resultadoItens_ALUNO = conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT r.NivelProficienciaID, 
                                                r.esc_codigo AS Chave, 
                                                r.AnoEscolar, 
                                                r.alu_nome + ' (' + r.alu_matricula + ')' AS Titulo, 
                                                0 AS TotalAlunos, 
                                                COALESCE(r.Valor,-1) AS Valor, 
                                                0 AS PercentualAbaixoDoBasico, 
                                                0 AS PercentualBasico, 
                                                0 AS PercentualAdequado, 
                                                0 AS PercentualAvancado, 
                                                0 AS PercentualAlfabetizado, 
                                                r.REDQ1, r.REDQ2, r.REDQ3, r.REDQ4, r.REDQ5
                                            FROM ResultadoAluno r (NOLOCK)
                                            WHERE r.Edicao=@Edicao AND r.AreaConhecimentoID=@AreaConhecimentoID AND r.alu_matricula IN(" + sbAlunos.ToString() + @")
                                            ORDER BY r.alu_nome
                                            ",
                                        param: parametros
                                    ).ToList<ResultadoItem>();

                if (resultadoItens_ALUNO.Count() > 0)
                    parametros.Add("AnoEscolar", resultadoItens_ALUNO.First().AnoEscolar, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 3);
                else
                    return resultado;



                if (!ExcluirSme_e_Dre)
                {
                    resultado.Itens.AddRange( //NÍVEL SME
                    conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT 'MÉDIA DA SME' AS Titulo, 
                                                'SME' AS Chave, 
                                                AnoEscolar, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoSme WITH (NOLOCK)
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND AnoEscolar=@AnoEscolar",
                                        param: parametros
                                    ).ToList<ResultadoItem>()
                    );

                    var resultadoItens_DRE = conn.Query<ResultadoItem>(
                                            sql: @"
                                            SELECT NivelProficienciaID, 
                                                uad_sigla AS Chave, 
                                                AnoEscolar, 
                                                '' AS Titulo, 
                                                TotalAlunos, 
                                                Valor, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoDre WITH (NOLOCK)
                                            WHERE Edicao=@Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND AnoEscolar=@AnoEscolar AND uad_sigla IN(SELECT uad_sigla FROM ResultadoAluno WHERE Edicao=@Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND alu_matricula IN(" + sbAlunos.ToString() + @")) 
                                            ",
                                            param: parametros
                                        ).ToList<ResultadoItem>();
                    foreach (var dre in resultadoItens_DRE)
                    {
                        dre.Titulo = "MÉDIA DA DRE " + DataDRE.RecuperarNome(dre.Chave);
                    }
                    resultado.Itens.AddRange( //NÍVEL DRE
                            resultadoItens_DRE
                    );
                }


                resultado.Itens.AddRange( //NÍVEL ESCOLA
                    conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT 'MÉDIA DA ESCOLA ' + e.esc_nome AS Titulo, 
                                                r.esc_codigo AS Chave, 
                                                r.AnoEscolar, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoEscola r (NOLOCK)
                                            JOIN Escola (NOLOCK) e ON e.esc_codigo = r.esc_codigo 
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND AnoEscolar=@AnoEscolar AND r.esc_codigo IN(SELECT esc_codigo FROM ResultadoAluno WHERE Edicao=@Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND alu_matricula IN(" + sbAlunos.ToString() + @")) 
                                            ",
                                        param: parametros
                                    ).ToList<ResultadoItem>()
                    );

                resultado.Itens.AddRange( //NÍVEL TURMA
                                    conn.Query<ResultadoItem>(
                                                        sql: @"
                                            SELECT 'MÉDIA DA TURMA ' + r.tur_codigo AS Titulo, 
                                                r.tur_codigo AS Chave, 
                                                r.AnoEscolar, 
                                                NivelProficienciaID, 
                                                COALESCE(Valor,0) AS Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoTurma r WITH (NOLOCK)
                                            JOIN Escola (NOLOCK) e ON e.esc_codigo = r.esc_codigo 
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND AnoEscolar=@AnoEscolar AND r.tur_id IN(SELECT tur_id FROM ResultadoAluno WHERE Edicao=@Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND alu_matricula IN(" + sbAlunos.ToString() + @") ) 
                                            ",
                                                        param: parametros
                                                    ).ToList<ResultadoItem>()
                                    );

                resultado.Itens.AddRange( //NÍVEL ALUNO
                    resultadoItens_ALUNO
                    );

            }

            if (resultado.Itens.Count > 0)
            {
                resultado.AnoEscolar = resultado.Itens.First().AnoEscolar;
            }



            return resultado;
        }

        public static int RetornarNivelProficienciaID(int AreaConhecimentoID, string AnoEscolar, float ValorProficiencia)
        {
            int Retorno = 0;

            int LIMITE_ABAIXO_DO_BASICO = 0;
            int LIMITE_BASICO = 0;
            int LIMITE_ADEQUADO = 0;

            if (AreaConhecimentoID == 1) //Ciências da Natureza
            {
                if (AnoEscolar == "3")
                {
                    LIMITE_ABAIXO_DO_BASICO = 125;
                    LIMITE_BASICO = 175;
                    LIMITE_ADEQUADO = 225;
                }
                else if (AnoEscolar == "4")
                {
                    LIMITE_ABAIXO_DO_BASICO = 150;
                    LIMITE_BASICO = 200;
                    LIMITE_ADEQUADO = 250;
                }
                else if (AnoEscolar == "5")
                {
                    LIMITE_ABAIXO_DO_BASICO = 175;
                    LIMITE_BASICO = 225;
                    LIMITE_ADEQUADO = 275;
                }
                else if (AnoEscolar == "6")
                {
                    LIMITE_ABAIXO_DO_BASICO = 190;
                    LIMITE_BASICO = 240;
                    LIMITE_ADEQUADO = 290;
                }
                else if (AnoEscolar == "7")
                {
                    LIMITE_ABAIXO_DO_BASICO = 200;
                    LIMITE_BASICO = 250;
                    LIMITE_ADEQUADO = 300;
                }
                else if (AnoEscolar == "8")
                {
                    LIMITE_ABAIXO_DO_BASICO = 210;
                    LIMITE_BASICO = 275;
                    LIMITE_ADEQUADO = 325;
                }
                else if (AnoEscolar == "9")
                {
                    LIMITE_ABAIXO_DO_BASICO = 225;
                    LIMITE_BASICO = 300;
                    LIMITE_ADEQUADO = 350;
                }
            }
            else if (AreaConhecimentoID == 2) //Língua Portuguesa
            {
                if (AnoEscolar == "3")
                {
                    LIMITE_ABAIXO_DO_BASICO = 115;
                    LIMITE_BASICO = 150;
                    LIMITE_ADEQUADO = 200;
                }
                else if (AnoEscolar == "4")
                {
                    LIMITE_ABAIXO_DO_BASICO = 135;
                    LIMITE_BASICO = 175;
                    LIMITE_ADEQUADO = 225;
                }
                else if (AnoEscolar == "5")
                {
                    LIMITE_ABAIXO_DO_BASICO = 150;
                    LIMITE_BASICO = 200;
                    LIMITE_ADEQUADO = 250;
                }
                else if (AnoEscolar == "6")
                {
                    LIMITE_ABAIXO_DO_BASICO = 165;
                    LIMITE_BASICO = 215;
                    LIMITE_ADEQUADO = 265;
                }
                else if (AnoEscolar == "7")
                {
                    LIMITE_ABAIXO_DO_BASICO = 175;
                    LIMITE_BASICO = 225;
                    LIMITE_ADEQUADO = 275;
                }
                else if (AnoEscolar == "8")
                {
                    LIMITE_ABAIXO_DO_BASICO = 185;
                    LIMITE_BASICO = 250;
                    LIMITE_ADEQUADO = 300;
                }
                else if (AnoEscolar == "9")
                {
                    LIMITE_ABAIXO_DO_BASICO = 200;
                    LIMITE_BASICO = 275;
                    LIMITE_ADEQUADO = 325;
                }
            }
            else if (AreaConhecimentoID == 3) // Matemática
            {
                if (AnoEscolar == "3")
                {
                    LIMITE_ABAIXO_DO_BASICO = 125;
                    LIMITE_BASICO = 175;
                    LIMITE_ADEQUADO = 225;
                }
                else if (AnoEscolar == "4")
                {
                    LIMITE_ABAIXO_DO_BASICO = 150;
                    LIMITE_BASICO = 200;
                    LIMITE_ADEQUADO = 250;
                }
                else if (AnoEscolar == "5")
                {
                    LIMITE_ABAIXO_DO_BASICO = 175;
                    LIMITE_BASICO = 225;
                    LIMITE_ADEQUADO = 275;
                }
                else if (AnoEscolar == "6")
                {
                    LIMITE_ABAIXO_DO_BASICO = 190;
                    LIMITE_BASICO = 240;
                    LIMITE_ADEQUADO = 290;
                }
                else if (AnoEscolar == "7")
                {
                    LIMITE_ABAIXO_DO_BASICO = 200;
                    LIMITE_BASICO = 250;
                    LIMITE_ADEQUADO = 300;
                }
                else if (AnoEscolar == "8")
                {
                    LIMITE_ABAIXO_DO_BASICO = 210;
                    LIMITE_BASICO = 275;
                    LIMITE_ADEQUADO = 325;
                }
                else if (AnoEscolar == "9")
                {
                    LIMITE_ABAIXO_DO_BASICO = 225;
                    LIMITE_BASICO = 300;
                    LIMITE_ADEQUADO = 350;
                }
            }
            else if (AreaConhecimentoID == 4) // Redação
            {
                LIMITE_ABAIXO_DO_BASICO = 50;
                LIMITE_BASICO = 65;
                LIMITE_ADEQUADO = 90;
            }

            if (ValorProficiencia < LIMITE_ABAIXO_DO_BASICO)
                Retorno = 1;
            else if (ValorProficiencia < LIMITE_BASICO)
                Retorno = 2;
            else if (ValorProficiencia < LIMITE_ADEQUADO)
                Retorno = 3;
            else
                Retorno = 4;

            return Retorno;
        }

        public static Resultado RecuperarResultadoCicloSME(string Edicao, int AreaConhecimentoID, string Ciclo)
        {
            var resultado = new Resultado();

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                conn.Open();

                resultado.Agregacao = conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT 'SME' AS Titulo, 
                                                'SME' AS Chave, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoCicloSme WITH (NOLOCK)
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND CicloId=@Ciclo",
                                        param: new
                                        {
                                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                                            AreaConhecimentoID = AreaConhecimentoID,
                                            Ciclo = new DbString() { Value = Ciclo, IsAnsi = true, Length = 3 }
                                        }).ToList<ResultadoItem>();

                resultado.Itens = new List<ResultadoItem>();

                resultado.Itens.AddRange( //NÍVEL SME
                    conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT 'MÉDIA DA SME' AS Titulo, 
                                                'SME' AS Chave, 
                                                CicloId, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoCicloSme WITH (NOLOCK)
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND CicloId=@Ciclo",
                                        param: new
                                        {
                                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                                            AreaConhecimentoID = AreaConhecimentoID,
                                            Ciclo = new DbString() { Value = Ciclo, IsAnsi = true, Length = 3 }
                                        }
                                    ).ToList<ResultadoItem>()
                );

                var resultadoItens_DRE = conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT NivelProficienciaID, 
                                                uad_sigla AS Chave, 
                                                '' AS Titulo, 
                                                TotalAlunos, 
                                                Valor, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoCicloDre WITH (NOLOCK)
                                            WHERE Edicao=@Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND CicloId=@Ciclo",
                                        param: new
                                        {
                                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                                            AreaConhecimentoID = AreaConhecimentoID,
                                            Ciclo = new DbString() { Value = Ciclo, IsAnsi = true, Length = 3 }

                                        }).ToList<ResultadoItem>();

                foreach (var dre in resultadoItens_DRE)
                {
                    dre.Titulo = DataDRE.RecuperarNome(dre.Chave);
                }

                resultado.Itens.AddRange(resultadoItens_DRE);


                var habilidades = conn.Query<HabilidadeTema, HabilidadeItem, HabilidadeTema>(
                        sql: @"
                                SELECT
                                    sme.HabilidadeCategoria AS Titulo,
                                    'split' AS Split,
                                    sme.HabilidadeCodigo AS Codigo,
                                    sme.HabilidadeDescricao AS Descricao,
                                    sme.PercentualAcertos AS PercentualAcertosNivelSME,
                                    -1 AS PercentualAcertosNivelDRE,
                                    -1 AS PercentualAcertosNivelEscola,
                                    -1 AS PercentualAcertosNivelTurma
                                FROM HabilidadeCicloSme sme
                                WHERE sme.Edicao=@Edicao AND sme.AreaConhecimentoID=@AreaConhecimentoID AND sme.CicloId=@Ciclo
                            ",
                        map: (_tema, _item) =>
                        {
                            _tema.Itens.Add(_item);
                            return _tema;
                        },
                        splitOn: "Split",
                        param: new
                        {
                            Edicao = new DbString() { Value = Edicao, IsAnsi = true, Length = 10 },
                            AreaConhecimentoID = AreaConhecimentoID,
                            Ciclo = new DbString() { Value = Ciclo, IsAnsi = true, Length = 3 }
                        }
                    ).ToList<HabilidadeTema>();


                resultado.Habilidades = OrganizarHabilidades(habilidades);
            }

            return resultado;
        }

        public static Resultado RecuperarResultadoCicloDRE(string Edicao, int AreaConhecimentoID, string CicloId, string lista_uad_sigla)
        {
            var resultado = new Resultado();

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                var parametros = new DynamicParameters();

                parametros.Add("Edicao", Edicao, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 10);
                parametros.Add("Ciclo", CicloId, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 3);
                parametros.Add("AreaConhecimentoID", AreaConhecimentoID, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                var sbDREs = new StringBuilder();
                var lista_uad_siglaSplit = lista_uad_sigla.Split(',');
                if (lista_uad_siglaSplit.Length == 0)
                {
                    return resultado;
                }

                //Construção dos parâmetros passados pela variável lista_uad_sigla de modo a evitar sql injection.
                foreach (var uad_sigla in lista_uad_siglaSplit)
                {
                    if (sbDREs.Length > 0)
                    {
                        sbDREs.Append(",");
                    }
                    string parameterName = "@p_" + uad_sigla;
                    sbDREs.Append(parameterName);
                    parametros.Add(parameterName, uad_sigla, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 4);
                }

                conn.Open();

                resultado.Agregacao = conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT '' AS Titulo, 
                                                uad_sigla AS Chave, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoCicloDre WITH (NOLOCK)
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND CicloId=@Ciclo AND uad_sigla IN(" + sbDREs.ToString() + @") ",
                                        param: parametros
                                    ).ToList<ResultadoItem>();


                resultado.Itens = new List<ResultadoItem>();

                resultado.Itens.AddRange( //NÍVEL SME
                    conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT 'MÉDIA DA SME' AS Titulo, 
                                                'SME' AS Chave, 
                                                CicloId, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoCicloSme WITH (NOLOCK)
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND CicloId=@Ciclo",
                                        param: parametros
                                    ).ToList<ResultadoItem>()
                );

                var resultadoItens_DRE = conn.Query<ResultadoItem>(
                                            sql: @"
                                            SELECT NivelProficienciaID, 
                                                uad_sigla AS Chave, 
                                                CicloId, 
                                                '' AS Titulo, 
                                                TotalAlunos, 
                                                Valor, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoCicloDre WITH (NOLOCK)
                                            WHERE Edicao=@Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND CicloId=@Ciclo AND uad_sigla IN(" + sbDREs.ToString() + @")
                                            ",
                                            param: parametros
                                        ).ToList<ResultadoItem>();
                foreach (var dre in resultadoItens_DRE)
                {
                    dre.Titulo = "MÉDIA DA DRE " + DataDRE.RecuperarNome(dre.Chave);
                }
                resultado.Itens.AddRange( //NÍVEL DRE
                        resultadoItens_DRE
                );


                resultado.Itens.AddRange(
                    conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT r.NivelProficienciaID, 
                                                r.esc_codigo AS Chave, 
                                                '(' + r.uad_sigla + ') ' +e.esc_nome AS Titulo, 
                                                r.TotalAlunos, 
                                                r.Valor, 
                                                r.PercentualAbaixoDoBasico, 
                                                r.PercentualBasico, 
                                                r.PercentualAdequado, 
                                                r.PercentualAvancado, 
                                                r.PercentualAlfabetizado
                                            FROM ResultadoCicloEscola r (NOLOCK)
                                            JOIN Escola (NOLOCK) e ON e.esc_codigo = r.esc_codigo 
                                            WHERE r.Edicao=@Edicao AND r.AreaConhecimentoID=@AreaConhecimentoID AND r.CicloId=@Ciclo AND r.uad_sigla IN(" + sbDREs.ToString() + @") 
                                            ORDER BY r.uad_sigla, e.esc_nome
                                            ",
                                        param: parametros
                                    ).ToList<ResultadoItem>()
                );

                var habilidades = conn.Query<HabilidadeTema, HabilidadeItem, HabilidadeTema>(
                        sql: @"
                                SELECT
                                    sme.HabilidadeCategoria AS Titulo,
                                    'split' AS Split,
                                    dre.uad_sigla AS OrigemTitulo,
                                    sme.HabilidadeCodigo AS Codigo,
                                    sme.HabilidadeDescricao AS Descricao,
                                    sme.PercentualAcertos AS PercentualAcertosNivelSME,
                                    dre.PercentualAcertos AS PercentualAcertosNivelDRE,
                                    -1 AS PercentualAcertosNivelEscola,
                                    -1 AS PercentualAcertosNivelTurma
                                FROM HabilidadeCicloSme sme
                                JOIN HabilidadeCicloDre dre    ON sme.edicao=dre.edicao AND sme.AreaConhecimentoID=dre.AreaConhecimentoID AND sme.CicloId=dre.CicloId AND sme.HabilidadeCodigo=dre.HabilidadeCodigo
                                WHERE sme.Edicao=@Edicao AND sme.AreaConhecimentoID=@AreaConhecimentoID AND sme.CicloId=@Ciclo AND dre.uad_sigla IN(" + sbDREs.ToString() + @") 
                            ",
                        map: (_tema, _item) =>
                        {
                            _tema.Itens.Add(_item);
                            return _tema;
                        },
                        splitOn: "Split",
                        param: parametros
                    ).ToList<HabilidadeTema>();

                resultado.Habilidades = OrganizarHabilidades(habilidades);

            }

            foreach (var dre in resultado.Agregacao)
            {
                dre.Titulo = DataDRE.RecuperarNome(dre.Chave);
            }

            if (lista_uad_sigla.Split(',').Length > 1)
            {
                foreach (var habilidadeTema in resultado.Habilidades)
                {
                    foreach (var habilidade in habilidadeTema.Itens)
                    {
                        habilidade.OrigemTitulo = DataDRE.RecuperarNome(habilidade.OrigemTitulo);
                    }
                }
            }

            return resultado;
        }

        public static Resultado RecuperarResultadoCicloEscola(string Edicao, int AreaConhecimentoID, string CicloId, string lista_esc_codigo)
        {
            var resultado = new Resultado();

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                var parametros = new DynamicParameters();

                parametros.Add("Edicao", Edicao, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 10);
                parametros.Add("Ciclo", CicloId, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 3);
                parametros.Add("AreaConhecimentoID", AreaConhecimentoID, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                var sbEscolas = new StringBuilder();
                var lista_esc_codigoSplit = lista_esc_codigo.Split(',');
                if (lista_esc_codigoSplit.Length == 0)
                {
                    return resultado;
                }

                //Construção dos parâmetros passados pela variável lista_esc_codigo de modo a evitar sql injection.
                foreach (var esc_codigo in lista_esc_codigoSplit)
                {
                    if (sbEscolas.Length > 0)
                    {
                        sbEscolas.Append(",");
                    }
                    string parameterName = "@p_" + esc_codigo;
                    sbEscolas.Append(parameterName);
                    parametros.Add(parameterName, esc_codigo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 20);
                }

                conn.Open();

                resultado.Agregacao = conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT e.esc_nome AS Titulo, 
                                                r.esc_codigo AS Chave, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoCicloEscola r (NOLOCK)
                                            JOIN Escola (NOLOCK) e ON e.esc_codigo = r.esc_codigo 
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND CicloId=@Ciclo AND r.esc_codigo IN(" + sbEscolas.ToString() + @") ",
                                        param: parametros
                                    ).ToList<ResultadoItem>();

                //resultado.NivelProficienciaID = RetornarNivelProficienciaID(AreaConhecimentoID, CicloId, resultado.Valor);


                string titulo = "r.tur_codigo AS Titulo";
                if (lista_esc_codigoSplit.Length > 1)
                {
                    titulo = "'(' + r.uad_sigla + ') ' +e.esc_nome + ' - ' + r.tur_codigo AS Titulo";
                }

                resultado.Itens = new List<ResultadoItem>();

                resultado.Itens.AddRange( //NÍVEL SME
                    conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT 'MÉDIA DA SME' AS Titulo, 
                                                'SME' AS Chave, 
                                                CicloId, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoCicloSme WITH (NOLOCK)
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND CicloId=@Ciclo",
                                        param: parametros
                                    ).ToList<ResultadoItem>()
                );

                var resultadoItens_DRE = conn.Query<ResultadoItem>(
                                            sql: @"
                                            SELECT NivelProficienciaID, 
                                                uad_sigla AS Chave, 
                                                CicloId, 
                                                '' AS Titulo, 
                                                TotalAlunos, 
                                                Valor, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoCicloDre WITH (NOLOCK)
                                            WHERE Edicao=@Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND CicloId=@Ciclo AND uad_sigla IN(SELECT DISTINCT uad_codigo FROM Escola WHERE esc_codigo IN(" + sbEscolas.ToString() + @"))
                                            ",
                                            param: parametros
                                        ).ToList<ResultadoItem>();
                foreach (var dre in resultadoItens_DRE)
                {
                    dre.Titulo = "MÉDIA DA DRE " + DataDRE.RecuperarNome(dre.Chave);
                }
                resultado.Itens.AddRange( //NÍVEL DRE
                        resultadoItens_DRE
                );

                resultado.Itens.AddRange(conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT r.NivelProficienciaID, 
                                                r.esc_codigo AS Chave, "
                                                + titulo + @", 
                                                r.TotalAlunos, 
                                                r.Valor, 
                                                r.PercentualAbaixoDoBasico, 
                                                r.PercentualBasico, 
                                                r.PercentualAdequado, 
                                                r.PercentualAvancado, 
                                                r.PercentualAlfabetizado
                                            FROM ResultadoCicloTurma r (NOLOCK)
                                            JOIN Escola (NOLOCK) e ON e.esc_codigo = r.esc_codigo 
                                            WHERE r.Edicao=@Edicao AND r.AreaConhecimentoID=@AreaConhecimentoID AND r.CicloId=@Ciclo AND r.esc_codigo IN(" + sbEscolas.ToString() + @") 
                                            ORDER BY r.uad_sigla, e.esc_nome
                                            ",
                                        param: parametros
                                    ).ToList<ResultadoItem>()
                );



                var habilidades = conn.Query<HabilidadeTema, HabilidadeItem, HabilidadeTema>(
                        sql: @"
                                SELECT
                                    sme.HabilidadeCategoria AS Titulo,
                                    'split' AS Split,
                                    e.esc_nome AS OrigemTitulo,
                                    sme.HabilidadeCodigo AS Codigo,
                                    sme.HabilidadeDescricao AS Descricao,
                                    sme.PercentualAcertos AS PercentualAcertosNivelSME,
                                    dre.PercentualAcertos AS PercentualAcertosNivelDRE,
                                    esc.PercentualAcertos AS PercentualAcertosNivelEscola,
                                    -1 AS PercentualAcertosNivelTurma
                                FROM HabilidadeCicloSme sme
                                JOIN HabilidadeCicloDre dre    ON sme.edicao=dre.edicao AND sme.AreaConhecimentoID=dre.AreaConhecimentoID AND sme.CicloId=dre.CicloId AND sme.HabilidadeCodigo=dre.HabilidadeCodigo
                                JOIN HabilidadeCicloEscola esc ON dre.edicao=esc.edicao AND dre.AreaConhecimentoID=esc.AreaConhecimentoID AND dre.CicloId=esc.CicloId AND dre.HabilidadeCodigo=esc.HabilidadeCodigo AND dre.uad_sigla=esc.uad_sigla
                                JOIN Escola e ON e.esc_codigo=esc.esc_codigo
                                WHERE sme.Edicao=@Edicao AND sme.AreaConhecimentoID=@AreaConhecimentoID AND sme.CicloId=@Ciclo AND esc.esc_codigo IN(" + sbEscolas.ToString() + @") 
                            ",
                        map: (_tema, _item) =>
                        {
                            _tema.Itens.Add(_item);
                            return _tema;
                        },
                        splitOn: "Split",
                        param: parametros
                    ).ToList<HabilidadeTema>();

                resultado.Habilidades = OrganizarHabilidades(habilidades);

            }

            return resultado;
        }

        public static Resultado RecuperarResultadoCicloTurma(string Edicao, int AreaConhecimentoID, string CicloId, string lista_esc_codigo, string lista_turmas)
        {
            var resultado = new Resultado();

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                var parametros = new DynamicParameters();

                parametros.Add("Edicao", Edicao, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 10);
                parametros.Add("Ciclo", CicloId, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 3);
                parametros.Add("AreaConhecimentoID", AreaConhecimentoID, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                var sbEscolas = new StringBuilder();
                var lista_esc_codigoSplit = lista_esc_codigo.Split(',');
                if (lista_esc_codigoSplit.Length == 0)
                {
                    return resultado;
                }

                //Construção dos parâmetros passados pela variável lista_esc_codigo de modo a evitar sql injection.
                foreach (var esc_codigo in lista_esc_codigoSplit)
                {
                    if (sbEscolas.Length > 0)
                    {
                        sbEscolas.Append(",");
                    }
                    string parameterName = "@p_" + esc_codigo;
                    sbEscolas.Append(parameterName);
                    parametros.Add(parameterName, esc_codigo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 20);
                }

                var sbTurmas = new StringBuilder();
                var lista_turmasSplit = lista_turmas.Split(',');
                if (lista_turmasSplit.Length == 0)
                {
                    return resultado;
                }

                //Construção dos parâmetros passados pela variável lista_turmas de modo a evitar sql injection.
                foreach (var tur_codigo in lista_turmasSplit)
                {
                    if (sbTurmas.Length > 0)
                    {
                        sbTurmas.Append(",");
                    }
                    string parameterName = "@p_" + tur_codigo;
                    sbTurmas.Append(parameterName);
                    parametros.Add(parameterName, tur_codigo, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 20);
                }

                conn.Open();

                resultado.Agregacao = conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT e.esc_nome + ' - ' + r.tur_codigo AS Titulo, 
                                                r.tur_codigo AS Chave, 
                                                NivelProficienciaID, 
                                                COALESCE(Valor,0) AS Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoCicloTurma r WITH (NOLOCK)
                                            JOIN Escola (NOLOCK) e ON e.esc_codigo = r.esc_codigo 
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND CicloId=@Ciclo AND r.esc_codigo IN(" + sbEscolas.ToString() + ") AND tur_codigo IN(" + sbTurmas.ToString() + @") ",
                                        param: parametros
                                    ).ToList<ResultadoItem>();

                //resultado.NivelProficienciaID = RetornarNivelProficienciaID(AreaConhecimentoID, CicloId, resultado.Valor);

                string titulo = "'(' + tur_codigo + ') ' + r.alu_nome + ' (' + r.alu_matricula + ')' AS Titulo";
                if (lista_esc_codigoSplit.Length > 1)
                {
                    titulo = "'(' + LEFT(e.esc_nome, 20) + '... ' + tur_codigo + ') ' + r.alu_nome + ' (' + r.alu_matricula + ')' AS Titulo";
                }

                resultado.Itens = new List<ResultadoItem>();

                resultado.Itens.AddRange( //NÍVEL SME
                    conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT 'MÉDIA DA SME' AS Titulo, 
                                                'SME' AS Chave, 
                                                CicloId, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoCicloSme WITH (NOLOCK)
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND CicloId=@Ciclo",
                                        param: parametros
                                    ).ToList<ResultadoItem>()
                );

                var resultadoItens_DRE = conn.Query<ResultadoItem>(
                                            sql: @"
                                            SELECT NivelProficienciaID, 
                                                uad_sigla AS Chave, 
                                                CicloId, 
                                                '' AS Titulo, 
                                                TotalAlunos, 
                                                Valor, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoCicloDre WITH (NOLOCK)
                                            WHERE Edicao=@Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND CicloId=@Ciclo AND uad_sigla IN(SELECT DISTINCT uad_codigo FROM Escola WHERE esc_codigo IN(" + sbEscolas.ToString() + @"))
                                            ",
                                            param: parametros
                                        ).ToList<ResultadoItem>();
                foreach (var dre in resultadoItens_DRE)
                {
                    dre.Titulo = "MÉDIA DA DRE " + DataDRE.RecuperarNome(dre.Chave);
                }
                resultado.Itens.AddRange( //NÍVEL DRE
                        resultadoItens_DRE
                );

                resultado.Itens.AddRange( //NÍVEL ESCOLA
                    conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT 'MÉDIA DA ESCOLA ' + e.esc_nome AS Titulo, 
                                                r.esc_codigo AS Chave, 
                                                r.CicloId, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoCicloEscola r (NOLOCK)
                                            JOIN Escola (NOLOCK) e ON e.esc_codigo = r.esc_codigo 
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND CicloId=@Ciclo AND r.esc_codigo IN(" + sbEscolas.ToString() + @")
                                            ",
                                        param: parametros
                                    ).ToList<ResultadoItem>()
                    );

                resultado.Itens.AddRange( //NÍVEL TURMA
                                    conn.Query<ResultadoItem>(
                                                        sql: @"
                                            SELECT 'MÉDIA DA TURMA ' + e.esc_nome + ' (' + r.tur_codigo + ')' AS Titulo, 
                                                r.tur_codigo AS Chave, 
                                                r.CicloId, 
                                                NivelProficienciaID, 
                                                COALESCE(Valor,0) AS Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoCicloTurma r WITH (NOLOCK)
                                            JOIN Escola (NOLOCK) e ON e.esc_codigo = r.esc_codigo 
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND CicloId=@Ciclo AND r.esc_codigo IN(" + sbEscolas.ToString() + ") AND r.tur_codigo IN(" + sbTurmas.ToString() + @")
                                            ",
                                                        param: parametros
                                                    ).ToList<ResultadoItem>()
                );

                resultado.Itens.AddRange( //NÍVEL ALUNO
                    conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT r.NivelProficienciaID, 
                                                r.esc_codigo AS Chave, "
                                                + titulo + @", 
                                                0 AS TotalAlunos, 
                                                COALESCE(r.Valor,-1) AS Valor, 
                                                0 AS PercentualAbaixoDoBasico, 
                                                0 AS PercentualBasico, 
                                                0 AS PercentualAdequado, 
                                                0 AS PercentualAvancado, 
                                                0 AS PercentualAlfabetizado
                                            FROM ResultadoCicloAluno r (NOLOCK)
                                            JOIN Escola (NOLOCK) e ON e.esc_codigo = r.esc_codigo 
                                            WHERE r.Edicao=@Edicao AND r.AreaConhecimentoID=@AreaConhecimentoID AND r.CicloId=@Ciclo AND r.esc_codigo IN(" + sbEscolas.ToString() + ") AND r.tur_codigo IN(" + sbTurmas.ToString() + @")
                                            ORDER BY e.esc_nome, tur_codigo, r.alu_nome
                                            ",
                                        param: parametros
                                    ).ToList<ResultadoItem>()
                );

                var habilidades = conn.Query<HabilidadeTema, HabilidadeItem, HabilidadeTema>(
                        sql: @"
                                SELECT
                                    sme.HabilidadeCategoria AS Titulo,
                                    'split' AS Split,
                                    e.esc_nome + ' - ' + tur.tur_codigo AS OrigemTitulo,
                                    sme.HabilidadeCodigo AS Codigo,
                                    sme.HabilidadeDescricao AS Descricao,
                                    sme.PercentualAcertos AS PercentualAcertosNivelSME,
                                    dre.PercentualAcertos AS PercentualAcertosNivelDRE,
                                    esc.PercentualAcertos AS PercentualAcertosNivelEscola,
                                    tur.PercentualAcertos AS PercentualAcertosNivelTurma
                                FROM HabilidadeCicloSme sme
                                JOIN HabilidadeCicloDre dre    ON sme.edicao=dre.edicao AND sme.AreaConhecimentoID=dre.AreaConhecimentoID AND sme.CicloId=dre.CicloId AND sme.HabilidadeCodigo=dre.HabilidadeCodigo
                                JOIN HabilidadeCicloEscola esc ON dre.edicao=esc.edicao AND dre.AreaConhecimentoID=esc.AreaConhecimentoID AND dre.CicloId=esc.CicloId AND dre.HabilidadeCodigo=esc.HabilidadeCodigo AND dre.uad_sigla=esc.uad_sigla
                                JOIN HabilidadeCicloTurma  tur ON esc.edicao=tur.edicao AND esc.AreaConhecimentoID=tur.AreaConhecimentoID AND esc.CicloId=tur.CicloId AND esc.HabilidadeCodigo=tur.HabilidadeCodigo AND esc.esc_codigo=tur.esc_codigo
                                JOIN Escola e ON e.esc_codigo=esc.esc_codigo
                                WHERE sme.Edicao=@Edicao AND sme.AreaConhecimentoID=@AreaConhecimentoID AND sme.CicloId=@Ciclo AND esc.esc_codigo IN(" + sbEscolas.ToString() + @") AND tur_codigo IN(" + sbTurmas.ToString() + @")
                            ",
                        map: (_tema, _item) =>
                        {
                            _tema.Itens.Add(_item);
                            return _tema;
                        },
                        splitOn: "Split",
                        param: parametros
                    ).ToList<HabilidadeTema>();

                resultado.Habilidades = OrganizarHabilidades(habilidades);

            }

            return resultado;
        }

        public static Resultado RecuperarResultadoCicloEnturmacaoAtual(string Edicao, int AreaConhecimentoID, string CicloCorrente, string lista_turmas)
        {
            var resultado = new Resultado();

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                var parametros = new DynamicParameters();

                parametros.Add("AreaConhecimentoID", AreaConhecimentoID, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                if (lista_turmas.Split(',').Length == 0)
                {
                    return resultado;
                }

                var listaAlunos = DataAluno.RecuperarAlunos(Edicao, AreaConhecimentoID, "", "", lista_turmas);

                if (listaAlunos.Count == 0)
                {
                    throw new Exception("Nenhum aluno encontrado nessa turma");
                }

                var sbAlunos = new StringBuilder();

                //Construção dos parâmetros passados pela variável lista_alu_matricula de modo a evitar sql injection.
                foreach (var aluno in listaAlunos)
                {
                    if (sbAlunos.Length > 0)
                    {
                        sbAlunos.Append(",");
                    }
                    string parameterName = "@p_" + aluno.alu_matricula;
                    sbAlunos.Append(parameterName);
                    parametros.Add(parameterName, aluno.alu_matricula, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 50);
                }

                conn.Open();

                var ultimaEdicao = conn.ExecuteScalar<string>(
                        sql: @"
                            SELECT TOP 1 Edicao
                            FROM ResultadoCicloAluno
                            ORDER BY Edicao DESC
                        "
                    );

                parametros.Add("Edicao", ultimaEdicao, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 10);


                resultado.Itens = conn.Query<ResultadoItem>(
                        sql: @"
                                SELECT r.CicloId, 
                                    r.NivelProficienciaID, 
                                    r.alu_matricula AS Chave, 
                                    r.alu_nome + ' (' + r.alu_matricula + ')' AS Titulo, 
                                    0 AS TotalAlunos, 
                                    COALESCE(r.Valor,0) AS Valor,
                                    0 AS PercentualAbaixoDoBasico, 
                                    0 AS PercentualBasico, 
                                    0 AS PercentualAdequado, 
                                    0 AS PercentualAvancado, 
                                    0 AS PercentualAlfabetizado
                                FROM ResultadoCicloAluno r
                                WHERE Edicao=@Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND r.alu_matricula IN(" + sbAlunos.ToString() + @")
                            ",
                            param: parametros
                    ).ToList<ResultadoItem>();


                int quantidadeEscolasEnvolvidas = listaAlunos.GroupBy(x => x.esc_codigo).Count();

                int menorCiclo = 10;

                foreach (var resultadoItem in resultado.Itens)
                {
                    var aluno = listaAlunos.First(x => x.alu_matricula == resultadoItem.Chave);
                    resultadoItem.Chave = aluno.alu_matricula;
                    resultadoItem.ChavePai = "{ 'esc_codigo':'" + aluno.esc_codigo + "', 'tur_codigo':'" + aluno.tur_codigo + "'}";
                    string repetente = "";

                    if (resultadoItem.CicloId == CicloCorrente)
                        repetente = " *";

                    if (menorCiclo < int.Parse(resultadoItem.CicloId))
                    {
                        menorCiclo = int.Parse(resultadoItem.CicloId);
                    }

                    if (quantidadeEscolasEnvolvidas > 1)
                    {
                        //Quando quantidadeEscolasEnvolvidas>1, inclui o nome da Escola seguido do código da Turma no título 
                        string esc_nome = aluno.esc_nome;
                        if (esc_nome.Length > 20)
                            esc_nome = aluno.esc_nome.Substring(0, 20);

                        resultadoItem.Titulo = "(" + esc_nome + "... " + aluno.tur_codigo + ") " + abreviarNome(resultadoItem.Titulo) + repetente;
                    }
                    else
                    {
                        //Inclui o código da Turma no título 
                        resultadoItem.Titulo = "(" + aluno.tur_codigo + ") " + abreviarNome(resultadoItem.Titulo) + repetente;
                    }
                }


                foreach (var aluno in listaAlunos)
                {
                    if (!resultado.Itens.Any(x => x.Titulo.IndexOf(aluno.alu_matricula) > 0))
                    {
                        if (quantidadeEscolasEnvolvidas > 1)
                        {
                            string esc_nome = aluno.esc_nome;
                            if (esc_nome.Length > 20)
                                esc_nome = aluno.esc_nome.Substring(0, 20);
                            resultado.Itens.Add(new ResultadoItem()
                            {
                                Valor = -1,
                                Titulo = "(" + esc_nome + "... " + aluno.tur_codigo + ") " + abreviarNome(aluno.Nome) + "(" + aluno.alu_matricula + ")"
                            });
                        }
                        else
                        {
                            resultado.Itens.Add(new ResultadoItem()
                            {
                                Valor = -1,
                                Titulo = "(" + aluno.tur_codigo + ") " + abreviarNome(aluno.Nome) + "(" + aluno.alu_matricula + ")"
                            });
                        }
                    }
                }

                resultado.Itens = resultado.Itens.OrderBy(x => x.Titulo).ToList();

                resultado.Agregacao = new List<ResultadoItem>();
            }

            return resultado;
        }

        public static Resultado RecuperarResultadoCicloAluno(string Edicao, int AreaConhecimentoID, string CicloId, string lista_alu_matricula, bool ExcluirSme_e_Dre)
        {
            var resultado = new Resultado();

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                var parametros = new DynamicParameters();

                parametros.Add("Edicao", Edicao, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 10);
                parametros.Add("AreaConhecimentoID", AreaConhecimentoID, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                var sbAlunos = new StringBuilder();
                var lista_alu_matriculaSplit = lista_alu_matricula.Split(',');
                if (lista_alu_matriculaSplit.Length == 0)
                {
                    return resultado;
                }

                //Construção dos parâmetros passados pela variável lista_alu_matricula de modo a evitar sql injection.
                foreach (var alu_matricula in lista_alu_matriculaSplit)
                {
                    if (sbAlunos.Length > 0)
                    {
                        sbAlunos.Append(",");
                    }
                    string parameterName = "@p_" + alu_matricula;
                    sbAlunos.Append(parameterName);
                    parametros.Add(parameterName, alu_matricula, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 50);
                }

                conn.Open();

                resultado.Agregacao = new List<ResultadoItem>();

                resultado.Itens = new List<ResultadoItem>();

                var resultadoItens_ALUNO = conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT r.NivelProficienciaID, 
                                                r.esc_codigo AS Chave, 
                                                r.CicloId, 
                                                r.alu_nome + ' (' + r.alu_matricula + ')' AS Titulo, 
                                                0 AS TotalAlunos, 
                                                COALESCE(r.Valor,-1) AS Valor, 
                                                0 AS PercentualAbaixoDoBasico, 
                                                0 AS PercentualBasico, 
                                                0 AS PercentualAdequado, 
                                                0 AS PercentualAvancado, 
                                                0 AS PercentualAlfabetizado
                                            FROM ResultadoCicloAluno r (NOLOCK)
                                            WHERE r.Edicao=@Edicao AND r.AreaConhecimentoID=@AreaConhecimentoID AND r.alu_matricula IN(" + sbAlunos.ToString() + @")
                                            ORDER BY r.alu_nome
                                            ",
                                        param: parametros
                                    ).ToList<ResultadoItem>();

                if (resultadoItens_ALUNO.Count() > 0)
                    parametros.Add("Ciclo", resultadoItens_ALUNO.First().CicloId, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 3);
                else
                    return resultado;

                if (!ExcluirSme_e_Dre)
                {
                    resultado.Itens.AddRange( //NÍVEL SME
                    conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT 'MÉDIA DA SME' AS Titulo, 
                                                'SME' AS Chave, 
                                                CicloId, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoCicloSme WITH (NOLOCK)
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND CicloId=@Ciclo",
                                        param: parametros
                                    ).ToList<ResultadoItem>()
                    );

                    var resultadoItens_DRE = conn.Query<ResultadoItem>(
                                            sql: @"
                                            SELECT NivelProficienciaID, 
                                                uad_sigla AS Chave, 
                                                CicloId, 
                                                '' AS Titulo, 
                                                TotalAlunos, 
                                                Valor, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoCicloDre WITH (NOLOCK)
                                            WHERE Edicao=@Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND CicloId=@Ciclo AND uad_sigla IN(SELECT uad_sigla FROM ResultadoCicloAluno WHERE Edicao=@Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND alu_matricula IN(" + sbAlunos.ToString() + @")) 
                                            ",
                                            param: parametros
                                        ).ToList<ResultadoItem>();
                    foreach (var dre in resultadoItens_DRE)
                    {
                        dre.Titulo = "MÉDIA DA DRE " + DataDRE.RecuperarNome(dre.Chave);
                    }
                    resultado.Itens.AddRange( //NÍVEL DRE
                            resultadoItens_DRE
                    );
                }

                resultado.Itens.AddRange( //NÍVEL ESCOLA
                    conn.Query<ResultadoItem>(
                                        sql: @"
                                            SELECT 'MÉDIA DA ESCOLA ' + e.esc_nome AS Titulo, 
                                                r.esc_codigo AS Chave, 
                                                r.CicloId, 
                                                NivelProficienciaID, 
                                                Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoCicloEscola r (NOLOCK)
                                            JOIN Escola (NOLOCK) e ON e.esc_codigo = r.esc_codigo 
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND CicloId=@Ciclo AND r.esc_codigo IN(SELECT esc_codigo FROM ResultadoCicloAluno WHERE Edicao=@Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND alu_matricula IN(" + sbAlunos.ToString() + @")) 
                                            ",
                                        param: parametros
                                    ).ToList<ResultadoItem>()
                    );

                resultado.Itens.AddRange( //NÍVEL TURMA
                                    conn.Query<ResultadoItem>(
                                                        sql: @"
                                            SELECT 'MÉDIA DA TURMA ' + r.tur_codigo AS Titulo, 
                                                r.tur_codigo AS Chave, 
                                                r.CicloId, 
                                                NivelProficienciaID, 
                                                COALESCE(Valor,0) AS Valor, 
                                                TotalAlunos, 
                                                PercentualAbaixoDoBasico, 
                                                PercentualBasico, 
                                                PercentualAdequado, 
                                                PercentualAvancado, 
                                                PercentualAlfabetizado
                                            FROM ResultadoCicloTurma r WITH (NOLOCK)
                                            JOIN Escola (NOLOCK) e ON e.esc_codigo = r.esc_codigo 
                                            WHERE Edicao = @Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND CicloId=@Ciclo AND r.tur_id IN(SELECT tur_id FROM ResultadoCicloAluno WHERE Edicao=@Edicao AND AreaConhecimentoID=@AreaConhecimentoID AND alu_matricula IN(" + sbAlunos.ToString() + @") ) 
                                            ",
                                                        param: parametros
                                                    ).ToList<ResultadoItem>()
                                    );

                resultado.Itens.AddRange( //NÍVEL ALUNO
                    resultadoItens_ALUNO
                    );
            }

            if (resultado.Itens.Count > 0)
            {
                resultado.CicloId = resultado.Itens.First().CicloId;
            }

            return resultado;
        }
    }
}