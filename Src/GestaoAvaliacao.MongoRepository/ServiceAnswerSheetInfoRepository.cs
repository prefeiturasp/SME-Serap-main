using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace GestaoAvaliacao.MongoRepository
{
    public class ServiceAnswerSheetInfoRepository : BaseRepository<ServiceAnswerSheetInfo>, IServiceAnswerSheetInfoRepository
    {
        public async Task<ServiceAnswerSheetInfo> Get(ServiceAnswerSheetInfo entity)
        {
            var filter = Builders<ServiceAnswerSheetInfo>.Filter.Eq("_id", entity._id);
            var item = await FindOne(filter);

            return item;
        }

        public async Task<ServiceAnswerSheetInfo> Save(ServiceAnswerSheetInfo entity)
        {
            Task<ServiceAnswerSheetInfo> item;

            if (await Count(entity) > 0)
            {
                item = Replace(entity);
            }
            else
            {
                item = Insert(entity);
            }

            return await item;
        }
    }
}