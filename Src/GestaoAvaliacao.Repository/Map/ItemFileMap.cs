using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository.Map
{
    public class ItemFileMap : EntityBaseMap<ItemFile>
    {
        public ItemFileMap()
        {
            ToTable("ItemFile");

            HasOptional(p => p.Thumbnail)
                .WithMany()
                .HasForeignKey(p => p.Thumbnail_Id);
        }
    }
}
