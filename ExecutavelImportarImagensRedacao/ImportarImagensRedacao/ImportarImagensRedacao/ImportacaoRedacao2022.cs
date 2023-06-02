using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using static System.Net.Mime.MediaTypeNames;

namespace ImportarImagensRedacao
{
    public class ImportacaoRedacao2022
    {

        public void Importacao2022()
        {
            try
            {
               // Caminhos Prod

                string caminhoOrigemFotos = @"Z:\SERAP_PRD\Files\RedacaoPSP2022\";
                string caminhoDestinoFotos = @"Z:\SERAP_PRD\Files\Imagens\2022\RD\";
                string caminhoInscricaoNaoAchada = @"Z:\SERAP_PRD\Files\Imagens\2022\InscricaoNaoAchada\";
                string caminhoInsertComErro = @"Z:\SERAP_PRD\Files\Imagens\2022\caminhoInsertComErro\";
                string caminhoArquivoLeitura = @"Z:\SERAP_PRD\Files\Imagens\TesteRedacao2022\PSP22_PA2_FOLHA_DE_RESPOSTA_PT_3_A_9_ANO.csv";
                //  local

                //string caminhoOrigemFotos = @"C:\Users\caique.siqueira\Documents\RedacaoPSP2022\";
                //string caminhoDestinoFotos = @"C:\Users\caique.siqueira\Documents\SERAP\RD\";
                //string caminhoInscricaoNaoAchada = @"C:\Users\caique.siqueira\Documents\SERAP\InscricaoNaoAchada\";
                //string caminhoInsertComErro = @"C:\Users\caique.siqueira\Documents\SERAP\caminhoInsertComErro\";
                //string caminhoArquivoLeitura = @"C:\Users\caique.siqueira\Documents\Caique\ArquivoTeste.csv";

                // Variaveis 

                var log = new Log();
                var listaCodigoEscolas = new List<string>();
                int imagensMovidasComSucesso = 0;
                int imagensMovidaComErro = 0;
                int insertComErro = 0;
                int insertComSucesso = 0;
                int imagensNaoLiberarMovida = 0;
                int imagensInscricaoNaoAchada = 0;
                int contadorDeDezenas = 10;
                int linhasFaltantes = 0;
                int linhaArquivo = 0;

                var listaInsetComErro = new List<string>();
                var listaCsvAlunos = new List<PSP22Dto>();
                try
                {


                    Console.WriteLine("------------Criação dos diretórios-------------------");
                    Console.WriteLine("caminhoDestinoFotos: {0}", caminhoDestinoFotos);
                    Directory.CreateDirectory(caminhoDestinoFotos);

                    Console.WriteLine("caminhoInscricaoNaoAchada: {0}", caminhoInscricaoNaoAchada);
                    Directory.CreateDirectory(caminhoInscricaoNaoAchada);
                   
                    Console.WriteLine("caminhoInscricaoNaoAchada: {0}", caminhoInsertComErro);
                    Directory.CreateDirectory(caminhoInsertComErro);
                    
                 

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro na Criação dos diretórios");
                    Console.ReadKey();
                }

                Console.WriteLine("Inicio do Processo.....................");
                Console.ReadKey();
                Console.WriteLine("Leitura do Arquivo CSV");

                using (var reader = new StreamReader(caminhoArquivoLeitura, encoding: Encoding.UTF8))
                {
                    var csv = new CsvReader(reader, config);
                    listaCsvAlunos = csv.GetRecords<PSP22Dto>().ToList();
                }


                linhasFaltantes = listaCsvAlunos.Count();
                Console.WriteLine("Quantidade de linhas -- {0}", listaCsvAlunos.Count());
                
                foreach (var csvAluno in listaCsvAlunos)
                {
                    linhaArquivo += 1;
                    
                    Console.WriteLine($"Linha {linhaArquivo}  Arquivo");

                    var caminhoDestinoPorTurma = $"{caminhoDestinoFotos}\\{csvAluno.Tipo_escola}\\{csvAluno.DRE}\\{csvAluno.ESC_EOL}\\{csvAluno.Ano_Escolar}\\{csvAluno.Turma}";
                    var caminhoImagem = $"{caminhoOrigemFotos}{csvAluno.Tipo_escola}\\{csvAluno.DRE}\\{csvAluno.ESC_EOL}\\{csvAluno.Ano_Escolar}\\{csvAluno.Turma}\\{csvAluno.Desc_Arquivo}";
                    Directory.CreateDirectory(caminhoDestinoPorTurma);

                    string inscricaoAluno = PegaInscricaoAluno(csvAluno.Desc_Arquivo);

                    try
                    {

                        string esc_codigo = string.Empty;
                        string alu_matricula = string.Empty;
                        string alu_nome = string.Empty;

                        var nomeArquivoCompleto = Path.GetFileName(caminhoImagem);
                        if (!string.IsNullOrEmpty(inscricaoAluno))
                        {
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlCommand command = new SqlCommand(QueryBuscaInformacoesAluno(inscricaoAluno, csvAluno.Cod_EOL), connection);
                                try
                                {

                                    if (connection.State != ConnectionState.Open)
                                        connection.Open();

                                    SqlDataReader reader = command.ExecuteReader();
                                    SqlCommand commandInsert = connection.CreateCommand();
                                    while (reader.Read())
                                    {
                                        esc_codigo = reader["esc_codigo"].ToString();
                                        alu_matricula = reader["alu_matricula"].ToString();
                                        alu_nome = reader["alu_nome"].ToString();
                                    }

                                    if (string.IsNullOrEmpty(esc_codigo) ||
                                        string.IsNullOrEmpty(esc_codigo) ||
                                        string.IsNullOrEmpty(alu_nome))
                                    {
                                        string caminhoInscricaoNaoAchadaCompleto = caminhoInscricaoNaoAchada + csvAluno.Desc_Arquivo;
                                        File.Move(caminhoImagem, caminhoInscricaoNaoAchadaCompleto);
                                        imagensInscricaoNaoAchada = imagensInscricaoNaoAchada + 1;
                                        linhasFaltantes = linhasFaltantes - 1;
                                        log.GravarLog($"01 - Aluno não Encontrato: {csvAluno.Cod_EOL} linha arquivo:  {linhaArquivo}");
                                    }
                                    else
                                    {

                                       
                                        string caminhoDestinoBanco = $"Imagens/2022/RD/{csvAluno.Tipo_escola}/{csvAluno.DRE}/{csvAluno.ESC_EOL}/{csvAluno.Ano_Escolar}/{csvAluno.Turma}/{csvAluno.Desc_Arquivo}";
                                        string caminhoDestinoCompleto = $"{caminhoDestinoPorTurma}\\{csvAluno.Desc_Arquivo}";
                                        File.Move(caminhoImagem, caminhoDestinoCompleto);
                                        reader.Close();

                                        if (File.Exists(caminhoDestinoCompleto))
                                        {
                                            imagensMovidasComSucesso = imagensMovidasComSucesso + 1;
                                            if (imagensMovidasComSucesso == contadorDeDezenas)
                                            {
                                                Console.WriteLine("Imagens movidas com sucesso: s" + imagensMovidasComSucesso);
                                                contadorDeDezenas = contadorDeDezenas + 10;
                                                Console.WriteLine($"LinhasFaltantes: {linhasFaltantes - 1}");
                                                Console.WriteLine($"LinhaAtual: {listaCsvAlunos.Count - linhasFaltantes}");
                                                Console.WriteLine($"TipoEscola: {csvAluno.Tipo_escola} Dre:{csvAluno.DRE} Escola: {csvAluno.ESC_EOL} " +
                                                    $"Turma {csvAluno.Turma}");
                                            }
                                            try
                                            {
                                                commandInsert.CommandText =

                                                           @"INSERT INTO [ProvaSP].[dbo].[ImagemAluno]
                                                       ([Edicao]
                                                       ,[AreaConhecimentoID]
                                                       ,[esc_codigo]
                                                       ,[alu_matricula]
                                                       ,[questao]
                                                       ,[pagina]
                                                       ,[alu_nome]
                                                       ,[caminho])
                                                 VALUES
                                                       ('2022', 4," + "'" + esc_codigo + "' , '" + alu_matricula + "' , 'Redação', 1, " + "'" + alu_nome + "', '" + caminhoDestinoBanco + "');";

                                                var ret = commandInsert.ExecuteNonQuery();
                                                connection.Close();

                                                insertComSucesso = insertComSucesso + 1;
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine("Insert com erro:{0}", insertComErro = insertComErro + 1);
                                                Console.WriteLine("ERRO INSERT " + ex.Message);
                                                listaInsetComErro.Add(alu_matricula);
                                                File.Move(caminhoImagem, caminhoInsertComErro + csvAluno.Desc_Arquivo);
                                                linhasFaltantes = linhasFaltantes - 1;
                                                insertComErro = insertComErro + 1;
                                                connection.Close();

                                                log.GravarLog($"02 - Aluno não Encontrato: {csvAluno.Cod_EOL} linha arquivo:  {linhaArquivo}");

                                            }
                                        }

                                        else
                                        {
                                            File.Move(caminhoImagem, caminhoDestinoPorTurma, true);
                                            imagensMovidaComErro = imagensMovidaComErro + 1;
                                            linhasFaltantes = linhasFaltantes - 1;
                                            connection.Close();

                                            log.GravarLog($"03 - Arquivo não encontrado: {csvAluno.Cod_EOL} linha arquivo:  {linhaArquivo}");
                                        }

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("ERRO NAO TRATADO___AREA_BANCO ");
                                    Console.WriteLine("CAMINHO IMAGEM " + caminhoImagem);
                                    Console.WriteLine(ex.Message);
                                    connection.Close();
                                    log.GravarLog($"04 - Erro de banco de dados- CodEoAluno:{csvAluno.Cod_EOL} linha arquivo:  {linhaArquivo}-- Exception: {ex.Message}");
                                }


                            }
                        }

                        else
                        {
                            //Se não achou o caminho move para pasta de erro.
                            File.Move(caminhoImagem, caminhoInscricaoNaoAchada);
                            imagensMovidaComErro = imagensMovidaComErro + 1;
                            Console.WriteLine("Imagem movida com erro: " + imagensMovidaComErro);
                            log.GravarLog($"05 - Imagem movida com erro: {csvAluno.Cod_EOL} linha arquivo:  {linhaArquivo}");
                        }

                    }
                    catch (Exception ex)
                    {
                        log.GravarLog($"06  Erro não tratado geral  : {csvAluno.Cod_EOL} linha arquivo:  {linhaArquivo} Exception: {ex.Message}");

                        Console.WriteLine(ex.Message);
                        log.GravarLog(caminhoImagem + "ERRO" + ex.Message);


                    }

                    linhasFaltantes = linhasFaltantes - 1;
                }
                Console.WriteLine("**************************FIM DO PROCESSO*********************");
                Console.WriteLine("imagemMovidasComSucesso: " + imagensMovidasComSucesso);
                Console.WriteLine("imagemMovidaComErro: " + imagensMovidaComErro);
                Console.WriteLine("imagensNaoLiberarMovida: " + imagensNaoLiberarMovida);
                Console.WriteLine("Imagens inscrição não encontrada ou redação em branco: " + imagensInscricaoNaoAchada);
                Console.WriteLine("insertComErro: " + insertComErro);
                Console.WriteLine("insertComSucesso:" + insertComSucesso);

                Console.WriteLine("-------------------------------------------------------------");
                Console.ReadKey();

                if (listaInsetComErro.Count > 0)
                {
                    Console.WriteLine("Lista de Codigo aluno insert com erro");

                    foreach (var matricula in listaInsetComErro)
                    {
                        Console.WriteLine(matricula);
                    }
                }
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                var log = new Log();
                log.GravarLog($"00 Erro de inicialização de variaveis; Exception {ex.Message}");
                throw;
            }



        }


