using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class DadosArquivoResultadoPspMap : EntityBaseMap<DadosArquivoResultadoPsp>
    {
        public DadosArquivoResultadoPspMap()
        {
            ToTable("DadosArquivoResultadoPsp");

            Property(p => p.ArquivoResultadoId)
               .IsRequired()
               .HasColumnType("bigint");            

            Property(p => p.TextoLinhaArquivo)
               .IsMaxLength()
               .HasColumnType("varchar");            
        }
    }
}
