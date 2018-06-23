using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootPartition
{
    class Program
    {
        static void Main(string[] args)
        {
            NativeApi native = new NativeApi();
            native.ListPartition();
            Console.ReadLine();
        }
    }
}
