using AvaliaMais.FolhaRespostas.Application.Interfaces;
using AvaliaMais.FolhaRespostas.Application.TemplateMethod;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoInicial.Interfaces;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva.Interfaces;

namespace AvaliaMais.FolhaRespostas.Application
{
    public class ProcessamentoAppServiceWrite : IProcessamentoAppServiceWrite
	{
		private readonly IProcessamentoInicialService _processamentoInicialService;
		private readonly IProcessamentoProvaService _processamentoProvaService;
		private AbstractProcessamento _processamento;
		private int _provaId;

		public ProcessamentoAppServiceWrite(IProcessamentoInicialService processamentoInicialService,
			IProcessamentoProvaService processamentoProvaService)
		{
			_processamentoInicialService = processamentoInicialService;
			_processamentoProvaService = processamentoProvaService;
		}

		public bool AdicionarProcessamento(int provaId, AbstractProcessamento tipoProcessamento)
		{
			_provaId = provaId;
			if(InativarDocumentosProva())
			{
				_processamento = DefinirTipoProcessamento(tipoProcessamento);
				return AtualizarDocumentos();
			}
			return false;
		}

		private bool InativarDocumentosProva()
		{
			return _processamentoProvaService.InativarDocumentos(_provaId);
		}

		private AbstractProcessamento DefinirTipoProcessamento(AbstractProcessamento tipoProcessamento)
		{
			_processamento = tipoProcessamento;
			_processamento.processamentoInicialService = _processamentoInicialService;
			_processamento.processamentoProvaService = _processamentoProvaService;
			return _processamento;
		}

		private bool AtualizarDocumentos()
		{
			if (_processamento.Adicionar(_provaId))
			{
				return _processamentoProvaService.DeletarInativos();
			}
			return false;
		}
	}
}
