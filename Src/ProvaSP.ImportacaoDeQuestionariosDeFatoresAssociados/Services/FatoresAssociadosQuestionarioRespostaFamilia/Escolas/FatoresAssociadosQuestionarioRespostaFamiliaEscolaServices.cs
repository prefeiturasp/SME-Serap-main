using ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociadosQuestionarioResposta.Escolas;
using ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.Escolas;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Dtos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.Escolas
{
    public class FatoresAssociadosQuestionarioRespostaFamiliaEscolaServices : FatoresAssociadosQuestionarioRespostaFamiliaServices, IFatoresAssociadosQuestionarioRespostaFamiliaServices
    {
        private ICollection<FatorAssociadoQuestionarioRespostaEscola> _entitiesEscola;
        private ICollection<FatorAssociadoQuestionarioRespostaEscolaConstructo> _entitiesEscolaConstructo;
        private readonly IFatorAssociadoQuestionarioRespostaEscolaRepository _fatorAssociadoQuestionarioRespostaEscolaRepository;
        protected IEnumerable<QuestaoConstructoDto> _questaoConstructoDtos;
        private int _nextPKEscola;

        public FatoresAssociadosQuestionarioRespostaFamiliaEscolaServices(DataTable dtRespostas) : base(dtRespostas)
        {
            _entitiesEscola = new List<FatorAssociadoQuestionarioRespostaEscola>();
            _fatorAssociadoQuestionarioRespostaEscolaRepository = new FatorAssociadoQuestionarioRespostaEscolaRepository();
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

                _nextPKEscola = await _fatorAssociadoQuestionarioRespostaEscolaRepository.GetNextPk();

                for (var numQuestao = 1; numQuestao <= 61; numQuestao++)
                {
                    var questao = questionarioItens.FirstOrDefault(x => x.Numero == numQuestao);
                    if (questao is null)
                    {
                        dto.AddErro("Questão não encontrada.");
                        continue;
                    }

                    var agrupamentosPorAnoEscolarUadEscolaEOpcao = _dtRespostas
                        .AsEnumerable()
                        .Where(row => !string.IsNullOrWhiteSpace(row[$"q{numQuestao}"].ToString()) && row[$"q{numQuestao}"].ToString() != "*")
                        .GroupBy(row => new { AnoEscolar = int.Parse(row["AnoEscolar"].ToString()), 
                            UadSigla = row["cod_DRE"].ToString(),
                            EscCod = row["codesc"].ToString(),
                            Alternativa = row[$"q{numQuestao}"].ToString() })
                        .Select(x => new
                        {
                            x.Key.AnoEscolar,
                            x.Key.UadSigla,
                            x.Key.EscCod,
                            x.Key.Alternativa,
                            Quantidade = x.Count()
                        })
                        .ToList();

                    var agrupamentosPorAnoEscolarEUad = agrupamentosPorAnoEscolarUadEscolaEOpcao
                        .GroupBy(x => new { x.AnoEscolar, x.UadSigla, x.EscCod })
                        .Select(x => new
                        {
                            x.Key.AnoEscolar,
                            x.Key.UadSigla,
                            x.Key.EscCod,
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

                            var quantidadeDaOpcao = agrupamentosPorAnoEscolarUadEscolaEOpcao
                                    .FirstOrDefault(x => x.AnoEscolar == agrupamentoPorAnoEscolarEUad.AnoEscolar 
                                    && x.UadSigla == agrupamentoPorAnoEscolarEUad.UadSigla
                                    && x.EscCod == agrupamentoPorAnoEscolarEUad.EscCod
                                    && x.Alternativa == opcao.Letra)
                                    ?.Quantidade;

                            var valor = quantidadeDaOpcao is null
                                ? 0m
                                : (quantidadeDaOpcao.Value * 100m) / agrupamentoPorAnoEscolarEUad.Quantidade;

                            if(valor > 100m)
                            {

                            }

                            var entity = new FatorAssociadoQuestionarioRespostaEscola
                            {
                                AnoEscolar = agrupamentoPorAnoEscolarEUad.AnoEscolar,
                                CicloId = cicloId,
                                Edicao = dto.Edicao,
                                EscCodigo = agrupamentoPorAnoEscolarEUad.EscCod,
                                FatorAssociadoQuestionarioId = 6,
                                Id = _nextPKEscola++,
                                ItemDescricao = opcao.Descricao,
                                ItemId = opcao.Numero,
                                UadSigla = agrupamentoPorAnoEscolarEUad.UadSigla,
                                Valor = valor,
                                VariavelDescricao = questao.Enunciado,
                                VariavelId = questao.Numero.ToString()
                            };

                            _entitiesEscola.Add(entity);
                        }
                    }
                }

                await _fatorAssociadoQuestionarioRespostaEscolaRepository.InsertAsync(_entitiesEscola);
            }
            catch (Exception ex)
            {
                dto.AddErro(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
