using GestaoAvaliacao.Util;
using System;

namespace GestaoAvaliacao.Entities
{
    /// <summary>
    /// Objeto para os filtros de pesquisa do upload de arquivos
    /// </summary>
    public class FileFilter
    {
        public FileFilter()
        {
            if (StartDate != null && StartDate.Equals(DateTime.MinValue))
                StartDate = null;

            if (EndDate != null && EndDate.Equals(DateTime.MinValue))
                EndDate = null;
        }

        public virtual string Description { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual long OwnerId { get; set; }
        public virtual EnumFileType OwnerType { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual int CoreVisionId { get; set; }
        public virtual int CoreSystemId { get; set; }
        public virtual bool ShowLinks { get; set; }
    }
}
