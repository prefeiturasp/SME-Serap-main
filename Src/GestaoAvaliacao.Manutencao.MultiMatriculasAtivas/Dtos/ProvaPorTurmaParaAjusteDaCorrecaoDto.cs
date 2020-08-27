using System;

namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Dtos
{
    internal class ProvaPorTurmaParaAjusteDaCorrecaoDto
    {
        public long TestId { get; set; }
        public long TurmaId { get; set; }
        public Guid EntidadeId { get; set; }
    }
}