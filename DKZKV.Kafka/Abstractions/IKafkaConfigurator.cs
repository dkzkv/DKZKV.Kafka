using DKZKV.Kafka.Abstractions.Consumer;
using DKZKV.Kafka.Abstractions.Producer;
using DKZKV.Kafka.Abstractions.Serialization;
using DKZKV.Kafka.Settings;

namespace DKZKV.Kafka.Abstractions;

/// <summary>
/// Consumers producers setup
/// </summary>
public interface IKafkaConfigurator
{
    #region Consumer
    /// <summary>
    /// Set consumers group name for all consumers (can be overridden in extended consumer settings)
    /// </summary>
    /// <param name="groupId"></param>
    /// <returns></returns>
    public IKafkaConfigurator SetConsumersGroupId(string groupId);
    
    /// <summary>
    /// Add consumer
    /// </summary>
    /// <param name="topic">name of topic</param>
    /// <param name="batchSize">batch size (incoming message batch can be less than batch size)</param>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <returns></returns>
    public IKafkaConfigurator AddConsumer<TConsumer, TMessage>(string topic, int batchSize)
        where TConsumer : class, IKafkaConsumer<TMessage>
        where TMessage : class, IKafkaMessage;
    
    /// <summary>
    /// Add consumer with extended settings
    /// </summary>
    /// <param name="settings"></param>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <returns></returns>
    public IKafkaConfigurator AddConsumer<TConsumer, TMessage>(Action<IKafkaConsumerSettings<TMessage>> settings)
        where TConsumer : class, IKafkaConsumer<TMessage>
        where TMessage : class, IKafkaMessage;
    
    /// <summary>
    /// Override message deserialization for consumer
    /// </summary>
    /// <param name="deserializer">Deserializer, default = DefaultJsonDeserializer (Utf8Json)</param>
    /// <returns></returns>
    public IKafkaConfigurator SetConsumerDeserializer(IKafkaDeserializer deserializer = null);
    
    /// <summary>
    /// Add consumer middleware
    /// </summary>
    /// <typeparam name="TMiddleware"></typeparam>
    /// <returns></returns>
    IKafkaConfigurator AddConsumerMiddleware<TMiddleware>() where TMiddleware : KafkaConsumerMiddleware;
    #endregion

    #region Producer

    /// <summary>
    /// Set up global producer settings
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    public IKafkaConfigurator SetGlobalProducersSettings(Action<IKafkaGlobalProducerSettings> settings);
    
    /// <summary>
    /// Add producer
    /// </summary>
    /// <param name="topic">topic name</param>
    /// <typeparam name="TMessage">message type</typeparam>
    /// <returns></returns>
    public IKafkaConfigurator AddProducer<TMessage>(string topic) where TMessage : class, IKafkaMessage;

    /// <summary>
    /// Add producer with partitioning
    /// </summary>
    /// <param name="topic">topic name</param>
    /// <param name="getKey">partition key function</param>
    /// <typeparam name="TMessage">message type</typeparam>
    /// <returns></returns>
    public IKafkaConfigurator AddProducer<TMessage>(string topic, Func<TMessage, object> getKey) where TMessage : class, IKafkaMessage;
    
    /// <summary>
    /// Override producer serialization
    /// </summary>
    /// <param name="serializer">Serializer, default = DefaultJsonSerializer (Utf8Json)</param>
    /// <returns></returns>
    public IKafkaConfigurator SetProducerSerializer(IKafkaSerializer serializer = null);

    /// <summary>
    /// Add producer middleware
    /// </summary>
    /// <typeparam name="TMiddleware">middleware type</typeparam>
    /// <returns></returns>
    public IKafkaConfigurator AddProducerMiddleware<TMiddleware>() where TMiddleware : KafkaProducerMiddleware;
    #endregion
}