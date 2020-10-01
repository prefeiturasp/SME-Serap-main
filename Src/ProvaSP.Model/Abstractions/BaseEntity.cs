using System.Collections.Generic;
using System.Linq;

namespace ProvaSP.Model.Abstractions
{
    public abstract class BaseEntity
    {
        public bool Valid => !ErrorMessages?.Any() ?? true;
        public List<string> ErrorMessages { get; private set; }

        public BaseEntity()
        {
            ErrorMessages = new List<string>();
        }

        protected void AddErrorMessage(string message) => ErrorMessages.Add(message);

        protected void AddErrorMessages(IEnumerable<string> messages) => ErrorMessages.AddRange(messages);
    }
}