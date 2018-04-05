using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business
{
    public class SectionTestGenerateLotBusiness : ISectionTestGenerateLotBusiness
    {
        private readonly ISectionTestGenerateLotRepository sectionTestGenerateLotRepository;

        public SectionTestGenerateLotBusiness(ISectionTestGenerateLotRepository sectionTestGenerateLotRepository)
        {
            this.sectionTestGenerateLotRepository = sectionTestGenerateLotRepository;
        }

        public void Save(IEnumerable<SectionTestGenerateLot> list)
        {
            sectionTestGenerateLotRepository.InsertMany(list);
        }

        public async Task<IEnumerable<SectionTestGenerateLot>> Load(long test_id, long tur_id)
        {
            return await sectionTestGenerateLotRepository.Load(test_id, tur_id);
        }
    }
}
