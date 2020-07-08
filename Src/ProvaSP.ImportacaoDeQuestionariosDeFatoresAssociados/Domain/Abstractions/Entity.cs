using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Domain.Abstractions
{
    public abstract class Entity
    {
        public Entity()
        {
            Erros = new List<string>();
        }

        public ICollection<string> Erros { get; private set; }

        public void AddErro(string erro) => Erros.Add(erro);

        public bool IsValid() => !Erros?.Any() ?? true;
    }
}