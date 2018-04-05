using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IFormatTypeRepository
    {
        FormatType Get(long id);
        IEnumerable<FormatType> Load();
    }
}
