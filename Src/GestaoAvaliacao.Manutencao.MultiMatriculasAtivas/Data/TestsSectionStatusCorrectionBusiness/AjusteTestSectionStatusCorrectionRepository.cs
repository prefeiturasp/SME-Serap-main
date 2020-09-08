using Dapper;
using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Data.Abstractions;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Data.TestsSectionStatusCorrectionBusiness
{
    internal class AjusteTestSectionStatusCorrectionRepository : BaseRepository, IAjusteTestSectionStatusCorrectionRepository
    {
        public async Task UpdateStatusAsync(long testId, long turId, EnumStatusCorrection status)
        {
            var command = @"UPDATE 
                                TestSectionStatusCorrection
                            SET
                                StatusCorrection = @status
                            WHERE
                                Test_Id = @testId
                                AND tur_id = @turId;";

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                if(await cn.ExecuteAsync(command, new { status = (byte)status, testId, turId }) <= 0)
                    await InsertStatusAsync(testId, turId, status);
            }
        }

        private async Task InsertStatusAsync(long testId, long turId, EnumStatusCorrection status)
        {
            var command = @"INSERT INTO 
                                TestSectionStatusCorrection
                            VALUES
                                (@testId, @turId, @status, GETDATE(), GETDATE(), 1);";

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                await cn.ExecuteAsync(command, new { status = (byte)status, testId, turId });
            }
        }
    }
}
