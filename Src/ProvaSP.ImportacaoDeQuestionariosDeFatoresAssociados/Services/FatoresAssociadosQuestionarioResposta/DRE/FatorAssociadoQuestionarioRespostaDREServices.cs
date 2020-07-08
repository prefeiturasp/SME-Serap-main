using ImportacaoDeQuestionariosSME.Data.Repositories.CiclosAnoEscolar;
using ImportacaoDeQuestionariosSME.Data.Repositories.Constructos;
using ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociadosQuestionario.DRE;
using ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociadosQuestionarioResposta.DRE;
using ImportacaoDeQuestionariosSME.Data.Repositories.Questionarios.Itens;
using ImportacaoDeQuestionariosSME.Domain.CiclosAnoEscolar;
using ImportacaoDeQuestionariosSME.Domain.Constructos;
using ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.DRE;
using ImportacaoDeQuestionariosSME.Domain.Questionarios.Itens;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.DRE.Dtos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Dtos;
using ImportacaoDeQuestionariosSME.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.DRE
{
    public class FatorAssociadoQuestionarioRespostaDREServices : FatorAssociadoQuestionarioRespostaServices, IFatorAssociadoQuestionarioRespostaServices
    {
        private readonly ICicloAnoEscolarRepository _cicloAnoEscolarRepository;
        private readonly IConstructoRepository _constructoRepository;
        private readonly IFatorAssociadoQuestionarioRespostaDRERepository _fatorAssociadoQuestionarioRespostaDRERepository;
        private readonly IFatorAssociadoQuestionarioRespostaDREConstructoRepository _fatorAssociadoQuestionarioRespostaDREConstructoRepository;
        private readonly IQuestionarioItemRepository _questionarioItemRepository;

        public FatorAssociadoQuestionarioRespostaDREServices(IEnumerable<QuestaoConstructoDto> questaoConstructoDtos)
            : base(questaoConstructoDtos)
        {
            _cicloAnoEscolarRepository = new CicloAnoEscolarRepository();
            _constructoRepository = new ConstructoRepository();
            _fatorAssociadoQuestionarioRespostaDRERepository = new FatorAssociadoQuestionarioRespostaDRERepository();
            _fatorAssociadoQuestionarioRespostaDREConstructoRepository = new FatorAssociadoQuestionarioRespostaDREConstructoRepository();
            _questionarioItemRepository = new QuestionarioItemRepository();
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
                var nextPK = await _fatorAssociadoQuestionarioRespostaDRERepository.GetNextPk();

                var respostasDRE = dtQuestionario
                    .AsEnumerable()
                    .Where(row => !string.IsNullOrWhiteSpace(row["Resposta"].ToString()))
                    .Select(row => new FatorAssociadoQuestionarioRespostaDREDto
                    {
                        AnoEscolar = int.Parse(row["AnoEscolar"].ToString()),
                        Quantidade = decimal.Parse(row["Quantidade"].ToString()),
                        Questao = int.Parse(row["Questao"].ToString()),
                        Resposta = row["Resposta"].ToString(),
                        UadSigla = row["uad_sigla"].ToString(),
                    })
                    .ToList();

                var respostasDREAgrupadas = respostasDRE
                    .GroupBy(x => new { x.AnoEscolar, x.UadSigla, x.Questao })
                    .Select(x => new
                    {
                        x.Key.AnoEscolar,
                        x.Key.Questao,
                        x.Key.UadSigla,
                        Quantidade = x.Sum(y => y.Quantidade)
                    })
                    .ToList();

                var entities = new List<FatorAssociadoQuestionarioRespostaDRE>();
                var entitiesConstructos = new List<FatorAssociadoQuestionarioRespostaDREConstructo>();

                respostasDRE
                    .ForEach(x =>
                    {
                        var questionarioItem = questionarioItens.FirstOrDefault(y => y.Numero == x.Questao);
                        var cicloId = ciclosAnoEscolar.First(z => z.AnoEscolar == x.AnoEscolar).CicloId;
                        var valor = GetValorPercentual(x.Quantidade,
                                    respostasDREAgrupadas.First(z => z.AnoEscolar == x.AnoEscolar && z.Questao == x.Questao && z.UadSigla == x.UadSigla).Quantidade);
                        var constructosRelacionados = GetConstructosRelacionados(dto.Edicao, cicloId, x.AnoEscolar, x.Questao, constructos);

                        var entity = new FatorAssociadoQuestionarioRespostaDRE
                        {
                            Id = nextPK++,
                            AnoEscolar = x.AnoEscolar,
                            CicloId = cicloId,
                            Edicao = dto.Edicao,
                            FatorAssociadoQuestionarioId = FatorAssociadoQuestionarioIdEstudante,
                            ItemDescricao = GetItemDescricao(questionarioItem, x),
                            ItemId = GetItemId(questionarioItem, x),
                            UadSigla = x.UadSigla,
                            Valor = valor,
                            VariavelDescricao = $"{x.Questao} - {questionarioItem.Enunciado}",
                            VariavelId = x.Questao.ToString()
                        };

                        entities.Add(entity);

                        if (constructosRelacionados?.Any() ?? false)
                        {
                            var dreConstructos = constructosRelacionados
                                .Select(y => new FatorAssociadoQuestionarioRespostaDREConstructo
                                {
                                    ConstructoId = y.ConstructoId,
                                    FatorAssociadoQuestionarioRespostaDREId = entity.Id
                                })
                                .ToList();

                            entitiesConstructos.AddRange(dreConstructos);
                        }
                    });

                await _fatorAssociadoQuestionarioRespostaDRERepository.InsertAsync(entities);
                await _fatorAssociadoQuestionarioRespostaDREConstructoRepository.InsertAsync(entitiesConstructos);
            }
            catch (Exception ex)
            {
                dto.AddErro(ex.InnerException?.Message ?? ex.Message);
            }
        }

        private string GetItemDescricao(QuestionarioItem questionarioItem, FatorAssociadoQuestionarioRespostaDREDto dreDto)
        {
            if (string.IsNullOrWhiteSpace(dreDto.Resposta)) return "Não respondido.";
            return questionarioItem.Opcoes.First(z => z.Letra == dreDto.Resposta).Descricao;
        }

        private int GetItemId(QuestionarioItem questionarioItem, FatorAssociadoQuestionarioRespostaDREDto dreDto)
        {
            if (string.IsNullOrWhiteSpace(dreDto.Resposta)) return questionarioItem.Opcoes.Count() + 1;
            return questionarioItem.Opcoes.First(z => z.Letra == dreDto.Resposta).Numero;
        }
    }
}