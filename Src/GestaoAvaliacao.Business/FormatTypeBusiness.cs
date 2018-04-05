using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using System.Collections.Generic;

namespace GestaoAvaliacao.Business
{
    public class FormatTypeBusiness : IFormatTypeBusiness
    {
        private readonly IFormatTypeRepository formatTypeRepository;

        public FormatTypeBusiness(IFormatTypeRepository formatTypeRepository)
        {
            this.formatTypeRepository = formatTypeRepository;
        }

        #region Read

        public FormatType Get(long Id)
        {
            return formatTypeRepository.Get(Id);
        }

        public IEnumerable<FormatType> Load()
        {
            return formatTypeRepository.Load();
        }

        #endregion
    }
}
