using System;
using System.Configuration;

namespace Purchasing.Jobs.UpdateOrderIndexes
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting v2 run at {0}", DateTime.Now);
            Console.WriteLine("Connecting using {0}", ConfigurationManager.ConnectionStrings["MainDb"].ConnectionString);
        }
    }
}
