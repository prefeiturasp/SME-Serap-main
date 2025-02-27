using ImportacaoDeQuestionariosSME.Data.Repositories.CiclosAnoEscolar;
using ImportacaoDeQuestionariosSME.Data.Repositories.Constructos;
using ImportacaoDeQuestionariosSME.Domain.CiclosAnoEscolar;
using ImportacaoDeQuestionariosSME.Domain.Constructos;
using ImportacaoDeQuestionariosSME.Services.Constructos.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
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
                new ConstructoResumido("Sexo [feminino]", 5, "masculino"),
                new ConstructoResumido("Cor/Origem [pardo]", 5, "branco/amarelo"),
                new ConstructoResumido("Cor/Origem [preto/indígena]", 5, "branco/amarelo"),
                new ConstructoResumido("Cor/Origem [não sabe/não resp.]", 5, "branco/amarelo"),
                new ConstructoResumido("NSE [médio-baixo]", 5, "baixo/muito baixo"),
                new ConstructoResumido("NSE [médio]", 5, "baixo/muito baixo"),
                new ConstructoResumido("NSE [médio-alto]", 5, "baixo/muito baixo"),
                new ConstructoResumido("Trabalha fora ou doméstico > 3h [sim]", 5, "não"),
                new ConstructoResumido("Perdeu algum ano (defasado) [sim]", 5, "não"),
                new ConstructoResumido("Faz o dever de casa [de vez em quando]", 5, "nunca"),
                new ConstructoResumido("Faz o dever de casa [sempre]", 5, "nunca"),
                new ConstructoResumido("Bagunça na aula atrapalha [sim]", 5, "não"),
                new ConstructoResumido("Nível de Bullying [médio]", 5, "baixo"),
                new ConstructoResumido("Nível de Bullying [alto]", 5, "baixo"),
                new ConstructoResumido("Nível do comprometimento dos pais [regular]", 5, "ruim"),
                new ConstructoResumido("Nível do comprometimento dos pais [bom/ótimo]", 5, "ruim"),
                new ConstructoResumido("Nível de satisfação com a escola [regular]", 5, "ruim"),
                new ConstructoResumido("Nível de satisfação com a escola [bom]", 5, "ruim"),
                new ConstructoResumido("Nível do relacionamento escolar [ótimo]", 5, "ruim"),
                new ConstructoResumido("Nível do relacionamento escolar [reg./bom]", 5, "ruim"),
                new ConstructoResumido("Professor de preocupa com dever de casa [sim]", 5, "não")
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