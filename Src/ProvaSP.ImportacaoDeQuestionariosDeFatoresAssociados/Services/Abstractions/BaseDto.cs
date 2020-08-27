using System.Collections.Generic;
using System.Linq;

namespace ImportacaoDeQuestionariosSME.Services.Abstractions
{
    public abstract class BaseDto
    {
        public BaseDto()
        {
            Erros = new List<string>();
        }

        public ICollection<string> Erros { get; private set; }

        public void AddErro(string erro) => Erros.Add(erro);

        public bool IsValid() => !Erros?.Any() ?? true;
    }
}