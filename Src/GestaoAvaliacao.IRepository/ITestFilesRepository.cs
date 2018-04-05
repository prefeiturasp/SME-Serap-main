using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestFilesRepository
	{
        IEnumerable<TestFiles> Update(TestFiles test, List<TestFiles> files);
        void SaveList(List<TestFiles> list);
        void UpdateList(List<TestFiles> list);
        IEnumerable<File> GetFiles(long Id, EnumFileType answerSheetType);
        IEnumerable<TestFiles> GetTestFiles(long Id);
        bool GetChecked(long Id, long OwnerId);
	}
}
