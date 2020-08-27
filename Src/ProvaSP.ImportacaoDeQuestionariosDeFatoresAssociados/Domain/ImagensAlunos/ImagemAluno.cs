using ImportacaoDeQuestionariosSME.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Domain.ImagensAlunos
{
    public class ImagemAluno : Entity
    {
        public ImagemAluno()
            : base()
        {
        }

        public string Edicao { get; set; }
        public int AreaConhecimentoId { get; set; }
        public string EscCodigo { get; set; }
        public string AluMatricula { get; set; }
        public string Questao { get; set; }
        public int Pagina { get; set; }
        public string AluNome { get; set; }
        public string Caminho { get; set; }
    }
}