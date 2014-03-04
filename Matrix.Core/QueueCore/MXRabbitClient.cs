using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.QueueCore
{
    /// <summary>
    /// RabbitMQ client. This is a singleton; check out the IoC container registrations
    /// </summary>
    public class MXRabbitClient : IQueueClient
    {
        static string _connectionString;

        static Lazy<IBus> _bus = new Lazy<IBus>(() => RabbitHutch.CreateBus(_connectionString));

        static MXRabbitClient()
        {
            _connectionString = ConfigurationManager.AppSettings["rabbitMQConnectionString"];
        }
                
        public MXRabbitClient() 
        { 
        
        }

        public IBus Bus
        {
            get
            {
                return _bus.Value;
            }
        }

        public void Dispose()
        {
            if (_bus.Value != null) _bus.Value.Dispose();
        }
    }
}
