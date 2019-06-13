using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            //Usage of ArduinoBridge

            Console.Write("Port: ");
            string p = Console.ReadLine();
            int port = 7;
            Int32.TryParse(p, out port);
            ArduinoBridge a = new ArduinoBridge(port);
            Console.WriteLine("Bereit");
            bool exit = true;

            while (true)
            {
                exit = Run(a).Result;
            }
        }

        static async Task<bool> Run(ArduinoBridge a)
        {
            var key = Console.ReadKey();
            try
            {
                switch (key.Key)
                {
                    case ConsoleKey.C:
                        a.Connected = false; return true;
                    case ConsoleKey.X: a.Connected = true; break;
                    case ConsoleKey.W: a.IncreasePower(); break;
                    case ConsoleKey.S: a.DecreasePower(); break;
                    case ConsoleKey.A: a.Rotate(-1); break;
                    case ConsoleKey.D: a.Rotate(1); break;
                    case ConsoleKey.UpArrow: a.MoveForward(); break;
                    case ConsoleKey.DownArrow: a.MoveBackward(); break;
                    case ConsoleKey.LeftArrow: a.MoveLeft(); break;
                    case ConsoleKey.RightArrow: a.MoveRight(); break;
                    case ConsoleKey.Q: a.Reset(); break;
                    case ConsoleKey.Spacebar: a.SetPower(0);break;
                }
                Console.WriteLine(a.Status);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            await Task.Delay(10);
            return false;
        }
    }
}
