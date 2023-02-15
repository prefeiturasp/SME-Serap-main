using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Util;
using System;

namespace GestaoAvaliacao.Entities
{
    public class DadosArquivoResultadoPsp : EntityBase
    {
        public DadosArquivoResultadoPsp()
        {

        }

        public DadosArquivoResultadoPsp(long arquivoResultadoId, string textoLinhaArquivo)
        {
            ArquivoResultadoId = arquivoResultadoId;
            TextoLinhaArquivo = textoLinhaArquivo;
            State = (byte)EnumStatusImportResulProvaSp.Pendente;
            CreateDate = UpdateDate = DateTime.Now;
        }

        public long ArquivoResultadoId { get; set; }
        public string TextoLinhaArquivo { get; set; }

        public void AlterarStatus(EnumStatusImportResulProvaSp status)
        {
            this.State = (byte)status;
        }

    }
}