using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface IRepositoryCache
    {
        string CriarChaveDeCache(params object[] chaves);

        string Obter(string nomeChave, bool utilizarGZip = false);

        T Obter<T>(string nomeChave, bool utilizarGZip = false);

        T Obter<T>(string nomeChave, Func<T> buscarDados, int minutosParaExpirar = 720, bool utilizarGZip = false);

        Task<T> Obter<T>(string nomeChave, Func<Task<T>> buscarDados, int minutosParaExpirar = 720, bool utilizarGZip = false);

        Task<string> ObterAsync(string nomeChave, bool utilizarGZip = false);

        Task<T> ObterAsync<T>(string nomeChave, Func<Task<T>> buscarDados, int minutosParaExpirar = 720, bool utilizarGZip = false);

        Task RemoverAsync(string nomeChave);

        void Salvar(string nomeChave, string valor, int minutosParaExpirar = 720, bool utilizarGZip = false);

        Task SalvarAsync(string nomeChave, string valor, int minutosParaExpirar = 720, bool utilizarGZip = false);

        Task SalvarAsync(string nomeChave, object valor, int minutosParaExpirar = 720, bool utilizarGZip = false);
    }
}
