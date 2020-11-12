using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ProducerConsumerProblemInCSharp2
{
    class Program
    {
        private static int[] _buffer = new int[5];
        private static int _bufferIndex = 0;
        private static int _bufferSize = 0;
        private static bool _done = false;

        private static object _locker = new object();

        static void Main(string[] args)
        {
            Thread producer = new Thread(Produce);
            producer.Start();

            List<Thread> threads = new List<Thread>();
            Enumerable.Range(1, 5)
                .ToList()
                .ForEach(id =>
                {
                    var thread = new Thread(new ParameterizedThreadStart(Consumer));
                    threads.Add(thread);
                    thread.Start(id);
                });

            threads.ForEach(t => t.Join());
            Console.WriteLine("finished all activities");
        }

        static void Consumer(object id)
        {
            while (!(_done && _bufferSize == 0))
            {
                if (_bufferSize > 0)
                {
                    int item;
                    lock (_locker)
                    {
                        if (_bufferSize > 0)
                        {
                            item = _buffer[(_bufferIndex - _bufferSize + _buffer.Length) % _buffer.Length];
                            _bufferSize--;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    // put this in locked area to have an ordered output
                    Console.Out.WriteLine($"{id.ToString()} read {item}");
                    Console.Out.Flush();
                }

                Thread.Sleep(100);
            }
        }

        static void Produce()
        {
            int i = 1;
            while (i <= 100)
            {
                lock (_locker)
                {
                    if (_bufferSize < _buffer.Length)
                    {
                        _buffer[_bufferIndex] = i;

                        _bufferSize++;
                        _bufferIndex = (_bufferIndex + 1) % _buffer.Length;
                        i++;
                    }
                }
            }

            lock (_locker)
            {
                _done = true;
            }
        }
    }
}
