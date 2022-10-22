using DKZKV.Kafka.Abstractions;
using DKZKV.Kafka.Abstractions.Consumer;
using DKZKV.Kafka.Abstractions.Producer;
using DKZKV.Kafka.Abstractions.Serialization;
using DKZKV.Kafka.Consumer;
using DKZKV.Kafka.Consumer.Middleware;
using DKZKV.Kafka.Producer;
using DKZKV.Kafka.Producer.Middleware;
using DKZKV.Kafka.Serialization;
using DKZKV.Kafka.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DKZKV.Kafka;

internal class KafkaConfigurator : IKafkaConfigurator
{
    private readonly IServiceCollection _serviceCollection;

    public KafkaConfigurator(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
        serviceCollection.AddSingleton<KafkaReadinessClient>();
        _serviceCollection.AddSingleton<ProducerInstanceProvider>();
    }

    private string _groupId;

    public IKafkaConfigurator SetConsumersGroupId(string groupId)
    {
        _groupId = groupId;
        return this;
    }

    public IKafkaConfigurator AddConsumer<TConsumer, TMessage>(string topic, int batchSize)
        where TConsumer : class, IKafkaConsumer<TMessage>
        where TMessage : class, IKafkaMessage
    {
        var consumerSettings = new KafkaConsumerSettings<TMessage>
        {
            TopicName = topic,
            BatchSize = batchSize,
            GroupId = _groupId
        };
        consumerSettings.Validate();
        AddConsumer<TConsumer, TMessage>(consumerSettings);
        return this;
    }

    public IKafkaConfigurator AddConsumer<TConsumer, TMessage>(Action<IKafkaConsumerSettings<TMessage>> settings)
        where TConsumer : class, IKafkaConsumer<TMessage>
        where TMessage : class, IKafkaMessage
    {
        var consumerSettings = new KafkaConsumerSettings<TMessage>();
        settings.Invoke(consumerSettings);
        if (string.IsNullOrEmpty(consumerSettings.GroupId))
            consumerSettings.GroupId = _groupId;

        consumerSettings.Validate();
        AddConsumer<TConsumer, TMessage>(consumerSettings);
        return this;
    }

    public IKafkaConfigurator SetConsumerDeserializer(IKafkaDeserializer deserializer = null)
    {
        if (deserializer is null)
            _serviceCollection.TryAddSingleton<IKafkaDeserializer>(new DefaultJsonDeserializer());
        else
            _serviceCollection.TryAddSingleton(deserializer);
        return this;
    }

    public IKafkaConfigurator SetGlobalProducersSettings(Action<IKafkaGlobalProducerSettings> settings)
    {
        var consumerSettings = new KafkaGlobalProducerSettings();
        settings.Invoke(consumerSettings);
        _serviceCollection.TryAddSingleton<IKafkaGlobalProducerSettings>(consumerSettings);
        return this;
    }

    public void SetGlobalConsumersSettings()
    {
        var consumerSettings = new KafkaGlobalProducerSettings();
        consumerSettings.Validate();
        _serviceCollection.TryAddSingleton<IKafkaGlobalProducerSettings>(consumerSettings);
    }

    public IKafkaConfigurator AddProducer<TMessage>(string topic) where TMessage : class, IKafkaMessage
    {
        AddProducer(new KafkaProducerSettings<TMessage>(topic));
        return this;
    }

    public IKafkaConfigurator AddProducer<TMessage>(string topic, Func<TMessage, object> getKey) where TMessage : class, IKafkaMessage
    {
        AddProducer(new KafkaProducerSettings<TMessage>(topic, getKey));
        return this;
    }

    public IKafkaConfigurator AddConsumerMiddleware<TMiddleware>() where TMiddleware : KafkaConsumerMiddleware
    {
        _serviceCollection.AddSingleton<KafkaConsumerMiddleware, TMiddleware>();
        return this;
    }

    public IKafkaConfigurator AddProducerMiddleware<TMiddleware>() where TMiddleware : KafkaProducerMiddleware
    {
        _serviceCollection.AddSingleton<KafkaProducerMiddleware, TMiddleware>();
        return this;
    }

    public IKafkaConfigurator SetProducerSerializer(IKafkaSerializer serializer = null)
    {
        if (serializer is null)
            _serviceCollection.TryAddSingleton<IKafkaSerializer>(new DefaultJsonSerializer());
        else
            _serviceCollection.TryAddSingleton(serializer);
        return this;
    }

    internal void AddExecutorMiddlewares()
    {
        _serviceCollection.AddSingleton<KafkaProducerMiddleware, ProducerMiddleware>();

        _serviceCollection
            .AddSingleton<KafkaProducerMiddlewareExecutor>()
            .AddSingleton<KafkaProducerMiddleware>(provider => provider.GetRequiredService<KafkaProducerMiddlewareExecutor>());

        _serviceCollection.AddSingleton<KafkaConsumerMiddleware, ConsumerMiddleware>();
        _serviceCollection.AddSingleton<KafkaConsumerMiddlewareExecutor>();
    }

    private void AddProducer<TMessage>(KafkaProducerSettings<TMessage> settings) where TMessage : class, IKafkaMessage
    {
        _serviceCollection.AddSingleton(settings);
        _serviceCollection.AddSingleton<ProducerWrapper<TMessage>>();
        _serviceCollection.AddSingleton<IKafkaProducer<TMessage>, KafkaProducer<TMessage>>();
    }

    private void AddConsumer<TConsumer, TMessage>(KafkaConsumerSettings<TMessage> settings)
        where TConsumer : class, IKafkaConsumer<TMessage>
        where TMessage : class, IKafkaMessage
    {
        _serviceCollection.AddSingleton(settings);
        _serviceCollection.AddSingleton<ConsumeContextFactory<TMessage>>();
        _serviceCollection.AddSingleton<ConsumerFactory<TMessage>>();
        _serviceCollection.AddSingleton<IKafkaConsumer<TMessage>, TConsumer>();
        _serviceCollection.AddHostedService<KafkaConsumerService<TMessage>>();
    }
}