using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.Util;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
	public interface ITestContextBusiness
	{
		TestContext Save(TestContext entity);
		TestContext Update(long Id, TestContext entity);
		TestContext Delete(long Id);
		void DeleteByTestId(long Id);
	}
}
