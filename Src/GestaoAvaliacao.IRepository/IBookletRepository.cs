using GestaoAvaliacao.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.IRepository
{
    public interface IBookletRepository
    {
        IEnumerable<Booklet> GetAllByTest(long testId);

        Booklet GetTestBooklet(long Id);

        Booklet GetBookletByTest(long Id);
    }
}
