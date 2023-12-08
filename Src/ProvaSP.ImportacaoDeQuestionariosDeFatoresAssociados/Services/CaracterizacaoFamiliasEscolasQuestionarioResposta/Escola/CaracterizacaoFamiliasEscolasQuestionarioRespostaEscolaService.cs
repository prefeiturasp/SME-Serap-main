using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ImportacaoDeQuestionariosSME.Data.Repositories.CiclosAnoEscolar;
using ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociadosQuestionarioResposta.Escolas;
using ImportacaoDeQuestionariosSME.Domain.CiclosAnoEscolar;
using ImportacaoDeQuestionariosSME.Domain.FatoresAssociadosQuestionarioResposta.Escolas;
using ImportacaoDeQuestionariosSME.Services.CaracterizacaoFamiliasEscolasQuestionarioResposta.Dtos;
using ImportacaoDeQuestionariosSME.Utils;

namespace ImportacaoDeQuestionariosSME.Services.CaracterizacaoFamiliasEscolasQuestionarioResposta.Escola
{
    public class CaracterizacaoFamiliasEscolasQuestionarioRespostaEscolaService : ICaracterizacaoFamiliasEscolasQuestionarioRespostaService
    {
        private readonly ICicloAnoEscolarRepository _cicloAnoEscolarRepository = new CicloAnoEscolarRepository();
        private readonly IFatorAssociadoQuestionarioRespostaEscolaRepository _fatorAssociadoQuestionarioRespostaEscolaRepository = new FatorAssociadoQuestionarioRespostaEscolaRepository();

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
                dto.AddErro("O arquivo não é válido para importação do tipo Escola.");
                return;
            }

            var ciclosAnosEscolares = await _cicloAnoEscolarRepository.GetAsync();
            var id = await _fatorAssociadoQuestionarioRespostaEscolaRepository.GetNextPk();
            var entities = new List<FatorAssociadoQuestionarioRespostaEscola>();

            var indice = 0;
            while (indice < csv.Rows.Count)
            {
                var row = csv.Rows[indice];

                var uadSigla = row["dre_sigla"].ToString();
                var escCodigo = row["ue_codigo_eol"].ToString().PadLeft(6, '0');
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

                var entity = new FatorAssociadoQuestionarioRespostaEscola
                {
                    Edicao = dto.Edicao,
                    CicloId = cicloAnoEscolar.CicloId,
                    FatorAssociadoQuestionarioId = dto.FatorAssociadoQuestionarioId,
                    VariavelId = variavelId,
                    ItemId = itemId,
                    UadSigla = uadSigla,
                    EscCodigo = escCodigo,
                    VariavelDescricao = variavelDescricao,
                    ItemDescricao = itemDescricao,
                    Valor = valor,
                    Id = id++,
                    AnoEscolar = anoEscolar
                };
                entities.Add(entity);
                indice++;
            }

            //await _fatorAssociadoQuestionarioRespostaEscolaRepository.InsertAsync(entities);
        }

        private static bool EhArquivoValido(DataTable csv)
        {
            var colunasArquivo = new[] { "dre_sigla", "ue_codigo_eol", "QuestaoId", "ItemId", "Questao", "Item", "Valor", "AnoEscolar" };

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
