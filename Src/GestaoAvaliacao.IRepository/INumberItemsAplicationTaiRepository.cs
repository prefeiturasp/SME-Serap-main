using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface INumberItemsAplicationTaiRepository
    {
        NumberItemsAplicationTai GetByTestId(long testId);
        IEnumerable<NumberItemsAplicationTai> GetAll();
    }
}
