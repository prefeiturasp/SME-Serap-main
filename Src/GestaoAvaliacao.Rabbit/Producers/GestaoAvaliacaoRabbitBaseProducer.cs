using GestaoAvaliacao.Rabbit.Messages;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Rabbit.Producers
{
    public abstract class GestaoAvaliacaoRabbitBaseProducer<TMessageData>
        where TMessageData : class    {
    
        public Task<bool> PublishAsync(TMessageData data, Guid usuId)
        {
            var message = new GestaoAvaliacaoRabbitMessage(data, usuId);
            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);
            var exchange = ConfigurationManager.AppSettings["GestaoAvaliacaoRabbitSettings_ExchangeGestaoAvaliacao"];
            var queue = ConfigurationManager.AppSettings["GestaoAvaliacaoRabbitSettings_QueueName"];

            var factory = new ConnectionFactory
            {
                HostName = ConfigurationManager.AppSettings["GestaoAvaliacaoRabbitSettings_HostName"],
                UserName = ConfigurationManager.AppSettings["GestaoAvaliacaoRabbitSettings_UserName"],
                Password = ConfigurationManager.AppSettings["GestaoAvaliacaoRabbitSettings_Password"],
                VirtualHost = ConfigurationManager.AppSettings["GestaoAvaliacaoRabbitSettings_VirtualHost"],
            };

            using (var conexaoRabbit = factory.CreateConnection())
            {
                using (IModel _channel = conexaoRabbit.CreateModel())
                {
                    _channel.ExchangeDeclare(exchange, ExchangeType.Topic);
                    _channel.QueueDeclare(queue, false, false, false, null);
                    _channel.BasicPublish(exchange, "*", null, body);
                }
            }

            return Task.FromResult(true);
        }    
    }
}