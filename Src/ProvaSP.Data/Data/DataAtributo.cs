using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using ProvaSP.Data.Funcionalidades;
using ProvaSP.Model.Entidades;
namespace ProvaSP.Data
{
    public static class DataAtributo
    {
        public static int RecuperarID(string AtributoNome, SqlTransaction dbContextTransaction, SqlConnection conn)
        {
            return conn.ExecuteScalar<int>(
                                        sql: @"
                                    DECLARE @AtributoID int
                                    IF (NOT EXISTS(SELECT * FROM Atributo WITH (NOLOCK) WHERE Nome=@Nome))
                                    BEGIN
                                        INSERT INTO Atributo (Nome) VALUES (@Nome)
                                        SELECT @AtributoID=@@IDENTITY
                                    END
                                    ELSE
                                        SELECT @AtributoID=AtributoID FROM Atributo WITH (NOLOCK) WHERE Nome=@Nome

                                    SELECT @AtributoID
                                    ",
                                        param: new
                                        {
                                            Nome = new DbString() { Value = AtributoNome, IsAnsi = true, Length = 150 }
                                        },
                                        transaction: dbContextTransaction);
        }
    }
}
