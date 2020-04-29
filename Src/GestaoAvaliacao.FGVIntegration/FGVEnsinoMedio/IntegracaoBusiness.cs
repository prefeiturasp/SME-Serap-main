using GestaoAvaliacao.FGVIntegration.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.FGVIntegration.FGVEnsinoMedio
{
    public class IntegracaoBusiness : IIntegracaoBusiness
    {

        private readonly IDataRepository dataRepository;
        private readonly IFGVAPIClient fgvApiClient;

        public IntegracaoBusiness(IDataRepository dataRepository, IFGVAPIClient fgvApiClient)
        {
            this.dataRepository = dataRepository;
            this.fgvApiClient = fgvApiClient;
        }

        public async void RealizarIntegracaoCompleta()
        {
            var taskEscolas = IntegrarEscolas();
            await Task.WhenAll(taskEscolas);

            var taskCoordenadores = IntegrarCoordenadores();
            var taskTurmas = IntegrarTurmas();
            var taskProfessores = IntegrarProfessores();
            //pode executar estas 3 integrações ao mesmo tempo que uma não depende da outra
            await Task.WhenAll(taskCoordenadores, taskTurmas, taskProfessores);

            var taskProfessoresTurmas = IntegrarProfessoresTurmas();
            var taskAlunos = IntegrarAlunos();
            await Task.WhenAll(taskProfessoresTurmas, taskAlunos);
        }

        public async Task<ICollection<ResultadoFGV>> IntegrarEscolas()
        {
            var colecaoEnviar = dataRepository.BuscarEscolas();
            var result = await ProcessItensUsingTaskPoolAsync<Escola, ResultadoFGV>(colecaoEnviar, FGVAPIClient.ENDPOINT_ESCOLA_REGISTRAR, fgvApiClient.SendPost, 1, CancellationToken.None);
            return result;
        }

        public async Task<ICollection<ResultadoFGV>> IntegrarCoordenadores()
        {
            var colecaoEnviar = dataRepository.BuscarCoordenadores();
            var result = await ProcessItensUsingTaskPoolAsync<Coordenador, ResultadoFGV>(colecaoEnviar, FGVAPIClient.ENDPOINT_COORDENADOR_REGISTRAR, fgvApiClient.SendPost, 1, CancellationToken.None);
            return result;
        }

        public async Task<ICollection<ResultadoFGV>> IntegrarTurmas()
        {
            var colecaoEnviar = dataRepository.BuscarTurmas();
            var result = await ProcessItensUsingTaskPoolAsync<Turma, ResultadoFGV>(colecaoEnviar, FGVAPIClient.ENDPOINT_TURMA_REGISTRAR, fgvApiClient.SendPost, 1, CancellationToken.None);
            return result;
        }

        public async Task<ICollection<ResultadoFGV>> IntegrarProfessores()
        {
            var colecaoEnviar = dataRepository.BuscarProfessores();
            var result = await ProcessItensUsingTaskPoolAsync<Professor, ResultadoFGV>(colecaoEnviar, FGVAPIClient.ENDPOINT_PROFESSOR_REGISTRAR, fgvApiClient.SendPost, 1, CancellationToken.None);
            return result;
        }

        public async Task<ICollection<ResultadoFGV>> IntegrarProfessoresTurmas()
        {
            var colecaoEnviar = dataRepository.BuscarProfessoresTurmas();
            var result = await ProcessItensUsingTaskPoolAsync<ProfessorTurma, ResultadoFGV>(colecaoEnviar, FGVAPIClient.ENDPOINT_PROFESSOR_INSERIRNATURMA, fgvApiClient.SendPost, 1, CancellationToken.None);
            return result;
        }

        public async Task<ICollection<ResultadoFGV>> IntegrarAlunos()
        {
            var colecaoEnviar = dataRepository.BuscarAlunos();
            var result = await ProcessItensUsingTaskPoolAsync<Aluno, ResultadoFGV>(colecaoEnviar, FGVAPIClient.ENDPOINT_ALUNO_REGISTRAR, fgvApiClient.SendPost, 1, CancellationToken.None);
            return result;
        }

        private static async Task<ICollection<T2>> ProcessItensUsingTaskPoolAsync<T, T2>(ICollection<T> list, string endpoint, Func<string, T, Task<T2>> function,
            int parallelTasksLimit, CancellationToken cancellationToken)
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
            tasksProcess.Add(ExecutarAcao<T, T2>(queue, endpoint, function));

            while (tasksProcess.Any())
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                if (queue.Any() && tasksProcess.Count < parallelTasksLimit)
                {
                    tasksProcess.Add(ExecutarAcao<T, T2>(queue, endpoint, function));
                    continue;
                }

                var taskCompleted = await Task.WhenAny(tasksProcess);
                results.Add(taskCompleted.Result);
                tasksProcess.Remove(taskCompleted);
            }

            return results;
        }

        private static async Task<T2> ExecutarAcao<T, T2>(ConcurrentQueue<T> queue, string endpoint, Func<string, T, Task<T2>> function)
        {
            if (!queue.TryDequeue(out T obj))
                return default;

            return await function(endpoint, obj);
        }

    }
}
