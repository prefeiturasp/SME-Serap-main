using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;

namespace GestaoAvaliacao.Business
{
    public class BaseTextBusiness : IBaseTextBusiness
	{
        private readonly IBaseTextRepository _baseTextRepository;

        public BaseTextBusiness(IBaseTextRepository baseTextRepository)
        {
            _baseTextRepository = baseTextRepository;
        }

        public string GetBaxeTestByItemId(long itemId) 
        {
            var baseTexto = _baseTextRepository.GetByItemId(itemId);

            return string.IsNullOrWhiteSpace(baseTexto) 
                ? "<p>O item não possui texto base.</p>" 
                : baseTexto;
		}
	}
}
