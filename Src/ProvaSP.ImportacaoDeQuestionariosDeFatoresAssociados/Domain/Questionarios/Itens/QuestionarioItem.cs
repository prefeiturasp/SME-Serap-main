using ImportacaoDeQuestionariosSME.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Domain.Questionarios.Itens
{
    public class QuestionarioItem : Entity
    {
        public QuestionarioItem()
            :base()
        {
            Opcoes = new List<Opcao>();
        }

        public int Numero { get; set; }
        public string Titulo { get; set; }

        public string Enunciado { get; set; }
        public ICollection<Opcao> Opcoes { get; set; }

        public void MontarQuestoes()
        {
            Enunciado = Titulo.Substring(0, Titulo.IndexOf("A)"));
            var respostasDisponiveis = Titulo.Substring(Titulo.IndexOf("A)"), Titulo.Length - Titulo.IndexOf("A)")).Trim();
            var partes = respostasDisponiveis.Split('.');

            for(var i = 0; i <= partes.Count() - 1; i++)
            {
                try
                {
                    var parte = partes[i].Trim();
                    if (string.IsNullOrWhiteSpace(parte)) continue;
                    var opcao = new Opcao
                    {
                        Descricao = parte,
                        Letra = parte.Substring(0, 1),
                        Numero = i + 1
                    };
                    Opcoes.Add(opcao);
                }
                catch(Exception ex)
                {

                }
            }
        }

        public void MontarTitulo()
        {
            if (string.IsNullOrWhiteSpace(Titulo)) return;
            if (Opcoes is null || !Opcoes.Any()) return;

            var result = $"{Enunciado} [";
            foreach(var opcao in Opcoes)
            {
                result += $"{opcao.Descricao} ";
            }

            Titulo = result;
        }
    }

    public class Opcao
    {
        public int Numero { get; set; }
        public string Letra { get; set; }
        public string Descricao { get; set; }
    }
}
