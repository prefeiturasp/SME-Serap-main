﻿using GestaoAvaliacao.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface INumberItemsAplicationTaiBusiness
    {
        NumberItemsAplicationTai GetByTestId(long testId);
        IEnumerable<NumberItemsAplicationTai> GetAll();
    }
}
