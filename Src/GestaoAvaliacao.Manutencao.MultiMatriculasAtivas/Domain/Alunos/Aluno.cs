using System;

namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Domain.Alunos
{
    public class Aluno
    {
        public long AlunoId { get; set; }
        public string Nome { get; set; }
        public string Matricula { get; set; }
        public Guid EntidadeId { get; set; }
    }
}