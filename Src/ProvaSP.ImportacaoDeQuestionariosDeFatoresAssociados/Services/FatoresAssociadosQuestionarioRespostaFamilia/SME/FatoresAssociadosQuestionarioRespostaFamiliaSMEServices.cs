using ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociadosQuestionarioResposta.SME;
using ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.SME;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Dtos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.Dtos;
using ImportacaoDeQuestionariosSME.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.SME
{
    public class FatoresAssociadosQuestionarioRespostaFamiliaSMEServices : FatoresAssociadosQuestionarioRespostaFamiliaServices, IFatoresAssociadosQuestionarioRespostaFamiliaServices
    {
        private ICollection<FatorAssociadoQuestionarioRespostaSME> _entitiesSME;
        private ICollection<FatorAssociadoQuestionarioRespostaSMEConstructo> _entitiesSMEConstructo;
        private readonly IFatorAssociadoQuestionarioRespostaSMERepository _fatorAssociadoQuestionarioRespostaSMERepository;
        protected IEnumerable<QuestaoConstructoDto> _questaoConstructoDtos;
        private int _nextPKSME;

        public FatoresAssociadosQuestionarioRespostaFamiliaSMEServices(DataTable dtRespostas)
            : base(dtRespostas)
        {
            _entitiesSME = new List<FatorAssociadoQuestionarioRespostaSME>();
            _fatorAssociadoQuestionarioRespostaSMERepository = new FatorAssociadoQuestionarioRespostaSMERepository();
        }

        public async Task ImportarAsync(ImportacaoDeQuestionariosDeFatoresAssociadosFamiliaDto dto, IEnumerable<QuestaoConstructoDto> questaoConstructoDtos)
        {
            if (dto is null)
            {
                dto = new ImportacaoDeQuestionariosDeFatoresAssociadosFamiliaDto();
                dto.AddErro("O DTO é nulo.");
                return;
            }

            try
            {
                _questaoConstructoDtos = questaoConstructoDtos;

                var questionarioItens = MontarQuestoes(dto);
                if (!dto.IsValid()) return;
                if (questionarioItens is null || !questionarioItens.Any()) return;

                if (_dtRespostas.Rows.Count <= 0)
                {
                    dto.AddErro("Não existem regitros na planilha para exportação.");
                    return;
                }

                var ciclosAnoEscolar = await _cicloAnoEscolarRepository.GetAsync();
                var constructos = await _constructoRepository.GetAsync(dto.Edicao);

                _nextPKSME = await _fatorAssociadoQuestionarioRespostaSMERepository.GetNextPk();

                for (var numQuestao = 1; numQuestao <= 61; numQuestao++)
                {
                    var questao = questionarioItens.FirstOrDefault(x => x.Numero == numQuestao);
                    if (questao is null)
                    {
                        dto.AddErro("Questão não encontrada.");
                        continue;
                    }

                    var agrupamentosPorAnoEscolarEOpcao = _dtRespostas
                        .AsEnumerable()
                        .Where(row => !string.IsNullOrWhiteSpace(row[$"q{numQuestao}"].ToString()) && row[$"q{numQuestao}"].ToString() != "*")
                        .GroupBy(row => new { AnoEscolar = int.Parse(row["AnoEscolar"].ToString()), Alternativa = row[$"q{numQuestao}"].ToString() })
                        .Select(x => new
                        {
                            x.Key.AnoEscolar,
                            x.Key.Alternativa,
                            Quantidade = x.Count()
                        })
                        .ToList();

                    var agrupamentosPorAnoEscolar = agrupamentosPorAnoEscolarEOpcao
                        .GroupBy(x => x.AnoEscolar)
                        .Select(x => new
                        {
                            AnoEscolar = x.Key,
                            Quantidade = x.Sum(y => y.Quantidade)
                        })
                        .ToList();

                    foreach (var agrupamentoPorAnoEscolar in agrupamentosPorAnoEscolar)
                    {
                        if (agrupamentoPorAnoEscolar.AnoEscolar == 0) continue;
                        var cicloId = ciclosAnoEscolar.First(x => x.AnoEscolar == agrupamentoPorAnoEscolar.AnoEscolar).CicloId;

                        foreach (var opcao in questao.Opcoes)
                        {
                            if (opcao.Letra == "NR") continue;

                            var quantidadeDaOpcao = agrupamentosPorAnoEscolarEOpcao
                                    .FirstOrDefault(x => x.AnoEscolar == agrupamentoPorAnoEscolar.AnoEscolar && x.Alternativa == opcao.Letra)
                                    ?.Quantidade;

                            var valor = quantidadeDaOpcao is null
                                ? 0m
                                : (quantidadeDaOpcao.Value * 100m) / agrupamentoPorAnoEscolar.Quantidade;

                            var entity = new FatorAssociadoQuestionarioRespostaSME
                            {
                                AnoEscolar = agrupamentoPorAnoEscolar.AnoEscolar,
                                CicloId = cicloId,
                                Edicao = dto.Edicao,
                                FatorAssociadoQuestionarioId = 6,
                                Id = _nextPKSME++,
                                ItemDescricao = opcao.Descricao,
                                ItemId = opcao.Numero,
                                Valor = valor,
                                VariavelDescricao = questao.Enunciado,
                                VariavelId = questao.Numero.ToString()
                            };

                            _entitiesSME.Add(entity);
                        }
                    }
                }

                await _fatorAssociadoQuestionarioRespostaSMERepository.InsertAsync(_entitiesSME);
            }
            catch (Exception ex)
            {
                dto.AddErro(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}