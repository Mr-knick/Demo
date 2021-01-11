using System;
using System.Runtime.InteropServices;

namespace Beep
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool Beep(uint dwFreq, uint dwDuration);

        static void Main(string[] args)
        {
            Console.WriteLine("Testing PC speaker...");

            while (true)
            {
                //Beep(1000, 5000);
                //System.Threading.Thread.Sleep(500);
                //Beep(15000, 2000);
                //System.Threading.Thread.Sleep(500);
                //Beep(10000, 5000);
                //System.Threading.Thread.Sleep(500);
                Console.WriteLine("1000");
                Beep(1000, 3000);
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
                Console.WriteLine("3000");
                Beep(3000, 3000);
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
                Console.WriteLine("2000");
                Beep(2000, 3000);
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
                //Beep(440, 10000);
                //System.Threading.Thread.Sleep(1000);

            }
            Console.WriteLine("Testing complete.");
        }
    }
}
