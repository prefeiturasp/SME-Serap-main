using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Business
{
    public class TestFilesBusiness : ITestFilesBusiness
	{
		private readonly ITestFilesRepository testFileRepository;
		private readonly ITestBusiness testBusiness;

		public TestFilesBusiness(ITestFilesRepository testFileRepository, ITestBusiness testBusiness)
		{
			this.testFileRepository = testFileRepository;
			this.testBusiness = testBusiness;
		}

		#region Read

		public IEnumerable<EntityFile> GetFiles(long Id, EnumFileType answerSheetType)
		{
			return testFileRepository.GetFiles(Id, answerSheetType);
		}

		public IEnumerable<TestFiles> GetTestFiles(long Id)
		{
			return testFileRepository.GetTestFiles(Id);
		}

		public bool GetChecked(long Id, long OwnerId)
		{
			return testFileRepository.GetChecked(Id, OwnerId);
		}

		#endregion

		#region Write

		public IEnumerable<TestFiles> Update(TestFiles test, List<TestFiles> files, Guid UsuId, EnumSYS_Visao vision)
		{
			test.Validate = testBusiness.CanEdit(test.Test_Id, UsuId, vision);
			if (!test.Validate.IsValid)
			{
				var retorno = new List<TestFiles>();
				retorno.Add(test);
				return retorno.AsEnumerable();
			}
			else
				return testFileRepository.Update(test, files);

		}

		public void SaveList(List<TestFiles> list)
		{
			testFileRepository.SaveList(list);
		}

		public void UpdateList(List<TestFiles> list)
		{
			testFileRepository.UpdateList(list);
		}

		#endregion
	}
}
