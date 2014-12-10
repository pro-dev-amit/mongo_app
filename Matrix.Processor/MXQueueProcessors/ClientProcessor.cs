using Matrix.Core.FrameworkCore;
using Matrix.DAL.BaseMongoRepositories;
using Matrix.Entities.MongoEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Processor.MXQueueProcessors
{
    public class ClientProcessor
    {
        IMXBusinessMongoRepository _bRepository;

        public ClientProcessor(IMXBusinessMongoRepository bRepository)
        {
            _bRepository = bRepository;
            _bRepository.IsProcessedByQueue = true;
        }

        public void ProcessClient(IMXEntity message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-----------------Processing now...-----------------");

            var client = message as Client;

            if (client != null)
            {
                var result = _bRepository.Insert<Client>(client);
                Console.WriteLine("New Client Created with Id : " + result);
            }

            Console.WriteLine("\n-----------------Processing Complete..-----------------");
            Console.ResetColor();
        }

    }//End of class
}
