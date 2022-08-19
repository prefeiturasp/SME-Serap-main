using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Dtos.ItemApi
{
   public class AbilityDto : BaseDto
    {
        public string Codigo { get; set; }
        public bool UltimoNivel { get; set; }
    }
}