        private static string PegaInscricaoAluno(string nomeArquivoCompleto)
        {
            try
            {
                return nomeArquivoCompleto.Substring(0, 8);
            }
            catch
            {
                return null;
            }

        }

        private static List<string> ListaCaminhoArquivos(string diretorioAtual)
        {
            try
            {
                var Diretorios = Directory.GetDirectories(diretorioAtual);

                var listaCaminhoArquivos = new List<string>();

                foreach (var diretorio in Diretorios)
                {
                    var arquivosDiretorio = Directory.GetFiles(diretorio);


                    foreach (var arquivo in arquivosDiretorio)
                    {
                        listaCaminhoArquivos.Add(arquivo);
                    }
                }

                return listaCaminhoArquivos;

            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO ** LISTA CAMINHO ARQUIVOS **" + ex.Message);
                Console.ReadKey();
                throw;
            }
        }

        private static void CriarDiretoriosCodigoDasEscolas(string caminhoDestinoFotos, List<string> listaCodigoEscolas)
        {
            Console.WriteLine("Buscando codigo das Escolas..");
            pegaCodigoEscolas(listaCodigoEscolas);

            Console.WriteLine("Criando pastas com os codigos das escolas.");
            foreach (var codigoEscola in listaCodigoEscolas)
            {
                Directory.CreateDirectory(caminhoDestinoFotos + codigoEscola);
            }

            Console.WriteLine("Pastas com os nomes das escolas criadas.");
            Console.ReadKey();
        }

