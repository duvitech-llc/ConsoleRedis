using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using StackExchange.Redis;
using System.Collections.Generic;

namespace ConsoleRedis
{
    class Program
    {
        // Redis Connection string info
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = "peerserver.redis.cache.windows.net:6380,password=q8bGecefDYvcdVsGA2rMqVnAuRE4F83ZGBgJRURfnKM=,ssl=True,abortConnect=False"; /* ConfigurationManager.AppSettings["CacheConnection"].ToString();*/
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        static void sub_handler(RedisChannel channel, RedisValue val)
        {
            if (!channel.IsNullOrEmpty)
            {
                Console.Write(channel.ToString());

                if (val.HasValue)
                {
                    Console.Write(" => " + val.ToString());
                }

                Console.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            IDatabase cache = lazyConnection.Value.GetDatabase();
            var task = cache.Ping();
            watch.Stop();
            Console.WriteLine("Ping & Connection took: " + watch.ElapsedMilliseconds + " ms");
            Console.WriteLine("Ping was: " + task.Milliseconds + " ms");
            
            task = cache.Ping();
            
            Console.WriteLine("Ping took: " + task.Milliseconds + " ms");
            var pub = lazyConnection.Value.GetSubscriber();
            var sub = lazyConnection.Value.GetSubscriber();
            
            sub.Subscribe("ar_dev_ch", sub_handler);
            Thread.Sleep(1000);
            pub.Publish("ar_dev_ch", "Hello Work");
            while (true)
            {
                Thread.Sleep(10);
            }
        }

       
    }
}
