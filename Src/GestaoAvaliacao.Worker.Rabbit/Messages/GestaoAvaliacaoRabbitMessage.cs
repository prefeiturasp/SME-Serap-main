using System;
using System.Text.Json;

namespace GestaoAvaliacao.Worker.Rabbit.Messages
{
    public sealed class GestaoAvaliacaoRabbitMessage
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

        public TMessageData GetData<TMessageData>()
            where TMessageData : class 
            => JsonSerializer.Deserialize<TMessageData>(Data.ToString());
    }
}