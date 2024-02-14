using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ImportacaoDeQuestionariosSME.Data.Repositories.CiclosAnoEscolar;
using ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociadosQuestionario.DRE;
using ImportacaoDeQuestionariosSME.Domain.CiclosAnoEscolar;
using ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.DRE;
using ImportacaoDeQuestionariosSME.Services.CaracterizacaoFamiliasEscolasQuestionarioResposta.Dtos;
using ImportacaoDeQuestionariosSME.Utils;

namespace ImportacaoDeQuestionariosSME.Services.CaracterizacaoFamiliasEscolasQuestionarioResposta.DRE
{
    public class CaracterizacaoFamiliasEscolasQuestionarioRespostaDreService : ICaracterizacaoFamiliasEscolasQuestionarioRespostaService
    {
        private readonly ICicloAnoEscolarRepository _cicloAnoEscolarRepository = new CicloAnoEscolarRepository();
        private readonly IFatorAssociadoQuestionarioRespostaDRERepository _fatorAssociadoQuestionarioRespostaDreRepository = new FatorAssociadoQuestionarioRespostaDRERepository();

        public async Task ImportarAsync(CaracterizacaoFamiliasEscolasQuestionarioRespostaDto dto)
        {
            if (dto is null)
            {
                dto = new CaracterizacaoFamiliasEscolasQuestionarioRespostaDto();
                dto.AddErro("O DTO é nulo.");
                return;
            }

            var csv = CsvManager.GetCsvFile(dto.CaminhoArquivo);

            if (csv.Rows.Count <= 0)
            {
                dto.AddErro("Não existem regitros para serem importados.");
                return;
            }

            if (!EhArquivoValido(csv))
            {
                dto.AddErro("O arquivo não é válido para importação do tipo DRE.");
                return;
            }

            var ciclosAnosEscolares = await _cicloAnoEscolarRepository.GetAsync();
            var id = await _fatorAssociadoQuestionarioRespostaDreRepository.GetNextPk();
            var entities = new List<FatorAssociadoQuestionarioRespostaDRE>();

            var indice = 0;
            while (indice < csv.Rows.Count)
            {
                var row = csv.Rows[indice];

                var uadSigla = row["dre_sigla"].ToString();
                var variavelId = row["QuestaoId"].ToString();
                var itemId = Convert.ToInt32(row["ItemId"].ToString());
                var variavelDescricao = row["Questao"].ToString();
                var itemDescricao = row["Item"].ToString();
                var valor = Convert.ToDecimal(row["Valor"].ToString());
                var anoEscolar = Convert.ToInt32(row["AnoEscolar"].ToString());

                var cicloAnoEscolar = ciclosAnosEscolares.FirstOrDefault(c => c.AnoEscolar == anoEscolar);

                if (cicloAnoEscolar == null)
                {
                    dto.AddErro("Ciclo não localizado.");
                    return;
                }

                var entity = new FatorAssociadoQuestionarioRespostaDRE
                {
                    Edicao = dto.Edicao,
                    CicloId = cicloAnoEscolar.CicloId,
                    FatorAssociadoQuestionarioId = dto.FatorAssociadoQuestionarioId,
                    VariavelId = variavelId,
                    ItemId = itemId,
                    UadSigla = uadSigla,
                    VariavelDescricao = variavelDescricao,
                    ItemDescricao = itemDescricao,
                    Valor = valor,
                    Id = id++,
                    AnoEscolar = anoEscolar
                };
                entities.Add(entity);
                indice++;
            }

            //await _fatorAssociadoQuestionarioRespostaDreRepository.InsertAsync(entities);
        }

        private static bool EhArquivoValido(DataTable csv)
        {
            var colunasArquivo = new[] { "dre_sigla", "QuestaoId", "ItemId", "Questao", "Item", "Valor", "AnoEscolar" };

            for (var i = 0; i < csv.Columns.Count; i++)
            {
                var nomeColuna = csv.Columns[i].ColumnName;

                if (!colunasArquivo.Select(c => c.ToLower()).Contains(nomeColuna.ToLower()))
                    return false;
            }

            return true;
        }
    }
}
