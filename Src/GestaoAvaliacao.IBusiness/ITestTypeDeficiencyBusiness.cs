using GestaoAvaliacao.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface ITestTypeDeficiencyBusiness
    {
        Task<IEnumerable<DeficiencyDto>> GetDeficienciesAsync();
    }
}
