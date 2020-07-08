using ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociadosQuestionario.DRE;
using ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.DRE;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Dtos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.DRE
{
    public class FatoresAssociadosQuestionarioRespostaFamiliaDREServices : FatoresAssociadosQuestionarioRespostaFamiliaServices, IFatoresAssociadosQuestionarioRespostaFamiliaServices
    {
        private ICollection<FatorAssociadoQuestionarioRespostaDRE> _entitiesDRE;
        private ICollection<FatorAssociadoQuestionarioRespostaDREConstructo> _entitiesDREConstructo;
        private readonly IFatorAssociadoQuestionarioRespostaDRERepository _fatorAssociadoQuestionarioRespostaDRERepository;
        protected IEnumerable<QuestaoConstructoDto> _questaoConstructoDtos;
        private int _nextPKDRE;

        public FatoresAssociadosQuestionarioRespostaFamiliaDREServices(DataTable dtRespostas) : base(dtRespostas)
        {
            _entitiesDRE = new List<FatorAssociadoQuestionarioRespostaDRE>();
            _fatorAssociadoQuestionarioRespostaDRERepository = new FatorAssociadoQuestionarioRespostaDRERepository();
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

                _nextPKDRE = await _fatorAssociadoQuestionarioRespostaDRERepository.GetNextPk();

                for (var numQuestao = 1; numQuestao <= 61; numQuestao++)
                {
                    var questao = questionarioItens.FirstOrDefault(x => x.Numero == numQuestao);
                    if (questao is null)
                    {
                        dto.AddErro("Questão não encontrada.");
                        continue;
                    }

                    var agrupamentosPorAnoEscolarUadEOpcao = _dtRespostas
                        .AsEnumerable()
                        .Where(row => !string.IsNullOrWhiteSpace(row[$"q{numQuestao}"].ToString()) && row[$"q{numQuestao}"].ToString() != "*")
                        .GroupBy(row => new { AnoEscolar = int.Parse(row["AnoEscolar"].ToString()), UadSigla = row["cod_DRE"].ToString(), Alternativa = row[$"q{numQuestao}"].ToString() })
                        .Select(x => new
                        {
                            x.Key.AnoEscolar,
                            x.Key.UadSigla,
                            x.Key.Alternativa,
                            Quantidade = x.Count()
                        })
                        .ToList();

                    var agrupamentosPorAnoEscolarEUad = agrupamentosPorAnoEscolarUadEOpcao
                        .GroupBy(x => new { x.AnoEscolar, x.UadSigla })
                        .Select(x => new
                        {
                            x.Key.AnoEscolar,
                            x.Key.UadSigla,
                            Quantidade = x.Sum(y => y.Quantidade)
                        })
                        .ToList();

                    foreach (var agrupamentoPorAnoEscolarEUad in agrupamentosPorAnoEscolarEUad)
                    {
                        if (agrupamentoPorAnoEscolarEUad.AnoEscolar == 0) continue;
                        var cicloId = ciclosAnoEscolar.First(x => x.AnoEscolar == agrupamentoPorAnoEscolarEUad.AnoEscolar).CicloId;

                        foreach (var opcao in questao.Opcoes)
                        {
                            if (opcao.Letra == "NR") continue;

                            var quantidadeDaOpcao = agrupamentosPorAnoEscolarUadEOpcao
                                    .FirstOrDefault(x => x.AnoEscolar == agrupamentoPorAnoEscolarEUad.AnoEscolar
                                    && x.UadSigla == agrupamentoPorAnoEscolarEUad.UadSigla
                                    && x.Alternativa == opcao.Letra)
                                    ?.Quantidade;

                            var valor = quantidadeDaOpcao is null
                                ? 0m
                                : (quantidadeDaOpcao.Value * 100m) / agrupamentoPorAnoEscolarEUad.Quantidade;

                            var entity = new FatorAssociadoQuestionarioRespostaDRE
                            {
                                AnoEscolar = agrupamentoPorAnoEscolarEUad.AnoEscolar,
                                CicloId = cicloId,
                                Edicao = dto.Edicao,
                                FatorAssociadoQuestionarioId = 6,
                                Id = _nextPKDRE++,
                                ItemDescricao = opcao.Descricao,
                                ItemId = opcao.Numero,
                                UadSigla = agrupamentoPorAnoEscolarEUad.UadSigla,
                                Valor = valor,
                                VariavelDescricao = questao.Enunciado,
                                VariavelId = questao.Numero.ToString()
                            };

                            _entitiesDRE.Add(entity);
                        }
                    }
                }

                await _fatorAssociadoQuestionarioRespostaDRERepository.InsertAsync(_entitiesDRE);
            }
            catch (Exception ex)
            {
                dto.AddErro(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
