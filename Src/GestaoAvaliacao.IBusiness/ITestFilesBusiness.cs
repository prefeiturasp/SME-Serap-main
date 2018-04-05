using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface ITestFilesBusiness
	{
		IEnumerable<TestFiles> Update(TestFiles test, List<TestFiles> files, Guid UsuId, EnumSYS_Visao vision);
		void SaveList(List<TestFiles> list);
		void UpdateList(List<TestFiles> list);
		IEnumerable<File> GetFiles(long Id, EnumFileType answerSheetType);
		IEnumerable<TestFiles> GetTestFiles(long Id);
		bool GetChecked(long Id, long OwnerId);
	}
}
