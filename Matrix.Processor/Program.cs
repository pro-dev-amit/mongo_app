using Matrix.Core.FrameworkCore;
using Matrix.Core.QueueCore;
using Matrix.Entities.MongoEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Processor
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bus = new MXRabbitClient().Bus)
            {
                bus.Subscribe<IMXEntity>("ClientType", HandleMessage);

                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("Listening for messages. Hit <return> to quit.");

                Console.ResetColor();

                Console.ReadLine();
            }
        }

        static void HandleMessage(IMXEntity message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-----------------Processing now...-----------------");



            var client = message as Client;

            var repository = new MXMongoRepository();
            if (client != null)
            {
                var result = repository.Insert<Client>(client);
                Console.WriteLine("New Client Created with Id : " + result);
            }



            Console.WriteLine("\n-----------------Processing Complete..-----------------");
            Console.ResetColor();
        }
    }
}
