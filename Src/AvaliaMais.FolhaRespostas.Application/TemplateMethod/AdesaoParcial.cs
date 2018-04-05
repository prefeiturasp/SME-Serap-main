using AvaliaMais.FolhaRespostas.Domain.ProcessamentoInicial;
using System.Collections.Generic;

namespace AvaliaMais.FolhaRespostas.Application.TemplateMethod
{
    public class AdesaoParcial : AbstractProcessamento
	{
		protected override IEnumerable<Processamento> CarregarProcessamento(int provaId)
		{
			return processamentoInicialService.ObterProcessamentoProva(provaId);
		}
	}
}
