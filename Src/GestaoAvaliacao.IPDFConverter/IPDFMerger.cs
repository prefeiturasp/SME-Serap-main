using System.Collections.Generic;

namespace GestaoAvaliacao.IPDFConverter
{
    public interface IPDFMerger
	{
		byte[] Merge(IEnumerable<string> files);
	}
}
