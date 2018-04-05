using ProvaSP.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvaSP.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessarListaPresenca();

            ProcessarBaseUsuarioOffline();
        }

        private static void ProcessarListaPresenca()
        {
            System.Console.WriteLine("Esse programa é responsável pela criação dos arquivos .csv de turmas.");
            System.Console.WriteLine("Os dados são obtidos a partir da tabela ProvaSP.Aluno. Esses arquivos produzidos ");
            System.Console.WriteLine("são utilizados no módulo de chamada no app da ProvaSP.");
            System.Console.WriteLine("");
            System.Console.WriteLine("Os três seguintes campos podem ser encontrados no banco GestaoAvaliacao_SGP nas seguintes tabelas e campos:");
            System.Console.WriteLine("TUR_Turma.tur_id, ACA_Aluno.alu_matricula, ACA_Aluno.alu_nome.");
            System.Console.WriteLine("");
            System.Console.WriteLine("A ideia é separar o arquivo de entrada (aproximadamente 10 megas) em 10 arquivos.");
            System.Console.WriteLine("de aproximadamente de 1 mega. A separação é feita pelo final do tur_id.");
            System.Console.WriteLine("Dessa forma a leitura offline do app é aliviada.");
            System.Console.WriteLine("");
            System.Console.WriteLine("Recuperando alunos para composição da lista de presença... (aguarde aproximadamente 1 minuto)");

            string destino = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("ProvaSP.Console", "|").Split('|')[0];
            destino += @"ProvaSP.App\www\AppProvaSP\turmas\";

            var arquivos = Directory.GetFiles(destino);
            foreach (var arquivo in arquivos)
            {
                File.Delete(arquivo);
            }

            var baseListaPresenca = DataTurma.RecuperarListaPresenca();

            var sb = new StringBuilder();


            string tur_id_ultimaLeitura = "";
            int linhaAtual = 0;

            foreach (var listaPresenca in baseListaPresenca)
            {
                linhaAtual++;


                string tur_id = listaPresenca.tur_id.ToString();
                string alu_matricula = listaPresenca.alu_matricula;
                string alu_nome = listaPresenca.alu_nome;

                
                if (tur_id_ultimaLeitura != tur_id || linhaAtual == baseListaPresenca.Count)
                {
                    if (tur_id_ultimaLeitura != "")
                    {
                        System.Console.WriteLine("Processando tur_id " + tur_id_ultimaLeitura);
                        string tur_id_final = tur_id_ultimaLeitura.Substring(tur_id_ultimaLeitura.Length - 1);
                        using (var sw = new StreamWriter(destino + tur_id_final + ".csv", true))
                        {
                            sw.Write(sb.ToString());
                        }

                        sb = new StringBuilder();
                    }
                }

                sb.Append(tur_id);
                sb.Append(";");
                sb.Append(alu_matricula);
                sb.Append(";");
                sb.AppendLine(alu_nome);

                tur_id_ultimaLeitura = tur_id;
            }
        }

        private static void ProcessarBaseUsuarioOffline()
        {
            var baseUsuarioOffline = DataUsuario.RetornarBaseUsuarioOffline();

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(baseUsuarioOffline);

            //C:\sme\serap\Src\ProvaSP.Console\bin\Debug\ProvaSP.Console.exe
            string destino = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("ProvaSP.Console", "|").Split('|')[0];
            destino += @"ProvaSP.App\www\loginOffline.json";
            System.IO.File.WriteAllText(destino, json);
            System.Console.WriteLine(destino + " CRIADO COM SUCESSO!" );
            
        }
    }
}
