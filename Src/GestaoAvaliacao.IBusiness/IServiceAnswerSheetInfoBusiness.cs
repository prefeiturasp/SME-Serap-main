using GestaoAvaliacao.MongoEntities;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface IServiceAnswerSheetInfoBusiness
    {
        Task<ServiceAnswerSheetInfo> Get(ServiceAnswerSheetInfo entity);

        Task<ServiceAnswerSheetInfo> Save(ServiceAnswerSheetInfo entity);
    }
}