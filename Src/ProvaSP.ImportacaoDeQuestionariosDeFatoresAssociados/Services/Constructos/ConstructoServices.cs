using ImportacaoDeQuestionariosSME.Data.Repositories.CiclosAnoEscolar;
using ImportacaoDeQuestionariosSME.Data.Repositories.Constructos;
using ImportacaoDeQuestionariosSME.Domain.CiclosAnoEscolar;
using ImportacaoDeQuestionariosSME.Domain.Constructos;
using ImportacaoDeQuestionariosSME.Services.Constructos.Dtos;
using ImportacaoDeQuestionariosSME.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Services.Constructos
{
    public class ConstructoServices : IConstructoServices
    {
        private readonly ICicloAnoEscolarRepository _cicloAnoEscolarRepository;
        private readonly IConstructoRepository _constructoRepository;

        public ConstructoServices()
        {
            _cicloAnoEscolarRepository = new CicloAnoEscolarRepository();
            _constructoRepository = new ConstructoRepository();
        }

        public async Task ImportarAsync(ImportacaoDeConstructosDto dto)
        {
            if (dto is null)
            {
                dto = new ImportacaoDeConstructosDto();
                dto.AddErro("O DTO é nulo.");
                return;
            }

            try
            {
                var entities = new List<Constructo>();

                var maxConstructoId = await _constructoRepository.GetMaxConstructoId();
                var ciclos = await _cicloAnoEscolarRepository.GetAsync();
                var anosEscolares = GetAnosEscolares();
                var constructorsResumidos = GetConstructosResumidos();
                foreach (var constructoReumido in constructorsResumidos)
                {
                    var constructos = anosEscolares
                        .Select(anoEscolar => new Constructo
                        {
                            AnoEscolar = anoEscolar,
                            CicloId = ciclos.FirstOrDefault(x => x.AnoEscolar == anoEscolar)?.CicloId ?? default,
                            ConstructoId = ++maxConstructoId,
                            Edicao = dto.Edicao,
                            FatorAssociadoQuestionarioId = constructoReumido.FatorAssociadoQuestionarioId,
                            Nome = constructoReumido.Nome,
                            Referencia = constructoReumido.Referencia
                        })
                        .ToList();

                    entities.AddRange(constructos);
                }

                await _constructoRepository.InsertAsync(entities);
            }
            catch (Exception ex)
            {
                dto.AddErro(ex.InnerException?.Message ?? ex.Message);
            }
        }

        private static IEnumerable<int> GetAnosEscolares()
            => new List<int>
            {
                3, 4, 5, 6, 7, 8, 9
            };

        private static IEnumerable<ConstructoResumido> GetConstructosResumidos()
            => new List<ConstructoResumido>
            {
                new ConstructoResumido("Perfil", 10),
                new ConstructoResumido("Nível socioeconômico", 10),
                new ConstructoResumido("Nível educational", 10),
                new ConstructoResumido("Organização da rotina de estudos", 10),
                new ConstructoResumido("Perfil", 11),
                new ConstructoResumido("Nível socioeconômico", 11),
                new ConstructoResumido("Relação dos responsáveis com a escola e com o acompanhamento da rotina escolar", 11),
                new ConstructoResumido("Acompanhamento da rotina de estudo da(o) estudante", 11)
            };
    }

    public class ConstructoResumido
    {
        public ConstructoResumido(string nome, int fatorAssociadoQuestionarioId, string referencia = null)
        {
            Nome = nome;
            FatorAssociadoQuestionarioId = fatorAssociadoQuestionarioId;
            Referencia = referencia;
        }

        public string Nome { get; set; }
        public int FatorAssociadoQuestionarioId { get; set; }
        public string Referencia { get; set; }
    }
}