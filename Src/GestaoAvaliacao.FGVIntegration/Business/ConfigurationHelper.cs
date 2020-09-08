using System;
using System.Configuration;

namespace GestaoAvaliacao.FGVIntegration.Business
{
    public class ConfigurationHelper
    {

        public static string BuscarConfiguracaoObrigatoria(string pConfigName)
        {
            var configValue = ConfigurationManager.AppSettings[pConfigName];
            if (string.IsNullOrWhiteSpace(configValue))
                throw new ApplicationException($"Necessário configurar a chave '{pConfigName}' no App.config");
            return configValue;
        }

        public static string BuscarConnectionString(string pConfigName)
        {
            var connString = ConfigurationManager.ConnectionStrings[pConfigName].ConnectionString;
            if (string.IsNullOrWhiteSpace(connString))
                throw new ApplicationException($"Necessário configurar a chave '{pConfigName}' no App.config");
            return connString;
        }

    }
}