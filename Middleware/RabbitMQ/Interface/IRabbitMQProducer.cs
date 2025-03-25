using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middleware.RabbitMQ.Interface
{
    public interface IRabbitMQProducer
    {
        void PublishMessage(string email, string token);
    }
}
