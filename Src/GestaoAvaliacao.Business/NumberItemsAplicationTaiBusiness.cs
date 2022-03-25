using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;

namespace GestaoAvaliacao.Business
{
    public class NumberItemsAplicationTaiBusiness : INumberItemsAplicationTaiBusiness
    {

        private readonly INumberItemsAplicationTaiRepository numberItemsAplicationTaiRepository;

        public NumberItemsAplicationTaiBusiness(INumberItemsAplicationTaiRepository numberItemsAplicationTaiRepository)
        {
            this.numberItemsAplicationTaiRepository = numberItemsAplicationTaiRepository;
        }

        public NumberItemsAplicationTai GetByTestId(long testId)
        {
            return numberItemsAplicationTaiRepository.GetByTestId(testId);
        }

        public IEnumerable<NumberItemsAplicationTai> GetAll()
        {
            return numberItemsAplicationTaiRepository.GetAll();
        }
    }
}
