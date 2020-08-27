using ImportacaoDeQuestionariosSME.Domain.Constructos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta
{
    public abstract class FatorAssociadoQuestionarioRespostaServices
    {
        protected readonly IEnumerable<QuestaoConstructoDto> _questaoConstructoDtos;
        protected const int FatorAssociadoQuestionarioIdEstudante = 5;

        public FatorAssociadoQuestionarioRespostaServices(IEnumerable<QuestaoConstructoDto> questaoConstructoDtos)
        {
            _questaoConstructoDtos = questaoConstructoDtos;
        }

        protected DataTable AdjustDataTable(DataTable dtImportacao)
        {
            dtImportacao.Columns.Add("variavel_descricao", typeof(string));

            var variavelDescricao = string.Empty;
            foreach (DataRow row in dtImportacao.Rows)
            {
                if (row["seq"].ToString() == "0")
                {
                    variavelDescricao = row["texto"].ToString();
                    continue;
                }

                if (string.IsNullOrWhiteSpace(row["alternativa"].ToString())) continue;
                row["variavel_descricao"] = variavelDescricao;
            }

            var rowsToRemove = dtImportacao.Select("seq = '0' or seq = '-1' or seq = '-2'");
            foreach (var row in rowsToRemove) dtImportacao.Rows.Remove(row);

            return dtImportacao;
        }

        protected IEnumerable<Constructo> GetConstructosRelacionados(string edicao, int cicloId, int anoEscolar, int questao, IEnumerable<Constructo> constructos)
        {
            var questaoConstructoDtos = _questaoConstructoDtos.Where(x => x.Questao == questao);
            if (questaoConstructoDtos is null || !questaoConstructoDtos.Any()) return null;

            var results = questaoConstructoDtos
                .Select(x => constructos
                             .FirstOrDefault(y => y.Edicao == edicao && y.AnoEscolar == anoEscolar && y.CicloId == cicloId && y.Nome == x.ConstructoDescricao))
                .ToList();
            if (results is null || !results.Any() || results.Count() != questaoConstructoDtos.Count()) throw new ArgumentOutOfRangeException();
            return results;
        }

        protected decimal GetValorPercentual(decimal quantidadeAtual, decimal quantidadeTotal) => quantidadeAtual * 100 / quantidadeTotal;
    }
}