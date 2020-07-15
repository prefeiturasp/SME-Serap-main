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
			var sectionTestStats = await sectionTestStatsRepository.FindOneAsync(entity);
			if(sectionTestStats is null)
				sectionTestStats = entity;
			else
            {
				sectionTestStats.GeneralGrade = entity.GeneralGrade;
				sectionTestStats.Answers = entity.Answers;
				sectionTestStats.GeneralHits = entity.GeneralHits;
				sectionTestStats.NumberAnswers = entity.NumberAnswers;
			}

			await sectionTestStatsRepository.InsertOrReplaceAsync(sectionTestStats);
			return sectionTestStats;
		}

		public async Task<List<SectionTestStatsGroupDTO>> GetGrouped(long test_id, IEnumerable<long> turmas)
		{
			return await sectionTestStatsRepository.GetGrouped(test_id, turmas);
		}
    }
}
