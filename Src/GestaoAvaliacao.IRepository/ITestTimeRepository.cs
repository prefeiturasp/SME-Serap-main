using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestTimeRepository
	{
		List<TestTime> GetAll();
	}
}
