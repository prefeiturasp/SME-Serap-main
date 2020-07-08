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
                            FatorAssociadoQuestionarioId = 5,
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
                new ConstructoResumido("Sexo [feminino]","masculino"),
                new ConstructoResumido("Cor/Origem [pardo]","branco/amarelo"),
                new ConstructoResumido("Cor/Origem [preto/indígena]","branco/amarelo"),
                new ConstructoResumido("Cor/Origem [não sabe/não resp.]","branco/amarelo"),
                new ConstructoResumido("NSE [médio-baixo]","baixo/muito baixo"),
                new ConstructoResumido("NSE [médio]","baixo/muito baixo"),
                new ConstructoResumido("NSE [médio-alto]","baixo/muito baixo"),
                new ConstructoResumido("Trabalha fora ou doméstico > 3h [sim]","não"),
                new ConstructoResumido("Perdeu algum ano (defasado) [sim]","não"),
                new ConstructoResumido("Faz o dever de casa [de vez em quando]","nunca"),
                new ConstructoResumido("Faz o dever de casa [sempre]","nunca"),
                new ConstructoResumido("Bagunça na aula atrapalha [sim]","não"),
                new ConstructoResumido("Nível de Bullying [médio]","baixo"),
                new ConstructoResumido("Nível de Bullying [alto]","baixo"),
                new ConstructoResumido("Nível do comprometimento dos pais [regular]","ruim"),
                new ConstructoResumido("Nível do comprometimento dos pais [bom/ótimo]","ruim"),
                new ConstructoResumido("Nível de satisfação com a escola [regular]","ruim"),
                new ConstructoResumido("Nível de satisfação com a escola [bom]","ruim"),
                new ConstructoResumido("Nível do relacionamento escolar [ótimo]", "ruim"),
                new ConstructoResumido("Nível do relacionamento escolar [reg./bom]", "ruim"),
                new ConstructoResumido("Professor de preocupa com dever de casa [sim]", "não")
            };
    }

    public class ConstructoResumido
    {
        public ConstructoResumido(string nome, string referencia)
        {
            Nome = nome;
            Referencia = referencia;
        }

        public string Nome { get; set; }
        public string Referencia { get; set; }
    }
}