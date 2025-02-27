using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using System.Globalization;


namespace ImportarImagensRedacao
{
    public class ImportacaoRespostasConstruidas
    {
        public void Importacao2022RespostaConstruida()
        {
            var log = new Log();
            try
            {
                // Variaveis 


                var listaCodigoEscolas = new List<string>();
                int linhasFaltantes = 0;
                int linhaAtual = 0;
                var listaInsetComErro = new List<string>();
                var listaCsvAlunos = new List<PSPRC2022>();
                var contadorCentena = 100;
                // Caminhos Prod

            
             //   string caminhoArquivoLeitura = @"Z:\SERAP_PRD\Files\Imagens\TesteRedacao2022\PSP22_PA2_FOLHA_DE_RESPOSTA_PT_3_A_9_ANO.csv";
                //  local

                //string caminhoOrigemFotos = @"C:\Users\caique.siqueira\Documents\RedacaoPSP2022\";
                //string caminhoDestinoFotos = @"C:\Users\caique.siqueira\Documents\SERAP\RD\";
                //string caminhoInscricaoNaoAchada = @"C:\Users\caique.siqueira\Documents\SERAP\InscricaoNaoAchada\";
                //string caminhoInsertComErro = @"C:\Users\caique.siqueira\Documents\SERAP\caminhoInsertComErro\";
                string caminhoArquivoLeitura = @"C:\Users\caique.siqueira\Desktop\Resposta Construída\base_psp_Lp_2ano.csv";


                Console.WriteLine("Inicio do Processo.....................");
                Console.ReadKey();
                
                Console.WriteLine("Leitura do Arquivo CSV");


                using (var reader = new StreamReader(caminhoArquivoLeitura, encoding: Encoding.UTF8))
                {
                    var csv = new CsvReader(reader, config);
                    listaCsvAlunos = csv.GetRecords<PSPRC2022>().ToList();
                }


                linhasFaltantes = listaCsvAlunos.Count();
                Console.WriteLine("Quantidade de linhas -- {0}", listaCsvAlunos.Count());

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {

                    foreach (var csvAluno in listaCsvAlunos)
                    {
                        linhaAtual = linhaAtual + 1;
                        try
                        {
                            if (connection.State != ConnectionState.Open)
                                connection.Open();
                            SqlCommand commandInsert = connection.CreateCommand();
                            commandInsert.CommandText = InsereInformacoesAlunoRespostaConstruidaLP2Ano(csvAluno.Cod_EOL, csvAluno.questao, int.Parse(csvAluno.pagina));
                            var ret = commandInsert.ExecuteNonQuery();
                            linhasFaltantes = linhasFaltantes - ret;
                        



                        }

                        catch (Exception ex)
                        {
                       
                            log.GravarLog($"03 - Erro Insert - Cod Eol aluno: {csvAluno.Cod_EOL} -- Mensagem Erro {ex.Message}  ");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                log.GravarLog($"04 - ErroNão tratado  -- Mensagem Erro {ex.Message}  ");
            }
        }

        private static string InsereInformacoesAlunoRespostaConstruidaLP2Ano(string codEolAluno, string questao, int pagina)
        {
            return $@"INSERT INTO   PROVASP..IMAGEMALUNO

                select '2022' as edicao,
                        2 as AreaConhecimentoId, 
		                REPLICATE('0', 6 - LEN(insc.esc_eol)) + RTrim(insc.esc_eol) as esc_codigo,
		                alu.alu_matricula, 
		                questao,
		                pagina, 
		                alu.alu_nome,
                      'Imagens/2022/2AnoRC/'+insc.tipo_esc+ '/' + insc.DRE + '/' +
					   insc.esc_eol + '/' + insc.ano_escolar + '/' + turma + '/' +
					   insc.nome_arquivo as Caminho
        
         from Manutencao..[base_psp_Lp_2ano] insc 
         inner join    GestaoAvaliacao_SGP..[ACA_Aluno] as alu on alu.alu_matricula = insc.eol_aluno

         inner join ProvaSP..Escola e on REPLICATE('0', 6 - LEN(insc.esc_eol)) + RTrim(insc.esc_eol) = e.esc_codigo

       where alu.alu_matricula = {codEolAluno}
         and questao = {questao}
         and pagina = {pagina}";
        }

        private static string QueryBuscaInformacoesAluno(int areaConhecimentoId, string codEolAluno)
        {
            return $@" 
        select  distinct '2022' as edicao,
                        {areaConhecimentoId} as AreaConhecimentoId, 
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

        private static string ConnectionString =
       "Data Source=10.49.16.23;Initial Catalog=GestaoAvaliacao;User Id=Caique.Santos;Password=Antares2014";

        private static string ConnectionStringHMl =
           "Data Source=XXXXXXX;Initial Catalog=GestaoAvaliacao;User Id=XXXXXX;Password=XXXXX;";

        private static string ConnectionStringProd =
          @"Data Source=XXXXXXX ;Initial Catalog=GestaoAvaliacao;User Id=XXXXXX;Password=XXXXX;";
    }
}
