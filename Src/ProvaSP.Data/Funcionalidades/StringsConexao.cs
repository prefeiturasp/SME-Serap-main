using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvaSP.Data.Funcionalidades
{
    public static class StringsConexao
    {
        /// <summary>
        /// Retorna a StringConnection do Site
        /// </summary>
        /// <value>StringConnection</value>
        public static string ProvaSP
        {
            get
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    return ConfigurationManager.ConnectionStrings["DEBUG_ProvaSP"].ConnectionString;
                }
                else
                {
                    return ConfigurationManager.ConnectionStrings["ProvaSP"].ConnectionString;
                }

            }
        }

        public static string CoreSSO
        {
            get
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    return ConfigurationManager.ConnectionStrings["DEBUG_ProvaSP_CoreSSO"].ConnectionString;
                }
                else
                {
                    return ConfigurationManager.ConnectionStrings["ProvaSP_CoreSSO"].ConnectionString;
                }
                
            }
        }

        public static string GestaoEscolar
        {
            get
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    return ConfigurationManager.ConnectionStrings["DEBUG_ProvaSP_GestaoEscolar"].ConnectionString;
                }
                else
                {
                    return ConfigurationManager.ConnectionStrings["ProvaSP_GestaoEscolar"].ConnectionString;
                }
            }
        }
    }
}
