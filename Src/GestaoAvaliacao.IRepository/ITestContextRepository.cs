using GestaoAvaliacao.Entities;
using System;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestContextRepository
    {
        TestContext Save(TestContext entity);
        TestContext Update(TestContext entity);
        TestContext Update(long Id, TestContext entity);
        void Delete(long id);
        void DeleteByTestId(long id);
    }
}