        private static void pegaCodigoEscolas(List<string> listaCodigoEscolas)
        {
            using (SqlConnection connection =
            new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(QueryCodigoEscolas, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        listaCodigoEscolas.Add(reader["esc_codigo"].ToString());
                    }
                    reader.Close();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao pegar codigo escolas");
                    Console.WriteLine(ex.Message);

                }
            }
        }

        private static void RenomeiaArquivosDeOrigem(string caminhoOrigemFotos)
        {
            var arquivos = Directory.GetDirectories(caminhoOrigemFotos);


            var qtdarquivos = arquivos.Length;


            int numeroArquivo = 1;
            foreach (var nomeArquivo in arquivos)
            {

                Directory.Move(nomeArquivo, caminhoOrigemFotos + numeroArquivo.ToString());
                numeroArquivo++;
            }
        }



        private static string QueryCodigoEscolas = @"  
                               select esc.esc_codigo as esc_codigo
                        		 from Manutencao..dados_inscrica_redacao_liberar insc   -- 240096
                                 inner join   GestaoAvaliacao_SGP..[ACA_Aluno] alu on alu.alu_matricula = insc._alu_matricula_ -- 235943
                                 inner join GestaoAvaliacao_SGP..[MTR_MatriculaTurma] mtr on mtr.alu_id = alu.alu_id
                                 inner join GestaoAvaliacao_SGP..esc_escola esc  on esc.esc_id = mtr.esc_id 
                                 where  year(mtr.mtu_dataMatricula) = 2021
                                group by esc.esc_codigo";

