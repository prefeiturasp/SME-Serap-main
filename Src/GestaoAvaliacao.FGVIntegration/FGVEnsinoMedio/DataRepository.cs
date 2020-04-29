using Dapper;
using GestaoAvaliacao.FGVIntegration.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Text;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoAvaliacao.FGVIntegration.FGVEnsinoMedio
{
    public class DataRepository : IDataRepository
    {

        private MSTech.Security.Cryptography.SymmetricAlgorithm cripto;
        private string dbConnectionString;
        private int currentYear;

        public DataRepository()
        {
            this.cripto = new MSTech.Security.Cryptography.SymmetricAlgorithm(MSTech.Security.Cryptography.SymmetricAlgorithm.Tipo.TripleDES);
            this.dbConnectionString = ConfigurationManager.ConnectionStrings["GestaoPedagogica_SGP"].ConnectionString;
            this.currentYear = DateTime.Today.Year;
        }

        public ICollection<Escola> BuscarEscolas()
        {
            using (IDbConnection dbConn = GetConnection())
            {
                dbConn.Open();
                var sql = @"
                    implementar
                ";

                var escolas = dbConn.Query<Escola>(sql, new { anoLetivo = currentYear });
                return escolas.ToList();
            }
        }

        public ICollection<Coordenador> BuscarCoordenadores()
        {
            return null;
        }

        public ICollection<Turma> BuscarTurmas()
        {
            return null;
        }

        public ICollection<Professor> BuscarProfessores()
        {
            return null;
        }

        public ICollection<ProfessorTurma> BuscarProfessoresTurmas()
        {
            return null;
        }

        public ICollection<Aluno> BuscarAlunos()
        {
            return null;
        }

        private IDbConnection GetConnection()
        {
            return new SqlConnection(DecryptTripleDES(this.dbConnectionString));
        }

        private string DecryptTripleDES(string value)
        {
            return this.cripto.Decrypt(value);
        }

    }
}