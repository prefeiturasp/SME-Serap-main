using ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociadosQuestionario;
using ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionario;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionario.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionario
{
    public class FatorAssociadoQuestionarioServices : IFatorAssociadoQuestionarioServices
    {
        private readonly IFatorAssociadoQuestionarioRepository _fatorAssociadoQuestionarioRepository;

        public FatorAssociadoQuestionarioServices()
        {
            _fatorAssociadoQuestionarioRepository = new FatorAssociadoQuestionarioRepository();
        }

        public async Task ImportarAsync(ImportacaoDeFatorAssociadoQuestionarioDto dto)
        {
            if (dto is null)
            {
                dto = new ImportacaoDeFatorAssociadoQuestionarioDto();
                dto.AddErro("O DTO é nulo.");
                return;
            }

            try
            {
                var entities = new List<FatorAssociadoQuestionario>
                {
                    new FatorAssociadoQuestionario { Edicao = dto.Edicao, FatorAssociadoQuestionarioId = 5, Nome = "Estudante" },
                    new FatorAssociadoQuestionario { Edicao = dto.Edicao, FatorAssociadoQuestionarioId = 6, Nome = "Familia" },
                    new FatorAssociadoQuestionario { Edicao = dto.Edicao, FatorAssociadoQuestionarioId = 7, Nome = "Assistente de Diretor(a)" },
                    new FatorAssociadoQuestionario { Edicao = dto.Edicao, FatorAssociadoQuestionarioId = 8, Nome = "Diretor(a)" },
                    new FatorAssociadoQuestionario { Edicao = dto.Edicao, FatorAssociadoQuestionarioId = 9, Nome = "Professor(a)" }
                };

                await _fatorAssociadoQuestionarioRepository.InsertAsync(entities);
            }
            catch (Exception ex)
            {
                dto.AddErro(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}