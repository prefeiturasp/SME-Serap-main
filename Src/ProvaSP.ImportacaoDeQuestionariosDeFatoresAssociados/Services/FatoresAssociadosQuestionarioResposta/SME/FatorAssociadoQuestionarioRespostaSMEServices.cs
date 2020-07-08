using ImportacaoDeQuestionariosSME.Data.Repositories.CiclosAnoEscolar;
using ImportacaoDeQuestionariosSME.Data.Repositories.Constructos;
using ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociadosQuestionarioResposta.SME;
using ImportacaoDeQuestionariosSME.Data.Repositories.Questionarios.Itens;
using ImportacaoDeQuestionariosSME.Domain.CiclosAnoEscolar;
using ImportacaoDeQuestionariosSME.Domain.Constructos;
using ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.SME;
using ImportacaoDeQuestionariosSME.Domain.Questionarios.Itens;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.DRE.Dtos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Dtos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.SME.Dtos;
using ImportacaoDeQuestionariosSME.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.SME
{
    public class FatorAssociadoQuestionarioRespostaSMEServices : FatorAssociadoQuestionarioRespostaServices, IFatorAssociadoQuestionarioRespostaServices
    {
        private readonly ICicloAnoEscolarRepository _cicloAnoEscolarRepository;
        private readonly IConstructoRepository _constructoRepository;
        private readonly IFatorAssociadoQuestionarioRespostaSMEConstructoRepository _fatorAssociadoQuestionarioRespostaSMEConstructoRepository;
        private readonly IQuestionarioItemRepository _questionarioItemRepository;
        private readonly IFatorAssociadoQuestionarioRespostaSMERepository _fatorAssociadoQuestionarioRespostaSMERepository;

        public FatorAssociadoQuestionarioRespostaSMEServices(IEnumerable<QuestaoConstructoDto> questaoConstructoDtos)
            :base(questaoConstructoDtos)
        {
            _cicloAnoEscolarRepository = new CicloAnoEscolarRepository();
            _constructoRepository = new ConstructoRepository();
            _fatorAssociadoQuestionarioRespostaSMEConstructoRepository = new FatorAssociadoQuestionarioRespostaSMEConstructoRepository();
            _questionarioItemRepository = new QuestionarioItemRepository();
            _fatorAssociadoQuestionarioRespostaSMERepository = new FatorAssociadoQuestionarioRespostaSMERepository();
        }

        public async Task ImportarAsync(ImportacaoDeQuestionariosDeFatoresAssociadosDto dto)
        {
            if (dto is null)
            {
                dto = new ImportacaoDeQuestionariosDeFatoresAssociadosDto();
                dto.AddErro("O DTO é nulo.");
                return;
            }

            if (_questaoConstructoDtos is null || !_questaoConstructoDtos.Any())
            {
                dto.AddErro("Importe a tabela de fatores associados antes.");
                return;
            }

            try
            {
                var dtQuestionario = CsvManager.GetCsvFile(dto.CaminhoDaPlanilha);
                if (dtQuestionario.Rows.Count <= 0)
                {
                    dto.AddErro("Não existem regitros na planilha para exportação.");
                    return;
                }

                var constructos = await _constructoRepository.GetAsync(dto.Edicao);
                var ciclosAnoEscolar = await _cicloAnoEscolarRepository.GetAsync();
                var questionarioItens = await _questionarioItemRepository.GetAsync(dto.QuestionarioId);
                var nextPK = await _fatorAssociadoQuestionarioRespostaSMERepository.GetNextPk();

                var respostasSME = dtQuestionario
                    .AsEnumerable()
                    .Where(row => !string.IsNullOrWhiteSpace(row["Resposta"].ToString()))
                    .Select(row => new FatorAssociadoQuestionarioRespostaSMEDto
                    {
                        AnoEscolar = int.Parse(row["AnoEscolar"].ToString()),
                        Quantidade = decimal.Parse(row["Quantidade"].ToString()),
                        Questao = int.Parse(row["Questao"].ToString()),
                        Resposta = row["Resposta"].ToString()
                    })
                    .ToList();

                var respostasSMEAgrupadas = respostasSME
                    .GroupBy(x => new { x.AnoEscolar, x.Questao })
                    .Select(x => new
                    {
                        x.Key.AnoEscolar,
                        x.Key.Questao,
                        Quantidade = x.Sum(y => y.Quantidade)
                    })
                    .ToList();

                var entities = new List<FatorAssociadoQuestionarioRespostaSME>();
                var entitiesConstructos = new List<FatorAssociadoQuestionarioRespostaSMEConstructo>();

                respostasSME
                    .ForEach(x =>
                    {
                        var questionarioItem = questionarioItens.FirstOrDefault(y => y.Numero == x.Questao);
                        var cicloId = ciclosAnoEscolar.First(z => z.AnoEscolar == x.AnoEscolar).CicloId;
                        var valor = GetValorPercentual(x.Quantidade,
                                    respostasSMEAgrupadas.First(z => z.AnoEscolar == x.AnoEscolar && z.Questao == x.Questao).Quantidade);
                        var constructosRelacionados = GetConstructosRelacionados(dto.Edicao, cicloId, x.AnoEscolar, x.Questao, constructos);

                        var entity = new FatorAssociadoQuestionarioRespostaSME
                        {
                            Id = nextPK++,
                            AnoEscolar = x.AnoEscolar,
                            CicloId = cicloId,
                            Edicao = dto.Edicao,
                            FatorAssociadoQuestionarioId = FatorAssociadoQuestionarioIdEstudante,
                            ItemDescricao = GetItemDescricao(questionarioItem, x),
                            ItemId = GetItemId(questionarioItem, x),
                            Valor = valor,
                            VariavelDescricao = $"{x.Questao} - {questionarioItem.Enunciado}",
                            VariavelId = x.Questao.ToString()
                        };

                        entities.Add(entity);

                        if (constructosRelacionados?.Any() ?? false)
                        {
                            var SMEConstructos = constructosRelacionados
                                .Select(y => new FatorAssociadoQuestionarioRespostaSMEConstructo
                                {
                                    ConstructoId = y.ConstructoId,
                                    FatorAssociadoQuestionarioRespostaSMEId = entity.Id
                                })
                                .ToList();

                            entitiesConstructos.AddRange(SMEConstructos);
                        }
                    });

                await _fatorAssociadoQuestionarioRespostaSMERepository.InsertAsync(entities);
                await _fatorAssociadoQuestionarioRespostaSMEConstructoRepository.InsertAsync(entitiesConstructos);
            }
            catch (Exception ex)
            {
                dto.AddErro(ex.InnerException?.Message ?? ex.Message);
            }
        }

        private string GetItemDescricao(QuestionarioItem questionarioItem, FatorAssociadoQuestionarioRespostaSMEDto dreDto)
        {
            if (string.IsNullOrWhiteSpace(dreDto.Resposta)) return "Não respondido.";
            return questionarioItem.Opcoes.First(z => z.Letra == dreDto.Resposta).Descricao;
        }

        private int GetItemId(QuestionarioItem questionarioItem, FatorAssociadoQuestionarioRespostaSMEDto dreDto)
        {
            if (string.IsNullOrWhiteSpace(dreDto.Resposta)) return questionarioItem.Opcoes.Count() + 1;
            return questionarioItem.Opcoes.First(z => z.Letra == dreDto.Resposta).Numero;
        }
    }
}