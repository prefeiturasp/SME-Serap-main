﻿using GestaoAvaliacao.Worker.Rabbit.Messages;
using GestaoAvaliacao.Worker.Rabbit.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Rabbit.Consumers
{
    public abstract class GestaoAvaliacaoRabbitBaseConsumer<TMessageData>
        where TMessageData : class
    {
        private IModel _model;

        protected readonly IGestaoAvaliacaoRabbitSettings _gestaoAvaliacaoRabbitSettings;

        public GestaoAvaliacaoRabbitBaseConsumer(IGestaoAvaliacaoRabbitSettings gestaoAvaliacaoRabbitSettings)
        {
            _gestaoAvaliacaoRabbitSettings = gestaoAvaliacaoRabbitSettings;
            CreateModel();
        }

        public void Close()
        {
            if (_model.IsClosed) return;
            _model.Close();
        }

        protected async Task ConsumeFetchAsync(Func<TMessageData, CancellationToken, Task> onConsumingCallback, CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(Model);
            consumer.Received += async (ch, ea) =>
            {
                var data = GetMessageData(ea);
                Console.WriteLine("[ INFO ] Message received: {0}", ea.Body.ToString());
                await onConsumingCallback(data, cancellationToken);
                Model.BasicAck(ea.DeliveryTag, false);
            };

            Model.BasicConsume(_gestaoAvaliacaoRabbitSettings.QueueName, false, consumer);
        }

        protected IModel Model
        {
            get
            {
                if (_model is null || _model.IsClosed) CreateModel();
                return _model;
            }
        }

        private void CreateModel()
        {
            var factory = new ConnectionFactory
            {
                HostName = _gestaoAvaliacaoRabbitSettings.HostName,
                UserName = _gestaoAvaliacaoRabbitSettings.UserName,
                Password = _gestaoAvaliacaoRabbitSettings.Password,
                VirtualHost = _gestaoAvaliacaoRabbitSettings.VirtualHost
            };

            var rabbitConnection = factory.CreateConnection();
            _model = rabbitConnection.CreateModel();

            _model.ExchangeDeclare(_gestaoAvaliacaoRabbitSettings.ExchangeGestaoAvaliacao, ExchangeType.Topic, false);
            _model.QueueDeclare(_gestaoAvaliacaoRabbitSettings.QueueName, false, false, false, null);
            _model.QueueBind(_gestaoAvaliacaoRabbitSettings.QueueName, _gestaoAvaliacaoRabbitSettings.ExchangeGestaoAvaliacao, "*");
            _model.BasicQos(0, 5, true);
        }

        private TMessageData GetMessageData(BasicDeliverEventArgs basicDeliverEventArgs)
        {
            if (basicDeliverEventArgs is null) return null;
            var messageUtf8 = Encoding.UTF8.GetString(basicDeliverEventArgs.Body.Span);
            var message = JsonSerializer.Deserialize<GestaoAvaliacaoRabbitMessage>(messageUtf8);
            return message.GetData<TMessageData>();
        }
    }
}