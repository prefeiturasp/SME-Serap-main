using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ProvaSP.Data.Funcionalidades
{
    /// <summary>
    /// Tipos de bases de dados
    /// </summary>
    public enum TipoBaseDados
    {
        /// <summary>
        /// Base de dados da Produção
        /// </summary>
        [Description("ProvaSP")]
        Producao = 0,
        /// <summary>
        /// Base de dados Desenvolvimento
        /// </summary>
        [Description("ProvaSP_Dev")]
        Teste = 1
    }
}
