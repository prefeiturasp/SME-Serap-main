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
        where TMessageData : class
    {
        private IModel _model;

        public void Close()
        {
            if (_model.IsClosed) return;
            _model.Close();
        }

        public Task<bool> PublishAsync(TMessageData data, Guid usuId)
        {
            var message = new GestaoAvaliacaoRabbitMessage(data, usuId);
            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);
            var exchange = ConfigurationManager.AppSettings["GestaoAvaliacaoRabbitSettings_ExchangeGestaoAvaliacao"];
            Model.BasicPublish(exchange, "*", null, body);
            return Task.FromResult(true);
        }

        protected IModel Model
        {
            get
            {
                if (_model is null) CreateModel();
                return _model;
            }
        }

        private void CreateModel()
        {
            var factory = new ConnectionFactory
            {
                HostName = ConfigurationManager.AppSettings["GestaoAvaliacaoRabbitSettings_HostName"],
                UserName = ConfigurationManager.AppSettings["GestaoAvaliacaoRabbitSettings_UserName"],
                Password = ConfigurationManager.AppSettings["GestaoAvaliacaoRabbitSettings_Password"],
                VirtualHost = ConfigurationManager.AppSettings["GestaoAvaliacaoRabbitSettings_VirtualHost"],
            };

            var rabbitConnection = factory.CreateConnection();
            _model = rabbitConnection.CreateModel();

            var exchange = ConfigurationManager.AppSettings["GestaoAvaliacaoRabbitSettings_ExchangeGestaoAvaliacao"];
            var queue = ConfigurationManager.AppSettings["GestaoAvaliacaoRabbitSettings_QueueName"];

            _model.ExchangeDeclare(exchange, ExchangeType.Topic);
            _model.QueueDeclare(queue, false, false, false, null);
            _model.QueueBind(queue, exchange, "*");
        }
    }
}