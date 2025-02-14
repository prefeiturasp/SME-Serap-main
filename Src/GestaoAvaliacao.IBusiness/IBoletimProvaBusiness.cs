using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestaoAvaliacao.Entities.DTO.SerapEstudantes;

namespace GestaoAvaliacao.IBusiness
{
    public interface IBoletimProvaBusiness
    {
        AdminAutenticacaoRespostaDTO AdminAutenticacao(AdminAutenticacaoDTO adminAutenticacaoDTO);
    }
}
