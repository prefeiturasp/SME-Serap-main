using GestaoAvaliacao.MongoEntities;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface IServiceAnswerSheetInfoRepository
    {
        Task<ServiceAnswerSheetInfo> Get(ServiceAnswerSheetInfo entity);

        Task<ServiceAnswerSheetInfo> Save(ServiceAnswerSheetInfo entity);
    }
}