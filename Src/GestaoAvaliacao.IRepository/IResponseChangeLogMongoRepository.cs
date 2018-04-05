using GestaoAvaliacao.MongoEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface IResponseChangeLogMongoRepository
    {
        Task<ResponseChangeLog> Insert(ResponseChangeLog entity);
        List<ResponseChangeLog> GetResponseChangeLog(long test_id, Guid? uad_id, long? esc_id, long? tur_id);
    }
}
