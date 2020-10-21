using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository
{
    public class RepositoryCache : IRepositoryCache
    {
        private readonly IConnectionMultiplexerSME connectionMultiplexerSME;

        public RepositoryCache(IConnectionMultiplexerSME connectionMultiplexerSME)
        {
            this.connectionMultiplexerSME = connectionMultiplexerSME ?? throw new ArgumentNullException(nameof(connectionMultiplexerSME));
        }

        public string CriarChaveDeCache(params object[] chaves) => string.Join(":", chaves.Where(x => !string.IsNullOrWhiteSpace(x?.ToString())));

        public string Obter(string nomeChave, bool utilizarGZip = false)
        {
            var inicioOperacao = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var cacheParaRetorno = connectionMultiplexerSME.GetDatabase()?.StringGet(nomeChave);
                timer.Stop();

                if (utilizarGZip)
                {
                    cacheParaRetorno = UtilGZip.Descomprimir(Convert.FromBase64String(cacheParaRetorno));
                }

                return cacheParaRetorno;
            }
            catch (Exception ex)
            {
                timer.Stop();

                LogFacade.LogFacade.SaveError(ex, $"Redis Chave: {nomeChave}, Inicio: {inicioOperacao} e Fim: {timer.Elapsed}");
                return null;
            }
        }

        public T Obter<T>(string nomeChave, bool utilizarGZip = false)
        {
            try
            {
                var stringCache = connectionMultiplexerSME.GetDatabase()?.StringGet(nomeChave).ToString();
                if (!string.IsNullOrWhiteSpace(stringCache))
                {
                    if (utilizarGZip)
                    {
                        stringCache = UtilGZip.Descomprimir(Convert.FromBase64String(stringCache));
                    }
                    return JsonConvert.DeserializeObject<T>(stringCache);
                }
            }
            catch (Exception ex)
            {
                //Caso o cache esteja indisponível a aplicação precisa continuar funcionando mesmo sem o cache
                LogFacade.LogFacade.SaveError(ex);
            }
            return default(T);
        }

        public T Obter<T>(string nomeChave, Func<T> buscarDados, int minutosParaExpirar = 720, bool utilizarGZip = false)
        {
            try
            {
                var stringCache = connectionMultiplexerSME.GetDatabase()?.StringGet(nomeChave).ToString();
                if (!string.IsNullOrWhiteSpace(stringCache))
                {
                    if (utilizarGZip)
                    {
                        stringCache = UtilGZip.Descomprimir(Convert.FromBase64String(stringCache));
                    }
                    return JsonConvert.DeserializeObject<T>(stringCache);
                }

                var dados = buscarDados();

                Salvar(nomeChave, JsonConvert.SerializeObject(dados), minutosParaExpirar, utilizarGZip);

                return dados;
            }
            catch (Exception ex)
            {
                //Caso o cache esteja indisponível a aplicação precisa continuar funcionando mesmo sem o cache
                LogFacade.LogFacade.SaveError(ex);
                return buscarDados();
            }
        }

        public async Task<T> Obter<T>(string nomeChave, Func<Task<T>> buscarDados, int minutosParaExpirar = 720, bool utilizarGZip = false)
        {
            try
            {
                var stringCache = connectionMultiplexerSME.GetDatabase()?.StringGet(nomeChave).ToString();
                if (!string.IsNullOrWhiteSpace(stringCache))
                {
                    if (utilizarGZip)
                    {
                        stringCache = UtilGZip.Descomprimir(Convert.FromBase64String(stringCache));
                    }
                    return JsonConvert.DeserializeObject<T>(stringCache);
                }

                var dados = await buscarDados();

                await SalvarAsync(nomeChave, JsonConvert.SerializeObject(dados), minutosParaExpirar, utilizarGZip);

                return dados;
            }
            catch (Exception ex)
            {
                //Caso o cache esteja indisponível a aplicação precisa continuar funcionando mesmo sem o cache
                LogFacade.LogFacade.SaveError(ex);
                return await buscarDados();
            }
        }

        public async Task<T> ObterAsync<T>(string nomeChave, Func<Task<T>> buscarDados, int minutosParaExpirar = 720, bool utilizarGZip = false)
        {
            var inicioOperacao = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var dbCache = connectionMultiplexerSME.GetDatabase();
                var stringCache = dbCache != null ? await connectionMultiplexerSME.GetDatabase()?.StringGetAsync(nomeChave) : RedisValue.Null;

                timer.Stop();

                if (!stringCache.IsNullOrEmpty)
                {
                    if (utilizarGZip)
                    {
                        stringCache = UtilGZip.Descomprimir(Convert.FromBase64String(stringCache));
                    }
                    return JsonConvert.DeserializeObject<T>(stringCache);
                }

                var dados = await buscarDados();

                await SalvarAsync(nomeChave, dados, minutosParaExpirar, utilizarGZip);

                return dados;
            }
            catch (Exception ex)
            {
                //Caso o cache esteja indisponível a aplicação precisa continuar funcionando mesmo sem o cache
                timer.Stop();

                LogFacade.LogFacade.SaveError(ex, $"Redis Chave: {nomeChave}, Obtendo Async - Inicio: {inicioOperacao} e Fim: {timer.Elapsed}");
                return await buscarDados();
            }
        }

        public async Task<string> ObterAsync(string nomeChave, bool utilizarGZip = false)
        {
            var inicioOperacao = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var dbCache = connectionMultiplexerSME.GetDatabase();
                var cacheParaRetorno = dbCache != null ? await dbCache.StringGetAsync(nomeChave) : RedisValue.Null;
                timer.Stop();

                if (!cacheParaRetorno.IsNullOrEmpty && utilizarGZip)
                    cacheParaRetorno = UtilGZip.Descomprimir(Convert.FromBase64String(cacheParaRetorno));

                return cacheParaRetorno;
            }
            catch (Exception ex)
            {
                //Caso o cache esteja indisponível a aplicação precisa continuar funcionando mesmo sem o cache
                timer.Stop();
                LogFacade.LogFacade.SaveError(ex, $"Redis Chave: {nomeChave}, Obtendo Async - Inicio: {inicioOperacao} e Fim: {timer.Elapsed}");
                return null;
            }
        }

        public async Task RemoverAsync(string nomeChave)
        {
            var inicioOperacao = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var dbCache = connectionMultiplexerSME.GetDatabase();

                if (dbCache == null)
                    return;

                await dbCache.KeyDeleteAsync(nomeChave);
                timer.Stop();
            }
            catch (Exception ex)
            {
                //Caso o cache esteja indisponível a aplicação precisa continuar funcionando mesmo sem o cache
                timer.Stop();
                LogFacade.LogFacade.SaveError(ex, $"Redis Chave: {nomeChave}, Remover Async - Inicio: {inicioOperacao} e Fim: {timer.Elapsed}");
            }
        }

        public void Salvar(string nomeChave, string valor, int minutosParaExpirar = 720, bool utilizarGZip = false)
        {
            try
            {
                var dbCache = connectionMultiplexerSME.GetDatabase();

                if (dbCache == null)
                    return;

                if (utilizarGZip)
                {
                    var valorComprimido = UtilGZip.Comprimir(valor);
                    valor = Convert.ToBase64String(valorComprimido);
                }

                dbCache.StringSet(nomeChave, valor, TimeSpan.FromMinutes(minutosParaExpirar));
            }
            catch (Exception ex)
            {
                //Caso o cache esteja indisponível a aplicação precisa continuar funcionando mesmo sem o cache
                LogFacade.LogFacade.SaveError(ex);
            }
        }

        public async Task SalvarAsync(string nomeChave, string valor, int minutosParaExpirar = 720, bool utilizarGZip = false)
        {
            var inicioOperacao = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var dbCache = connectionMultiplexerSME.GetDatabase();

                if (dbCache == null)
                    return;

                if (!string.IsNullOrWhiteSpace(valor) && valor != "[]")
                {
                    if (utilizarGZip)
                    {
                        var valorComprimido = UtilGZip.Comprimir(valor);
                        valor = Convert.ToBase64String(valorComprimido);
                    }

                    await dbCache.StringSetAsync(nomeChave, valor, TimeSpan.FromMinutes(minutosParaExpirar));

                    timer.Stop();
                }
            }
            catch (Exception ex)
            {
                //Caso o cache esteja indisponível a aplicação precisa continuar funcionando mesmo sem o cache
                timer.Stop();
                LogFacade.LogFacade.SaveError(ex, $"Redis Chave: {nomeChave}, Salvar Async - Inicio: {inicioOperacao} e Fim: {timer.Elapsed}");
            }
        }

        public async Task SalvarAsync(string nomeChave, object valor, int minutosParaExpirar = 720, bool utilizarGZip = false)
        {
            await SalvarAsync(nomeChave, JsonConvert.SerializeObject(valor), minutosParaExpirar, utilizarGZip);
        }
    }
}
