using GestaoAvaliacao.FGVIntegration.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.FGVIntegration.Data
{
    public interface IEolRepository
    {
        /// <summary>
        /// Busca a informação de 
        /// </summary>
        Task<ICollection<Escola>> BuscarDiretoresEscolas(ICollection<Escola> pEscolas);

        /// <summary>
        /// Busca os coordenadores das escolas de ensino médio. 
        /// Se for informado o parâmetro pCodigoEscolas, serão consideradas apenas aquelas escolas de ensino médio.
        /// </summary>
        Task<ICollection<Coordenador>> BuscarCoordenadoresEscolas(ICollection<Escola> pEscolas);
    }
}