        private static string ConnectionString =
           "Data Source=10.49.16.23;Initial Catalog=GestaoAvaliacao;User Id=Caique.Santos;Password=Antares2014;";

        private static string ConnectionStringHMl =
           "Data Source=10.49.19.159;Initial Catalog=GestaoAvaliacao;User Id=Caique.Santos;Password=Antares2014;";

        private static string ConnectionStringProd =
          @"Data Source=XXXXXXX ;Initial Catalog=GestaoAvaliacao;User Id=Caique.Santos;Password=Antares2014;";

        private static string QueryBuscaInformacoesAluno(string inscricao, string codEolAluno)
        {
            return $@" 
        select  distinct '2022' as edicao,
                        4 as AreaConhecimentoId, 
		                esc.esc_codigo,
		                alu.alu_matricula, 
		                'Redação' as questao,
		                 1 as pagina, 
		                 alu.alu_nome
		  from  GestaoAvaliacao_SGP..[ACA_Aluno]  as alu
         inner join GestaoAvaliacao_SGP..[MTR_MatriculaTurma] mtr on mtr.alu_id = alu.alu_id
         inner join GestaoAvaliacao_SGP..esc_escola esc  on esc.esc_id = mtr.esc_id 
         where    alu.alu_matricula = {codEolAluno}
		    and year(mtr.mtu_dataMatricula) = 2022
           order by esc_codigo, alu_nome";
        }

        private static CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";",
            MissingFieldFound = null,
            IgnoreBlankLines = true,
            ShouldSkipRecord = records =>
            {
                var linha = records.Row.Parser.RawRecord.Replace(Environment.NewLine, string.Empty);
                linha = linha.Trim().Replace("\r", string.Empty);
                linha = linha.Trim().Replace("\n", string.Empty);
                linha = linha.Trim().Replace("\0", string.Empty);

                var arrayLinha = records.Row.Parser.Record;
                return string.IsNullOrEmpty(linha) || arrayLinha == null || arrayLinha.Length == 0 ||
                       (arrayLinha.Length > 0 && string.IsNullOrEmpty(arrayLinha[0]));
            }
        };


        public static CsvReader ObterArquivoCSV(string caminhoCsv, string nomeArquivo)
        {
            string path = $"caminhoCsv/{nomeArquivo}";
            var reader = new StreamReader(path, encoding: Encoding.UTF8);
            return new CsvReader(reader, config);
        }

    }
}
