using System;
using System.Collections.Generic;

namespace AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva
{
    public class DRE
    {
        public Guid _id { get; set; }
        public Guid DreId { get; set; }
        public string Nome { get; set; }
        public AlunoStatus AlunoStatus { get; set; }
        public ProcessamentoStatus ProcessamentoStatus { get; set; }
        public int ProvaId { get; set; }
        public ICollection<Escola> Escolas { get; set; }
        public bool Ativo { get; set; }       

        public DRE()
        {
            AlunoStatus = new AlunoStatus();
            ProcessamentoStatus = new ProcessamentoStatus();
            Escolas = new List<Escola>();
            Ativo = true;
        }
    }
}
