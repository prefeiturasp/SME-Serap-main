using System.Data;
using System.Data.SqlClient;

namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Data.Abstractions
{
    internal abstract class BaseRepository
    {
        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection("Data Source=SMESQLCLUSTER\\SME_PRD;Initial Catalog=GestaoAvaliacao;User Id=user_gestaoavaliacao;Password=gestaoavaliacao@adm;");
            }
        }
    }
}