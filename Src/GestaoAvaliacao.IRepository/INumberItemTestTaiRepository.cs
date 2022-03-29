using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface INumberItemTestTaiRepository
    {
        NumberItemTestTai Save(NumberItemTestTai entity);
        void DeleteByTestId(long testId);
        void DeleteSaveByTestId(NumberItemTestTai newItem);
    }
}
