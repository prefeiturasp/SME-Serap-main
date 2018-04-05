using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IBusiness
{
    public interface IFormatTypeBusiness
    {
        FormatType Get(long id);

        IEnumerable<FormatType> Load();
    }
}
