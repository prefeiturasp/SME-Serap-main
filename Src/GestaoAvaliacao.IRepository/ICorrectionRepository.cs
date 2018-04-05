using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Util;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface ICorrectionRepository
	{
        IEnumerable<SelectedSection> LoadOnlySelectedSectionPaginate(ref Pager pager, StudentResponseFilter filter);

        IEnumerable<TestTemplate> GetTestTemplate(long test_id);
	}
}
