using DKZKV.Kafka.Abstractions;
using DKZKV.Kafka.Exceptions;
using DKZKV.Kafka.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DKZKV.Kafka
{
    /// <summary>
    /// Extension for kafka
    /// </summary>
    public static class KafkaExtension
    {
        /// <summary>
        /// Plug in kafka for producers and consumers setup
        /// </summary>
        /// <param name="services"></param>
        /// <param name="conf"></param>
        /// <param name="configuration"></param>
        /// <exception cref="KafkaSettingsExceptions">If section is not defined or settings are invalid</exception>
        public static void AddKafka(this IServiceCollection services, Action<IKafkaConfigurator> conf, IConfiguration configuration)
        {
            services.AddOptions<KafkaConnectionSettings>().Bind(GetConfigurationSection(configuration));
            services.ConfigureKafka(conf);
        }

        /// <summary>
        /// Plug in kafka for producers and consumers setup
        /// </summary>
        /// <param name="services"></param>
        /// <param name="conf">setup with provider</param>
        /// <param name="configuration"></param>
        /// <exception cref="KafkaSettingsExceptions">If section is not defined or settings are invalid</exception>
        public static void AddKafka(this IServiceCollection services, Action<IServiceProvider, IKafkaConfigurator> conf, IConfiguration configuration)
        {
            services.AddOptions<KafkaConnectionSettings>().Bind(GetConfigurationSection(configuration));
            services.ConfigureKafkaWithBuilder(conf);
        }

        /// <summary>
        /// Plug in kafka for producers and consumers setup
        /// </summary>
        /// <param name="services"></param>
        /// <param name="conf"></param>
        /// <param name="settingsConfiguration"></param>
        /// <exception cref="KafkaSettingsExceptions">If settings are invalid</exception>
        public static void AddKafka(this IServiceCollection services, Action<IKafkaConfigurator> conf, Action<KafkaConnectionSettings> settingsConfiguration)
        {
            services.AddSingleton(Options.Create(GetConnectionSettings(settingsConfiguration)));
            services.ConfigureKafka(conf);
        }

        /// <summary>
        /// Plug in kafka for producers and consumers setup
        /// </summary>
        /// <param name="services"></param>
        /// <param name="conf"></param>
        /// <param name="settingsConfiguration"></param>
        /// <exception cref="KafkaSettingsExceptions">If settings are invalid</exception>
        public static void AddKafka(this IServiceCollection services, Action<IServiceProvider, IKafkaConfigurator> conf,
            Action<KafkaConnectionSettings> settingsConfiguration)
        {
            services.AddSingleton(Options.Create(GetConnectionSettings(settingsConfiguration)));
            services.ConfigureKafkaWithBuilder(conf);
        }

        private static IConfigurationSection GetConfigurationSection(IConfiguration configuration)
        {
            var configurationSection = configuration.GetSection(nameof(KafkaConnectionSettings));
            var kafkaSettings = configurationSection.Get<KafkaConnectionSettings>();
            if (kafkaSettings is null)
                throw new KafkaSettingsExceptions($"{nameof(KafkaConnectionSettings)} section is not defined");

            if (string.IsNullOrEmpty(kafkaSettings.Servers))
                throw new KafkaSettingsExceptions($" Servers in {nameof(KafkaConnectionSettings)} is not defined");
            return configurationSection;
        }

        private static KafkaConnectionSettings GetConnectionSettings(Action<KafkaConnectionSettings> settingsConfiguration)
        {
            var kafkaSettings = new KafkaConnectionSettings();
            settingsConfiguration.Invoke(kafkaSettings);
            if (string.IsNullOrEmpty(kafkaSettings.Servers))
                throw new KafkaSettingsExceptions($" Servers in {nameof(KafkaConnectionSettings)} is not defined");
            return kafkaSettings;
        }

        private static void ConfigureKafkaWithBuilder(this IServiceCollection services, Action<IServiceProvider, IKafkaConfigurator> conf)
        {
            var configurator = new KafkaConfigurator(services);
            conf.Invoke(services.BuildServiceProvider(), configurator);
            configurator.SetProducerSerializer();
            configurator.SetConsumerDeserializer();
            configurator.AddExecutorMiddlewares();
            configurator.SetGlobalConsumersSettings();
        }

        private static void ConfigureKafka(this IServiceCollection services, Action<IKafkaConfigurator> conf)
        {
            var configurator = new KafkaConfigurator(services);
            conf.Invoke(configurator);
            configurator.SetProducerSerializer();
            configurator.SetConsumerDeserializer();
            configurator.AddExecutorMiddlewares();
            configurator.SetGlobalConsumersSettings();
        }
    }
}