using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ImportarImagensRedacao
{
    public class Class1
    {
        static void Main(string[] args)
        {
            var importacaoRedacao2022 = new ImportacaoRespostasConstruidas();
            Console.WriteLine("Iniciar importação");
            Console.ReadKey();
            importacaoRedacao2022.Importacao2022RespostaConstruida();
            
          



            //Importacao2022();
        }

        private static void Importacao2021()
        {
            // Variaveis 

            // Caminhos Prod

            //var listaCodigoEscolas = new List<string>();
            // Z:\SERAP_PRD\Files\Imagens\2021\RD
            //string caminhoOrigemFotos = @"D:\imagens_2021 - Copy";
            //string caminhoDestinoFotos = @"Z:\SERAP_PRD\Files\Imagens\2021\RD\";
            // string caminhoTeste = @"Z:\SERAP_PRD\Files\Imagens\2021\teste";
            //string caminhoInscricaoNaoAchada = @"Z:\SERAP_PRD\Files\Imagens\2021\InscricaoNaoAchada\";
            //string caminhoInsertComErro = @"Z:\SERAP_PRD\Files\Imagens\2021\caminhoInsertComErro\";
            //string caminhoIncricaoNaoLiberada = @"Z:\SERAP_PRD\Files\Imagens\2021\caminhoIncricaoNaoLiberada\";

          //  local
           var listaCodigoEscolas = new List<string>();
            // Z:\SERAP_PRD\Files\Imagens\2021\RD
            string caminhoOrigemFotos = @"C:\Users\Convex\Documents\RedacaoPSP2022\";
            string caminhoDestinoFotos = @"C:\Users\Convex\Documents\SERAP\RD\";
            // string caminhoTeste = @"Z:\SERAP_PRD\Files\Imagens\2021\teste";
            string caminhoInscricaoNaoAchada = @"C:\Users\Convex\Documents\SERAP\InscricaoNaoAchada\";
            string caminhoInsertComErro = @"C:\Users\Convex\Documents\SERAP\caminhoInsertComErro\";
            string caminhoIncricaoNaoLiberada = @"C:\Users\Convex\Documents\SERAP\caminhoIncricaoNaoLiberada\";


            int imagensMovidasComSucesso = 0;
            int imagensMovidaComErro = 0;
            int insertComErro = 0;
            int insertComSucesso = 0;
            int imagensNaoLiberarMovida = 0;
            int imagensInscricaoNaoAchada = 0;
            int imagnesInscricaoNaoLiberada = 0;
            int contadorDeCentenas = 100;
            int imagensFaltantes = 0;

            var listaInsetComErro = new List<string>();

            Directory.CreateDirectory(caminhoDestinoFotos);
            Directory.CreateDirectory(caminhoInscricaoNaoAchada);
            Directory.CreateDirectory(caminhoInsertComErro);
            Directory.CreateDirectory(caminhoIncricaoNaoLiberada);



            // RenomeiaArquivosDeOrigem(caminhoOrigemFotos);
            CriarDiretoriosCodigoDasEscolas(caminhoDestinoFotos, listaCodigoEscolas);

            var listaCaminhoArquivos = ListaCaminhoArquivos(caminhoOrigemFotos);
            Console.WriteLine("Total de arquivos arquivos " + listaCaminhoArquivos.Count);
            Console.ReadKey();
            Console.WriteLine("Inicio do Processo.....................");

            imagensFaltantes = listaCaminhoArquivos.Count;
            foreach (var caminhoImagem in listaCaminhoArquivos)
            {
                imagensFaltantes = imagensFaltantes - 1;


                try
                {
                    var nomeArquivoCompleto = Path.GetFileName(caminhoImagem);

                    string inscricaoAluno = PegaInscricaoAluno(nomeArquivoCompleto);

                    if (!string.IsNullOrEmpty(inscricaoAluno))
                    {
                        using (SqlConnection connection = new SqlConnection(ConnectionStringProd))
                        {
                            SqlCommand command = new SqlCommand(QueryBuscaInformacoesAluno(inscricaoAluno), connection);
                            try
                            {
                                string esc_codigo = string.Empty;
                                string alu_matricula = string.Empty;
                                string alu_nome = string.Empty;
                                bool liberar = true;

                                if (connection.State != ConnectionState.Open)
                                    connection.Open();

                                SqlDataReader reader = command.ExecuteReader();
                                SqlCommand commandInsert = connection.CreateCommand();
                                while (reader.Read())
                                {
                                    esc_codigo = reader["esc_codigo"].ToString();
                                    alu_matricula = reader["alu_matricula"].ToString();
                                    alu_nome = reader["alu_nome"].ToString();
                                    liberar = Convert.ToBoolean(reader["liberar"]);
                                }

                                if (string.IsNullOrEmpty(esc_codigo) ||
                                    string.IsNullOrEmpty(esc_codigo) ||
                                    string.IsNullOrEmpty(alu_nome))
                                {
                                    string caminhoInscricaoNaoAchadaCompleto = caminhoInscricaoNaoAchada + nomeArquivoCompleto;
                                    File.Move(caminhoImagem, caminhoInscricaoNaoAchadaCompleto);
                                    imagensInscricaoNaoAchada = imagensInscricaoNaoAchada + 1;
                                }

                                else if (liberar == false)
                                {
                                    File.Move(caminhoImagem, caminhoIncricaoNaoLiberada + nomeArquivoCompleto);
                                    imagensNaoLiberarMovida = imagensNaoLiberarMovida + 1;

                                }

                                else
                                {

                                    string caminhoDestinoBanco = "Imagens" + "/" + "2021" + "/" + "RD" + "/" + esc_codigo + "/" + nomeArquivoCompleto;
                                    string caminhoDestinoCompleto = caminhoDestinoFotos + esc_codigo + "\\" + nomeArquivoCompleto;
                                    File.Move(caminhoImagem, caminhoDestinoCompleto);
                                    reader.Close();

                                    if (File.Exists(caminhoDestinoCompleto))
                                    {
                                        imagensMovidasComSucesso = imagensMovidasComSucesso + 1;
                                        if (imagensMovidasComSucesso == contadorDeCentenas)
                                        {
                                            Console.WriteLine("Imagens movidas com sucesso:" + imagensMovidasComSucesso);
                                            contadorDeCentenas = contadorDeCentenas + 100;
                                            Console.WriteLine("Imagens faltantes" + imagensFaltantes);
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
                                                       ('2021', 4," + "'" + esc_codigo + "' , '" + alu_matricula + "' , 'Redação', 1, " + "'" + alu_nome + "', '" + caminhoDestinoBanco + "');";

                                            var ret = commandInsert.ExecuteNonQuery();
                                            connection.Close();

                                            insertComSucesso = insertComSucesso + 1;

                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Insert com erro: {0}", insertComErro = insertComErro + 1);
                                            Console.WriteLine("ERRO INSERT " + ex.Message);
                                            listaInsetComErro.Add(alu_matricula);
                                            File.Move(caminhoDestinoCompleto, caminhoInsertComErro + nomeArquivoCompleto);
                                            connection.Close();
                                            
                                        }
                                    }

                                    else
                                    {
                                        File.Move(caminhoImagem, caminhoInscricaoNaoAchada);
                                        imagensMovidaComErro = imagensMovidaComErro + 1;
                                        connection.Close();
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("ERRO NAO TRATADO___AREA_BANCO ");
                                Console.WriteLine("CAMINHO IMAGEM " + caminhoImagem);
                                Console.WriteLine(ex.Message);
                                connection.Close();
                                Console.ReadKey();
                            }
                        }
                    }

                    else
                    {
                        //Se não achou o caminho move para pasta de erro.
                        File.Move(caminhoImagem, caminhoInscricaoNaoAchada);
                        imagensMovidaComErro = imagensMovidaComErro + 1;
                        Console.WriteLine("Imagem movida com erro: " + imagensMovidaComErro);

                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERRO NAO TRATADO ");
                    Console.WriteLine("CAMINHO IMAGEM " + caminhoImagem);
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                    throw;
                }
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
                    Console.ReadKey();
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
           "Data Source=local;Initial Catalog=local;User Id=sa;Password=Antares2014;";

        private static string ConnectionStringProd =
          @"Data Source=XXXXXXX ;Initial Catalog=GestaoAvaliacao;User Id=Caique.Santos;Password=Antares2014;";

        private static string QueryBuscaInformacoesAluno(string incricao)
        {
            return @" 
        select distinct '2021' as edicao,
                        4 as AreaConhecimentoId, 
		                esc.esc_codigo,
		                alu.alu_matricula, 
		                'Redação' as questao,
		                 1 as pagina, 
		                 alu.alu_nome,
                         insc._liberar_ as liberar,
		                'Imagens/2021/RD/'+ esc.esc_codigo +  convert(varchar,insc._Inscricao_) +'.jpg' as Caminho
		 from Manutencao..dados_inscrica_redacao_liberar insc   -- 240096
         inner join   GestaoAvaliacao_SGP..[ACA_Aluno] alu on alu.alu_matricula = insc._alu_matricula_ -- 235943
         inner join GestaoAvaliacao_SGP..[MTR_MatriculaTurma] mtr on mtr.alu_id = alu.alu_id
         inner join GestaoAvaliacao_SGP..esc_escola esc  on esc.esc_id = mtr.esc_id 
         where  year(mtr.mtu_dataMatricula) = 2021
           and _Inscricao_ = " + incricao.ToString() + @" order by esc_codigo, alu_nome";
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



