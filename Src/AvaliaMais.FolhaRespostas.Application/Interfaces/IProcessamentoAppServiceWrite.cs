using AvaliaMais.FolhaRespostas.Application.TemplateMethod;

namespace AvaliaMais.FolhaRespostas.Application.Interfaces
{
    public interface IProcessamentoAppServiceWrite
	{
		bool AdicionarProcessamento(int provaId, AbstractProcessamento tipoProcessamento);
	}
}
