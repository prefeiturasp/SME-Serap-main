using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using GestaoEscolar.IRepository;
using System;
using System.Collections.Generic;

namespace GestaoEscolar.Business
{
    public class ACA_CurriculoPeriodoBusiness : IACA_CurriculoPeriodoBusiness
    {
        private readonly IACA_CurriculoPeriodoRepository curriculumGradeRepository;

        public ACA_CurriculoPeriodoBusiness(IACA_CurriculoPeriodoRepository curriculumGradeRepository)
        {
            this.curriculumGradeRepository = curriculumGradeRepository;
        }

        #region Read

        public IEnumerable<ACA_CurriculoPeriodo> Load(Guid ent_id, int cur_id)
        {
            return curriculumGradeRepository.Load(cur_id, ent_id);
        }

		public IEnumerable<ACA_CurriculoPeriodo> GetCurriculumGradeByesc_id(int esc_id)
		{
			return curriculumGradeRepository.GetCurriculumGradeByesc_id(esc_id);
		}

        public ACA_CurriculoPeriodo Get(long id)
        {
            ACA_CurriculoPeriodo result = curriculumGradeRepository.Get(id);
            if (result != null)
                return result;
            else
                return new ACA_CurriculoPeriodo();
        }

        public object GetCustom(long id)
        {
            ACA_CurriculoPeriodo result = curriculumGradeRepository.Get(id);
            if (result != null)
            {
                return new
                {
                    Id = result.crp_id,
                    Ordem = result.crp_ordem,
                    Description = result.crp_descricao
                };
            }
            else
            {
                return new
                {
                    Id = 0,
                    Ordem = 0,
                    Description = string.Empty
                };
            }
        }

        #endregion
    }
}
