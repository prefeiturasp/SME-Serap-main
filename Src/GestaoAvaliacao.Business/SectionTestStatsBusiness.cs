using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business
{
    public class SectionTestStatsBusiness : ISectionTestStatsBusiness
	{
		readonly ISectionTestStatsRepository sectionTestStatsRepository;

		public SectionTestStatsBusiness(ISectionTestStatsRepository sectionTestStatsRepository)
		{
			this.sectionTestStatsRepository = sectionTestStatsRepository;
		}

		public Task<List<SectionTestStats>> GetByTest(long test_id)
		{
			return this.sectionTestStatsRepository.GetByTest(test_id);
		}

        public async Task<SectionTestStats> GetByTest(long test_id, long tur_id)
		{
			return await sectionTestStatsRepository.GetByTest(test_id, tur_id);
		}

		public async Task<SectionTestStats> Save(SectionTestStats entity, Guid ent_id)
		{
			SectionTestStats sectionTestStats = new SectionTestStats(entity.Test_Id, entity.tur_id, ent_id, entity.dre_id, entity.esc_id);

			var count = await sectionTestStatsRepository.Count(sectionTestStats);
			if (count == 0)
				return await sectionTestStatsRepository.Insert(entity);
			else
			{
				var cadastred = await sectionTestStatsRepository.GetEntity(sectionTestStats);
				cadastred.GeneralGrade = entity.GeneralGrade;
                cadastred.Answers = entity.Answers;
				cadastred.GeneralHits = entity.GeneralHits;
                cadastred.NumberAnswers = entity.NumberAnswers;

                return await sectionTestStatsRepository.Replace(cadastred);
			}
		}

		public async Task<List<SectionTestStatsGroupDTO>> GetGrouped(long test_id, IEnumerable<long> turmas)
		{
			return await sectionTestStatsRepository.GetGrouped(test_id, turmas);
		}
    }
}
