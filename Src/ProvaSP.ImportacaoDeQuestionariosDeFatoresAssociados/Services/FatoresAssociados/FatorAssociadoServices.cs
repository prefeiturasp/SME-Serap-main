using ImportacaoDeQuestionariosSME.Data.Repositories.CiclosAnoEscolar;
using ImportacaoDeQuestionariosSME.Data.Repositories.Constructos;
using ImportacaoDeQuestionariosSME.Data.Repositories.FatoresAssociados;
using ImportacaoDeQuestionariosSME.Domain.CiclosAnoEscolar;
using ImportacaoDeQuestionariosSME.Domain.Constructos;
using ImportacaoDeQuestionariosSME.Domain.FatoresAssociados;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociados.Dtos;
using ImportacaoDeQuestionariosSME.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Services.FatoresAssociados
{
    public class FatorAssociadoServices : IFatorAssociadoServices
    {
        private readonly ICicloAnoEscolarRepository _cicloAnoEscolarRepository;
        private readonly IConstructoRepository _constructoRepository;
        private readonly IFatorAssociadoRepository _fatorAssociadoRepository;

        public FatorAssociadoServices()
        {
            _cicloAnoEscolarRepository = new CicloAnoEscolarRepository();
            _constructoRepository = new ConstructoRepository();
            _fatorAssociadoRepository = new FatorAssociadoRepository();
        }

        public async Task ImportarAsync(ImportacaoDeFatoresAssociadosDto dto)
        {
            if (dto is null)
            {
                dto = new ImportacaoDeFatoresAssociadosDto();
                dto.AddErro("O DTO é nulo.");
                return;
            }

            try
            {
                var dtFatoresAssociados = GetTabelaDeFatoresAssociadosAjustada(dto.CaminhoDaPlanilha);
                if (dtFatoresAssociados.Rows.Count <= 0)
                {
                    dto.AddErro("Não existem regitros na planilha para exportação.");
                    return;
                }

                var ciclosAnoEscolar = await _cicloAnoEscolarRepository.GetAsync();
                var constructos = await _constructoRepository.GetAsync(dto.Edicao);

                var entities = dtFatoresAssociados
                    .AsEnumerable()
                    .Where(row => !string.IsNullOrWhiteSpace(row["AreaConhecimentoID"].ToString()))
                    .Select(row => new FatorAssociado
                    {
                        AreaConhecimentoId = int.Parse(row["AreaConhecimentoID"].ToString()),
                        ConstructoId = GetConstructoId(dto.Edicao, int.Parse(row["AnoEscolar"].ToString()), row["ID"].ToString(), ciclosAnoEscolar, constructos),
                        Pontos = row["Pontos"].ToString()
                    })
                    .ToList();

                await _fatorAssociadoRepository.InsertAsync(entities);
            }
            catch (Exception ex)
            {
                dto.AddErro(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public DataTable GetTabelaDeFatoresAssociadosAjustada(string caminhoDaPlanilha)
        {
            var dtFatoresAssociados = CsvManager.GetCsvFile(caminhoDaPlanilha);
            dtFatoresAssociados = AdjustDataTable(dtFatoresAssociados);
            return dtFatoresAssociados;
        }

        private int GetConstructoId(string edicao, int anoEscolar, string nome, IEnumerable<CicloAnoEscolar> ciclosAnoEscolar, IEnumerable<Constructo> constructos)
        {
            var cicloId = ciclosAnoEscolar.FirstOrDefault(c => c.AnoEscolar == anoEscolar)?.CicloId;
            if (cicloId is null) throw new NullReferenceException($"Não foi possível definir o cicloId para o ano escolar {anoEscolar}");

            var construto = constructos
                .FirstOrDefault(x => x.Edicao == edicao && x.AnoEscolar == anoEscolar && x.CicloId == cicloId && x.Nome == nome);
            if (construto is null) throw new NullReferenceException($"Não foi possível definir o construto para o ano escolar {anoEscolar}");
            return construto.ConstructoId;
        }

        private DataTable AdjustDataTable(DataTable dtImportacao)
        {
            var rowsToRemove = dtImportacao.Select("Nivel = 'escola'");
            foreach (var row in rowsToRemove) dtImportacao.Rows.Remove(row);

            return dtImportacao;
        }
    }
}