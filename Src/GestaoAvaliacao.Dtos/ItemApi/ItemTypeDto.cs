using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Dtos.ItemApi
{
    public class ItemTypeDto : BaseDto
    {
        public bool EhPadrao { get; set; }
        public int? QuantidadeAlternativa { get; set; }
    }
}
