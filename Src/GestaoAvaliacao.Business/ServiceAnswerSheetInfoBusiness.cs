using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business
{
    public class ServiceAnswerSheetInfoBusiness : IServiceAnswerSheetInfoBusiness
    {
        private readonly IServiceAnswerSheetInfoRepository serviceAnswerSheetInfoBusiness;

        public ServiceAnswerSheetInfoBusiness(IServiceAnswerSheetInfoRepository serviceAnswerSheetInfoBusiness)
        {
            this.serviceAnswerSheetInfoBusiness = serviceAnswerSheetInfoBusiness;
        }

        public async Task<ServiceAnswerSheetInfo> Get(ServiceAnswerSheetInfo entity)
        {
            return await serviceAnswerSheetInfoBusiness.Get(entity);
        }

        public async Task<ServiceAnswerSheetInfo> Save(ServiceAnswerSheetInfo entity)
        {
            return await serviceAnswerSheetInfoBusiness.Save(entity);
        }
    }
}