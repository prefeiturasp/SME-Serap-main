﻿using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class ArquivoResultadoPsp : EntityBase
    {
        public ArquivoResultadoPsp()
        {

        }

        public string NomeArquivo { get; set; }
        public string NomeOriginalArquivo { get; set; }
        public int CodigoTipoResultado { get; set; }

    }
}