using System;

namespace GestaoAvaliacao.FGVIntegration.Exceptions
{
    public class AutenticacaoException : ApplicationException
    {
        public AutenticacaoException() : base() { }

        public AutenticacaoException(string message) : base(message) { }

        public AutenticacaoException(string message, Exception innerException) : base(message, innerException) { }
    }
}