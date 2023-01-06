using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class ArquivoResultadoPspMap : EntityBaseMap<ArquivoResultadoPsp>
    {
        public ArquivoResultadoPspMap()
        {
            ToTable("ArquivoResultadoPsp");

            Property(p => p.FileId)
               .IsRequired()
               .HasColumnType("bigint");

            Property(p => p.CodigoTipoResultado)
                .IsRequired()
                .HasColumnType("bigint");

            Property(p => p.NomeArquivo)
               .HasMaxLength(500)
               .HasColumnType("varchar");

            Property(p => p.NomeOriginalArquivo)
                .HasMaxLength(500)
                .HasColumnType("varchar");
        }
    }
}
