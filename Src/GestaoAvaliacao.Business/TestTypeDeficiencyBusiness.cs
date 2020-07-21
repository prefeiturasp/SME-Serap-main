using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using MSTech.CoreSSO.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business
{
    public class TestTypeDeficiencyBusiness : ITestTypeDeficiencyBusiness
    {
        private readonly ITestTypeRepository testTypeRepository;

        public TestTypeDeficiencyBusiness(ITestTypeRepository testTypeRepository)
        {
            this.testTypeRepository = testTypeRepository;
        }

        public async Task<IEnumerable<DeficiencyDto>> GetDeficienciesAsync()
        {
            var lstDeficiencies = PES_TipoDeficienciaBO.GetSelect();
            if(!lstDeficiencies?.Any() ?? true) return null;

            var result = lstDeficiencies
                .Select(x => new DeficiencyDto
                {
                    Description = x.tde_nome,
                    Id = x.tde_id
                })
                .ToList();

            return await Task.FromResult(result);
        }
    }
}
