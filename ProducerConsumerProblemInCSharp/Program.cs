using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProducerConsumerProblemInCSharp
{
    class Program
    {
        private static readonly BlockingCollection<int> _buffer = 
            new BlockingCollection<int>(5);
        static void Main()
        {
            Thread producer = new Thread(Produce);
            producer.Start();

            Enumerable.Range(1, 5)
                .ToList()
                .ForEach(id => {
                    var thread = new Thread(new ParameterizedThreadStart(Consumer));
                    thread.Start(id);
                });
        }

        static void Consumer(object id)
        {
            foreach (var item in _buffer.GetConsumingEnumerable())
            {
                Console.Out.WriteLine($"{id.ToString()} read {item}");
                Console.Out.Flush();
            }
            Thread.Sleep(100);
        }

        static void Produce()
        {
            Enumerable
                .Range(1, 100)
                .ToList()
                .ForEach(x => _buffer.Add(x));
            _buffer.CompleteAdding();
        }
    }
}
