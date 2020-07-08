using ImportacaoDeQuestionariosSME.Data.Repositories.Abstractions;
using ImportacaoDeQuestionariosSME.Domain.Constructos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Data.Repositories.Constructos
{
    public class ConstructoRepository : BaseInsertRepository<Constructo>, IConstructoRepository
    {
        public async Task InsertAsync(IEnumerable<Constructo> constructos)
        {
            if (constructos is null || !constructos.Any()) return;

            var sqls = GetSqlsInPages(constructos);
            foreach (var sql in sqls)
            {
                await _dapperContext.ExecuteAsync(sql);
            }
        }

        protected override string GetQueryInsert()
            => @"INSERT INTO Constructo
               (ConstructoId, Edicao, CicloId, FatorAssociadoQuestionarioId, Nome, Referencia, AnoEscolar)
               VALUES ";

        protected override string GetValuesQueryForEntity(Constructo entity)
            => $"({entity.ConstructoId}, '{entity.Edicao}', {entity.CicloId}, {entity.FatorAssociadoQuestionarioId}, '{entity.Nome}', '{entity.Referencia}', {entity.AnoEscolar})";

        public async Task<int> GetMaxConstructoId()
        {
            var query = "SELECT MAX(ConstructoId) FROM Constructo (NOLOCK)";
            return await _dapperContext.QuerySingleOrDefaultAsync<int>(query);
        }

        public async Task<IEnumerable<Constructo>> GetAsync(string edicao)
        {
            var query = $"SELECT * FROM Constructo (NOLOCK) WHERE Edicao = '{edicao}'";
            return await _dapperContext.QueryAsync<Constructo>(query);
        }
    }
}