using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface ITestTimeBusiness
	{
		List<TestTime> GetAll();
    }
}
