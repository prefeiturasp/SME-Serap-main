using GestaoAvaliacao.Entities;

namespace GestaoAvaliacao.IRepository
{
    public interface IDadosArquivoResultadoPspRepository
    {
        DadosArquivoResultadoPsp InserirNovo(DadosArquivoResultadoPsp dadosArquivoResultadoPsp);
    }
}
