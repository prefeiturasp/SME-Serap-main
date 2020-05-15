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
            return await RealizarIntegracaoEnsinoMedio(null);
        }

        public async Task<bool> RealizarIntegracaoEnsinoMedio(ICollection<string> pCodigoEscolas)
        {
            pCodigoEscolas = pCodigoEscolas ?? new string[0];
            Logger.InfoFormat("Realizando o processo de integração do Ensino Médio com filtro de escolas: [{0}]", string.Join(", ", pCodigoEscolas));

            var escolas = await IntegrarEscolas(pCodigoEscolas);
            Logger.Info("Finalizada integração de escolas");

            var taskTurmas = IntegrarTurmas(escolas);
            var taskProfessores = IntegrarProfessores(escolas);
            //pode executar estas 2 integrações ao mesmo tempo que uma não depende da outra
            await Task.WhenAll(taskTurmas, taskProfessores);
            Logger.Info("Finalizada integração de turmas e professores");

            //coordenadores são incluídos no final para não gerar conflito com professores
            var taskCoordenadores = IntegrarCoordenadores(escolas);
            var taskProfessoresTurmas = IntegrarProfessoresTurmas(escolas);
            var taskAlunos = IntegrarAlunos(escolas);
            await Task.WhenAll(taskCoordenadores, taskProfessoresTurmas, taskAlunos);
            Logger.Info("Finalizada integração de professores-turmas e alunos");

            return true;
        }

        private async Task<ICollection<Escola>> IntegrarEscolas(ICollection<string> pCodigoEscolas)
        {
            var colecaoEscolas = dataRepository.BuscarEscolas(pCodigoEscolas);
            colecaoEscolas = await eolRepository.BuscarDiretoresEscolas(colecaoEscolas);
            foreach (var escola in colecaoEscolas)
            {
                var dadosPessoa = dataRepository.BuscarPessoa(escola.RfDoResponsavel);
                escola.CPFDoResponsavel = dadosPessoa.CPF;
                escola.Email = dadosPessoa.Email;
            }

            Logger.Info($"Integrando {colecaoEscolas.Count()} escolas");
            var result = await ProcessItensUsingTaskPoolAsync(colecaoEscolas, FGVAPIConsumer.ENDPOINT_ESCOLA_REGISTRAR, fgvApiClient.SendPost, threadsEnvio, CancellationToken.None);
            return colecaoEscolas;
        }

        private async Task<ICollection<ResultadoFGV>> IntegrarCoordenadores(ICollection<Escola> pEscolas)
        {
            //var colecaoEnviar = dataRepository.BuscarCoordenadores(pEscolas);
            var colecaoCoordenadores = await eolRepository.BuscarCoordenadoresEscolas(pEscolas);
            foreach (var coordenador in colecaoCoordenadores)
            {
                var dadosPessoa = dataRepository.BuscarPessoa(coordenador.RfDoCoordenador);
                coordenador.CPF = dadosPessoa.CPF;
                coordenador.EmailDoCoordenador = dadosPessoa.Email;
                coordenador.DataNascimento = dadosPessoa.DataNascimento;
                coordenador.Sexo = dadosPessoa.Sexo;
            }
            SepararNomeSobrenome(colecaoCoordenadores);

            CarregarEmailEscola(pEscolas, colecaoCoordenadores);

            Logger.Info($"Integrando {colecaoCoordenadores.Count()} coordenadores");
            var result = await ProcessItensUsingTaskPoolAsync(colecaoCoordenadores, FGVAPIConsumer.ENDPOINT_COORDENADOR_REGISTRAR, fgvApiClient.SendPost, threadsEnvio, CancellationToken.None);
            return result;
        }

        private async Task<ICollection<ResultadoFGV>> IntegrarTurmas(ICollection<Escola> pEscolas)
        {
            var colecaoEnviar = dataRepository.BuscarTurmas(pEscolas);
            CarregarEmailEscola(pEscolas, colecaoEnviar);

            Logger.Info($"Integrando {colecaoEnviar.Count()} turmas");
            var result = await ProcessItensUsingTaskPoolAsync(colecaoEnviar, FGVAPIConsumer.ENDPOINT_TURMA_REGISTRAR, fgvApiClient.SendPost, threadsEnvio, CancellationToken.None);
            return result;
        }

        private async Task<ICollection<ResultadoFGV>> IntegrarProfessores(ICollection<Escola> pEscolas)
        {
            var colecaoEnviar = dataRepository.BuscarProfessores(pEscolas);
            CarregarEmailEscola(pEscolas, colecaoEnviar);
            SepararNomeSobrenome(colecaoEnviar);

            Logger.Info($"Integrando {colecaoEnviar.Count()} professores");
            var result = await ProcessItensUsingTaskPoolAsync(colecaoEnviar, FGVAPIConsumer.ENDPOINT_PROFESSOR_REGISTRAR, fgvApiClient.SendPost, threadsEnvio, CancellationToken.None);
            return result;
        }

        private async Task<ICollection<ResultadoFGV>> IntegrarProfessoresTurmas(ICollection<Escola> pEscolas)
        {
            var colecaoEnviar = dataRepository.BuscarProfessoresTurmas(pEscolas);
            Logger.Info($"Integrando {colecaoEnviar.Count()} professores-turmas");
            var result = await ProcessItensUsingTaskPoolAsync(colecaoEnviar, FGVAPIConsumer.ENDPOINT_PROFESSOR_INSERIRNATURMA, fgvApiClient.SendPost, threadsEnvio, CancellationToken.None);
            return result;
        }

        private async Task<ICollection<ResultadoFGV>> IntegrarAlunos(ICollection<Escola> pEscolas)
        {
            var colecaoEnviar = dataRepository.BuscarAlunos(pEscolas);
            CarregarEmailEscola(pEscolas, colecaoEnviar);
            SepararNomeSobrenome(colecaoEnviar);

            Logger.Info($"Integrando {colecaoEnviar.Count()} alunos");
            var result = await ProcessItensUsingTaskPoolAsync(colecaoEnviar, FGVAPIConsumer.ENDPOINT_ALUNO_REGISTRAR, fgvApiClient.SendPost, threadsEnvio, CancellationToken.None);
            return result;
        }

        /// <summary>
        /// O e-mail da escola é considerado o identificador
        /// </summary>
        private void CarregarEmailEscola<T>(ICollection<Escola> pEscolas, ICollection<T> pColecaoCarregar) where T : IIdentificadorEscola
        {
            var dicionarioEscolas = pEscolas.ToDictionary(e => e.CodigoDaEscola);

            foreach (var item in pColecaoCarregar)
            {
                item.EmailDaEscola = dicionarioEscolas[item.CodigoDaEscola].Email;
            }
        }

        /// <summary>
        /// Considera o último nome como sobrenome
        /// </summary>
        private void SepararNomeSobrenome<T>(ICollection<T> pColecao) where T : IPessoaNomeSobrenome
        {
            foreach (var pessoa in pColecao)
            {
                int indiceSobrenome = pessoa.Nome.LastIndexOf(' ');
                pessoa.Sobrenome = pessoa.Nome.Substring(indiceSobrenome + 1);
                pessoa.Nome = pessoa.Nome.Substring(0, indiceSobrenome);
            }
        }

        private async Task<ICollection<T2>> ProcessItensUsingTaskPoolAsync<T, T2>(ICollection<T> list, string endpoint, Func<string, T, Task<T2>> function,
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

            var queue = new ConcurrentQueue<T>(list);

            var tasksProcess = new List<Task<T2>>(parallelTasksLimit);
            tasksProcess.Add(ExecutarAcao(queue, endpoint, function));

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
                    results.Add(result);
                tasksProcess.Remove(taskCompleted);
            }

            return results;
        }

        private async Task<T2> ExecutarAcao<T, T2>(ConcurrentQueue<T> queue, string endpoint, Func<string, T, Task<T2>> function) where T : BaseFGVObject
        {
            if (!queue.TryDequeue(out T obj))
                return default;

            //Insere o indicador sequencial para possivelmente facilitar a identificação de problemas
            obj.Seq = Interlocked.Increment(ref seq);
            try
            {
                return await function(endpoint, obj);
            }
            catch (ApplicationException)
            {
                //Logger.Error("Erro de aplicação. O processo irá continuar com outros registros.", ex);
                return default;
            }
        }

    }
}