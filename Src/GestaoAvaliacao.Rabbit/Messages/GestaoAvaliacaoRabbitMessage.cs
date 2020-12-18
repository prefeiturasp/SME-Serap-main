using System;

namespace GestaoAvaliacao.Rabbit.Messages
{
    public class GestaoAvaliacaoRabbitMessage
    {
        public DateTime CreateDate { get; private set; }
        public object Data { get; private set; }
        public Guid UsuId { get; private set; }

        public GestaoAvaliacaoRabbitMessage(object data, Guid usuId)
        {
            CreateDate = DateTime.Now;
            Data = data;
            UsuId = usuId;
        }
    }
}