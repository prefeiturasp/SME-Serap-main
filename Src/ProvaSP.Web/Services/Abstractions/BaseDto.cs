using System;
using System.Collections.Generic;
using System.Linq;

namespace ProvaSP.Web.Services.Abstractions
{
    public abstract class BaseDto
    {
        public bool Valid => !ErrorMessages?.Any() ?? true;
        public List<string> ErrorMessages { get; private set; }

        public BaseDto()
        {
            ErrorMessages = new List<string>();
        }

        public void AddErrorMessage(string message) => ErrorMessages.Add(message);

        public void AddErrorMessages(IEnumerable<string> messages) => ErrorMessages.AddRange(messages);

        public void AddErrorMessage(Exception exception) => ErrorMessages.Add(exception.InnerException?.Message ?? exception.Message);
    }
}