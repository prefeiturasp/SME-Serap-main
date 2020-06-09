using GestaoAvaliacao.FGVIntegration.Data;
using GestaoAvaliacao.FGVIntegration.Logging;
using GestaoAvaliacao.FGVIntegration.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.FGVIntegration.Business
{
    public class IntegracaoBusiness : LoggingBase, IIntegracaoBusiness
    {

        private readonly IDatabaseRepository dataRepository;
        private readonly IEolRepository eolRepository;
        private readonly IFGVAPIConsumer fgvApiClient;
        private readonly byte threadsEnvio;
        private int seq = 0;

        public IntegracaoBusiness(IDatabaseRepository dataRepository, IEolRepository eolRepository, IFGVAPIConsumer fgvApiClient)
        {
            this.dataRepository = dataRepository;
            this.eolRepository = eolRepository;
            this.fgvApiClient = fgvApiClient;

            this.threadsEnvio = Convert.ToByte(ConfigurationHelper.BuscarConfiguracaoObrigatoria("ApiFgvEnsinoMedio_ThreadsEnvio"));
        }

        public async Task<bool> RealizarIntegracaoEnsinoMedio()
        {
            Logger.Info("Realizando o processo completo de integração do Ensino Médio");
            //string[] codigoEscolas = new string[] { /*"094609", "017442",*/"018210"/*,"094668","017272","093181","093637","016519",*/ };

            return await RealizarIntegracaoEnsinoMedio(null);
        }

        public async Task<bool> RealizarIntegracaoEnsinoMedio(ICollection<string> pCodigoEscolas)
        {
            pCodigoEscolas = pCodigoEscolas ?? new string[0];
            Logger.InfoFormat("Realizando o processo de integração do Ensino Médio com filtro de escolas: [{0}]", string.Join(", ", pCodigoEscolas));
            //await IntegrarRede();

            var escolas = await IntegrarEscolas(pCodigoEscolas);
            Logger.Info("Finalizada integração de escolas");

            //await  IntegrarTurmas(escolas);
            //await IntegrarProfessores(escolas);
            Logger.Info("Finalizada integração de turmas e professores");

            //await IntegrarCoordenadores(escolas);
            await IntegrarProfessoresTurmas(escolas);
            //var taskAlunos = IntegrarAlunos(escolas);
            Logger.Info("Finalizada integração de professores-turmas e alunos");

            return true;
        }

        private async Task<ICollection<Escola>> IntegrarEscolas(ICollection<string> pCodigoEscolas)
        {
            var colecaoEscolas = dataRepository.BuscarEscolas(pCodigoEscolas);
            colecaoEscolas = await eolRepository.BuscarDiretoresEscolas(colecaoEscolas);
            var sequencia = 0;
            foreach (var escola in colecaoEscolas)
            {
                var dadosPessoa = dataRepository.BuscarPessoa(escola.RfDoResponsavel);
                escola.CPFDoResponsavel = dadosPessoa.CPF;
                escola.Email = dadosPessoa.Email;
                escola.Seq = sequencia++;
            }

            Logger.Info($"Integrando {colecaoEscolas.Count()} escolas");
            var result = await fgvApiClient.SendPostList(FGVAPIConsumer.ENDPOINT_ESCOLA_REGISTRAR, colecaoEscolas);
            return colecaoEscolas;
        }

        private async Task<IEnumerable<ResultadoFGV>> IntegrarCoordenadores(ICollection<Escola> pEscolas)
        {
            var colecaoCoordenadores = await eolRepository.BuscarCoordenadoresEscolas(pEscolas);
            var contador = 0;
            foreach (var coordenador in colecaoCoordenadores)
            {
                var dadosPessoa = dataRepository.BuscarPessoa(coordenador.RfDoCoordenador);
                coordenador.CPF = dadosPessoa.CPF;
                coordenador.EmailDoCoordenador = dadosPessoa.Email;
                coordenador.DataNascimento = dadosPessoa.DataNascimento;
                coordenador.Sexo = dadosPessoa.Sexo;
                coordenador.Seq = contador++;
            }
            SepararNomeSobrenome(colecaoCoordenadores);

            CarregarEmailEscola(pEscolas, colecaoCoordenadores);

            Logger.Info($"Integrando {colecaoCoordenadores.Count()} coordenadores");
            var result = await ProcessItensUsingTaskPoolAsync(colecaoCoordenadores, FGVAPIConsumer.ENDPOINT_COORDENADOR_REGISTRAR, fgvApiClient.SendPostList, threadsEnvio, CancellationToken.None);
            return result;
        }

        private int GetTotalDePaginas(long quantidade, int quantidadeDeRegistrosPorPagina)
        {
            decimal quantidadeDePaginas = (decimal)quantidade / quantidadeDeRegistrosPorPagina;
            return decimal.ToInt32(Math.Ceiling(quantidadeDePaginas));
        }

        private async Task<IEnumerable<ResultadoFGV>> IntegrarTurmas(ICollection<Escola> pEscolas)
        {
            var colecaoEnviar = dataRepository.BuscarTurmas(pEscolas);
            CarregarEmailEscola(pEscolas, colecaoEnviar);

            Logger.Info($"Integrando {colecaoEnviar.Count()} turmas");

            var result = await ProcessItensUsingTaskPoolAsync(colecaoEnviar, FGVAPIConsumer.ENDPOINT_TURMA_REGISTRAR, fgvApiClient.SendPostList, threadsEnvio, CancellationToken.None);
            return result;
        }

        private async Task<IEnumerable<ResultadoFGV>> IntegrarProfessores(ICollection<Escola> pEscolas)
        {
            var colecaoEnviar = dataRepository.BuscarProfessores(pEscolas);
            CarregarEmailEscola(pEscolas, colecaoEnviar);
            SepararNomeSobrenome(colecaoEnviar);

            Logger.Info($"Integrando {colecaoEnviar.Count()} professores");
            var result = await ProcessItensUsingTaskPoolAsync(colecaoEnviar, FGVAPIConsumer.ENDPOINT_PROFESSOR_REGISTRAR, fgvApiClient.SendPostList, threadsEnvio, CancellationToken.None);
            return result;
        }

        private async Task<IEnumerable<ResultadoFGV>> IntegrarProfessoresTurmas(ICollection<Escola> pEscolas)
        {
            var colecaoEnviar = dataRepository.BuscarProfessoresTurmas(pEscolas);
            Logger.Info($"Integrando {colecaoEnviar.Count()} professores-turmas");
            var result = await ProcessItensUsingTaskPoolAsync(colecaoEnviar, FGVAPIConsumer.ENDPOINT_PROFESSOR_INSERIRNATURMA, fgvApiClient.SendPostList, threadsEnvio, CancellationToken.None);
            return result;
        }

        private async Task<IEnumerable<ResultadoFGV>> IntegrarAlunos(ICollection<Escola> pEscolas)
        {
            var colecaoEnviar = dataRepository.BuscarAlunos(pEscolas);
            CarregarEmailEscola(pEscolas, colecaoEnviar);
            SepararNomeSobrenome(colecaoEnviar);

            Logger.Info($"Integrando {colecaoEnviar.Count()} alunos");
            var result = await ProcessItensUsingTaskPoolAsync(colecaoEnviar, FGVAPIConsumer.ENDPOINT_ALUNO_REGISTRAR, fgvApiClient.SendPostList, threadsEnvio, CancellationToken.None);
            return result;
        }

        private async Task<ResultadoFGV> IntegrarRede()
        {
            var colecaoEnviar =
                new Rede
                {
                    NomeDaRede = "SECRETARIA MUNICIPAL DE EDUCAÇÃO DE SÃO PAULO",
                    NomeDoResponsavel = "Claudio Maroja",
                    CargoDoResponsavel = "Diretor de Núcleo de Avaliação",
                    CPFDoResponsavel = "13180367806",
                    UF = "SP",
                    Cidade = 3550308,
                    CNPJ = "46392114000125",
                    WebSite = "https://educacao.sme.prefeitura.sp.gov.br/",
                    Tipo = 3
                };

            var result = await fgvApiClient.SendPost(FGVAPIConsumer.ENDPOINT_REDE_REGISTRAR, colecaoEnviar);
            return result;
        }

        /// <summary>
        /// O e-mail da escola é considerado o identificador
        /// </summary>
        private void CarregarEmailEscola<T>(ICollection<Escola> pEscolas, ICollection<T> pColecaoCarregar) where T : BaseFGVObject, IIdentificadorEscola
        {
            var dicionarioEscolas = pEscolas.ToDictionary(e => e.CodigoDaEscola);
            var contador = 0;
            foreach (var item in pColecaoCarregar)
            {
                item.EmailDaEscola = dicionarioEscolas[item.CodigoDaEscola].Email;
                item.Seq = contador++;
            }
        }

        /// <summary>
        /// Considera o último nome como sobrenome
        /// </summary>
        private void SepararNomeSobrenome<T>(ICollection<T> pColecao) where T : IPessoaNomeSobrenome
        {
            foreach (var pessoa in pColecao)
            {
                int indiceSobrenome = pessoa.Nome.Trim().LastIndexOf(' ');
                pessoa.Sobrenome = pessoa.Nome.Trim().Substring(indiceSobrenome + 1);
                pessoa.Nome = pessoa.Nome.Trim().Substring(0, indiceSobrenome);
            }
        }

        private async Task<IEnumerable<T2>> ProcessItensUsingTaskPoolAsync<T, T2>(ICollection<T> list, string endpoint, Func<string, List<T>, Task<IEnumerable<T2>>> function,
            int parallelTasksLimit, CancellationToken cancellationToken)
            where T : BaseFGVObject
        {
            if (list is null)
                throw new NullReferenceException("A lista de itens não deve ser nula.");
            if (function is null)
                throw new ArgumentNullException("A rotina de processamento dos itens deve ser definida");
            if (parallelTasksLimit <= 0)
                throw new InvalidOperationException("A quantidade de tasks que serão executadas em paralelo deve ser informada.");

            var results = new List<T2>(list.Count);

            if (!list.Any())
                return results;

            var queue = new ConcurrentQueue<List<T>>();
            var quantidadeDeRepeticoes = GetTotalDePaginas(list.Count(), 50);
            for (int i = 0; i < quantidadeDeRepeticoes; i++)
            {
                queue.Enqueue(list.Skip(i * 50).Take(50).ToList());
            }


            var tasksProcess = new List<Task<IEnumerable<T2>>>(parallelTasksLimit)
            {
                ExecutarAcao(queue, endpoint, function)
            };

            while (tasksProcess.Any() || queue.Any())
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                if (queue.Any() && tasksProcess.Count < parallelTasksLimit)
                {
                    tasksProcess.Add(ExecutarAcao(queue, endpoint, function));
                    continue;
                }

                var taskCompleted = await Task.WhenAny(tasksProcess);
                var result = taskCompleted.Result;
                if (result != null)
                    results.AddRange(result);
                tasksProcess.Remove(taskCompleted);
            }

            return results;
        }

        private async Task<IEnumerable<T2>> ExecutarAcao<T, T2>(ConcurrentQueue<List<T>> queue, string endpoint, Func<string, List<T>, Task<IEnumerable<T2>>> function) where T : BaseFGVObject
        {
            if (!queue.TryDequeue(out List<T> obj))
                return default;

            //Insere o indicador sequencial para possivelmente facilitar a identificação de problemas
            //obj.Seq = Interlocked.Increment(ref seq);
            try
            {
                return await function(endpoint, obj);
            }
            catch (ApplicationException ex)
            {
                Logger.Error("Erro de aplicação. O processo irá continuar com outros registros.", ex);
                return default;
            }
        }

    }
}