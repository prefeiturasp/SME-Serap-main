using ImportacaoDeQuestionariosSME.Data.Repositories.CiclosAnoEscolar;
using ImportacaoDeQuestionariosSME.Data.Repositories.Constructos;
using ImportacaoDeQuestionariosSME.Data.Repositories.Escolas;
using ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociadosQuestionarioResposta.Escolas;
using ImportacaoDeQuestionariosSME.Data.Repositories.Questionarios.Itens;
using ImportacaoDeQuestionariosSME.Domain.CiclosAnoEscolar;
using ImportacaoDeQuestionariosSME.Domain.Constructos;
using ImportacaoDeQuestionariosSME.Domain.Escolas;
using ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.Escolas;
using ImportacaoDeQuestionariosSME.Domain.Questionarios.Itens;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.DRE.Dtos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Dtos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Escolas.Dtos;
using ImportacaoDeQuestionariosSME.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Escolas
{
    public class FatorAssociadoQuestionarioRespostaEscolaServices : FatorAssociadoQuestionarioRespostaServices, IFatorAssociadoQuestionarioRespostaServices
    {
        private readonly ICicloAnoEscolarRepository _cicloAnoEscolarRepository;
        private readonly IConstructoRepository _constructoRepository;
        private readonly IFatorAssociadoQuestionarioRespostaEscolaConstructoRepository _fatorAssociadoQuestionarioRespostaEscolaConstructoRepository;
        private readonly IFatorAssociadoQuestionarioRespostaEscolaRepository _fatorAssociadoQuestionarioRespostaEscolaRepository;
        private readonly IQuestionarioItemRepository _questionarioItemRepository;

        public FatorAssociadoQuestionarioRespostaEscolaServices(IEnumerable<QuestaoConstructoDto> questaoConstructoDtos)
            :base(questaoConstructoDtos)
        {
            _cicloAnoEscolarRepository = new CicloAnoEscolarRepository();
            _constructoRepository = new ConstructoRepository();
            _fatorAssociadoQuestionarioRespostaEscolaConstructoRepository = new FatorAssociadoQuestionarioRespostaEscolaConstructoRepository();
            _fatorAssociadoQuestionarioRespostaEscolaRepository = new FatorAssociadoQuestionarioRespostaEscolaRepository();
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
                var nextPK = await _fatorAssociadoQuestionarioRespostaEscolaRepository.GetNextPk();

                var respostasEscola = dtQuestionario
                    .AsEnumerable()
                    .Where(row => !string.IsNullOrWhiteSpace(row["Resposta"].ToString()))
                    .Select(row => new FatorAssociadoQuestionarioRespostaEscolaDto
                    {
                        AnoEscolar = int.Parse(row["AnoEscolar"].ToString()),
                        Quantidade = decimal.Parse(row["Quantidade"].ToString()),
                        Questao = int.Parse(row["Questao"].ToString()),
                        Resposta = row["Resposta"].ToString(),
                        UadSigla = row["uad_sigla"].ToString(),
                        EscCodigo = row["esc_codigo"].ToString()
                    })
                    .ToList();

                var respostasEscolaAgrupadas = respostasEscola
                    .GroupBy(x => new { x.AnoEscolar, x.UadSigla, x.Questao, x.EscCodigo })
                    .Select(x => new
                    {
                        x.Key.AnoEscolar,
                        x.Key.Questao,
                        x.Key.UadSigla,
                        x.Key.EscCodigo,
                        Quantidade = x.Sum(y => y.Quantidade)
                    })
                    .ToList();

                var entities = new List<FatorAssociadoQuestionarioRespostaEscola>();
                var entitiesConstructos = new List<FatorAssociadoQuestionarioRespostaEscolaConstructo>();

                respostasEscola
                    .ForEach(x =>
                    {
                        var questionarioItem = questionarioItens.FirstOrDefault(y => y.Numero == x.Questao);
                        var cicloId = ciclosAnoEscolar.First(z => z.AnoEscolar == x.AnoEscolar).CicloId;
                        var valor = GetValorPercentual(x.Quantidade,
                                    respostasEscolaAgrupadas.First(z => z.AnoEscolar == x.AnoEscolar 
                                        && z.Questao == x.Questao 
                                        && z.UadSigla == x.UadSigla 
                                        && z.EscCodigo == x.EscCodigo)
                                    .Quantidade);
                        var constructosRelacionados = GetConstructosRelacionados(dto.Edicao, cicloId, x.AnoEscolar, x.Questao, constructos);

                        var entity = new FatorAssociadoQuestionarioRespostaEscola
                        {
                            Id = nextPK++,
                            AnoEscolar = x.AnoEscolar,
                            CicloId = cicloId,
                            Edicao = dto.Edicao,
                            EscCodigo = x.EscCodigo,
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
                                .Select(y => new FatorAssociadoQuestionarioRespostaEscolaConstructo
                                {
                                    ConstructoId = y.ConstructoId,
                                    FatorAssociadoQuestionarioRespostaEscolaId = entity.Id
                                })
                                .ToList();

                            entitiesConstructos.AddRange(dreConstructos);
                        }
                    });

                await _fatorAssociadoQuestionarioRespostaEscolaRepository.InsertAsync(entities);
                await _fatorAssociadoQuestionarioRespostaEscolaConstructoRepository.InsertAsync(entitiesConstructos);
            }
            catch (Exception ex)
            {
                dto.AddErro(ex.InnerException?.Message ?? ex.Message);
            }
        }

        private string GetItemDescricao(QuestionarioItem questionarioItem, FatorAssociadoQuestionarioRespostaEscolaDto dreDto)
        {
            if (string.IsNullOrWhiteSpace(dreDto.Resposta)) return "Não respondido.";
            return questionarioItem.Opcoes.First(z => z.Letra == dreDto.Resposta).Descricao;
        }

        private int GetItemId(QuestionarioItem questionarioItem, FatorAssociadoQuestionarioRespostaEscolaDto dreDto)
        {
            if (string.IsNullOrWhiteSpace(dreDto.Resposta)) return questionarioItem.Opcoes.Count() + 1;
            return questionarioItem.Opcoes.First(z => z.Letra == dreDto.Resposta).Numero;
        }
    }
}