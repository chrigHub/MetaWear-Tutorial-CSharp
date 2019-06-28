using MbientLab.MetaWear;
using MbientLab.MetaWear.Core;
using MbientLab.MetaWear.Data;
using MbientLab.MetaWear.Sensor;
using MbientLab.MetaWear.Sensor.GyroBmi160;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Integration;


namespace NetCoreExamples
{
    class OwnDataProcessor
    {
        static bool cont = true;
        static int thr = 1000;

        static ArduinoBridge a;

        static async Task RunAsync(string[] args)
        {
            var metawear = await ScanConnect.Connect(args[0]);

            Console.WriteLine($"Configuring {args[0]}...");

            var acc = metawear.GetModule<IAccelerometer>();

            // Initialise AD
            Console.Write("Port: ");
            string p = Console.ReadLine();
            int port = 7;
            Int32.TryParse(p, out port);
            a = new ArduinoBridge(port);
            Console.WriteLine("Bereit");

            acc.Configure(odr: 25f);
            double pathX = 0, pathY = 0;


            Acceleration currAcc, prevAcc;
            double s;
            double x = 0, y = 0, z = 0;
            double xAccAvg = 0, yAccAvg = 0, zAccAvg = 0;
            double roll = 0, pitch = 0; // Make Angles avaialbe to programm
            await acc.Acceleration.AddRouteAsync(source => source.Stream(data2 =>
            {
                currAcc = data2.Value<Acceleration>();
                x = currAcc.X * 9.80665f;
                y = currAcc.Y * 9.80665f;
                z = currAcc.Z * 9.80665f;


                // Angle Calc
                roll = (CalcTheta(x, y, z) * 180) / Math.PI;
                pitch = (CalcTheta(y, x, z) * 180) / Math.PI;
                String direction = "";
                if (roll > 0)
                    direction = "LEFT";
                if (roll < 0)
                    direction = "RIGHT";
                Console.WriteLine(String.Format("{0:0.#####} {1:0.#####} {2}", pitch, Math.Abs(roll), direction));

                // Acceleration workings


                prevAcc = currAcc;
            }));
            acc.Acceleration.Start();
            acc.Start();
            Task.Run(() => ListenForKeyEvents());
            
            while (cont)
            {
                /*
                var key = Console.ReadKey();
                try
                {
                    switch (key.Key)
                    {
                        case ConsoleKey.C:
                            cont = false; break;
                        case ConsoleKey.W: a.IncreasePower(); break;
                        case ConsoleKey.S: a.DecreasePower(); break;
                        case ConsoleKey.Q: a.Reset(); break;
                        case ConsoleKey.Spacebar: a.SetPower(0); break;
                        default: thr = a.Throttle;break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                */
                a.Move(Convert.ToInt32(-pitch * 1.5), Convert.ToInt32(-roll * 1.5));
                Console.WriteLine(Convert.ToInt32(roll) + "x");
                Console.WriteLine(Convert.ToInt32(-pitch) + "y");
                Console.WriteLine(a.Status);
                await Task.Delay(50);
            }
                
                // Console.WriteLine(String.Format("{0} {1} {2}}", xgy, ygy, zgy));
                //Console.WriteLine(String.Format("{0:0.####}", (0.5*xacc * Math.Pow(0.1, 2)*1000) + (0.5*yacc * Math.Pow(0.1, 2)*1000) + (0.5*zacc * Math.Pow(0.1, 2)*1000)));

                //Console.WriteLine(String.Format("{0:0.##} {1:0.##} {2:0.##}", 0.5*xacc * Math.Pow(0.1, 2)*1000, 0.5*yacc * Math.Pow(0.1, 2)*1000, 0.5*zacc * Math.Pow(0.1, 2)*1000));
                //   Console.WriteLine(String.Format("{0:0.##} {1:0.##} {2:0.##} => {3:0.##} {4:0.##} {5:0.##}", xacc, yacc, zacc, (0.5*xacc*Math.Pow(0.1,2))*1000, (0.5 * yacc * Math.Pow(0.1, 2)), (0.5 * zacc * Math.Pow(0.1, 2))*1000));
                  //Console.WriteLine(String.Format("{0:0.#####} {1:0.#####} {2:0.#####}", xacc, yacc, zacc));

                //####### ANGLE CALCULATION
                /*double Ax = (CalcAx(xacc, yacc, zacc) * 180) / Math.PI;
                double Ay = (CalcAy(xacc, yacc, zacc) * 180) / Math.PI;
                double Az = (CalcAz(xacc, yacc, zacc) * 180) / Math.PI;
                String direction = "";
                 if (Ax > 0)
                     direction = "LEFT";
                 else if (Ax < 0)
                     direction = "RIGHT";
                 Console.WriteLine(String.Format("{0:0.#####} {1:0.#####} {2:0.#####}" + direction, Ay, Math.Abs(Ax), Az));*/
                
                //####### PATH CALCULATION
                // if (z > 55)
                // {
                    

                    //  Console.WriteLine(String.Format("Vals: {0:0.#####} {1:0.#####} {2:0.#####} {3:0.#####} {4:0.####}", xVals[z - 2], xVals[z - 1], (Math.Abs(secVals[z - 2] - secVals[z-3])) / 1000.0, (Math.Abs(secVals[z-1] - secVals[z - 3])) / 1000.0, z)); 


                    //Console.WriteLine((secVals[z - 1] - secVals[z - 2]) / 1000.0);
                    // (secVals[z] - secVals[z - 1]) / 1000
                // }
                
            
            acc.Stop();
            //Console.WriteLine(x1);
        }

        public static async void ListenForKeyEvents()
        {
            while (cont) { 
                var key = Console.ReadKey();
                try
                {
                    switch (key.Key)
                    {
                        case ConsoleKey.C:
                            cont = false; break;
                        case ConsoleKey.W: a.IncreasePower(); break;
                        case ConsoleKey.S: a.DecreasePower(); break;
                        case ConsoleKey.Q: a.Reset(); break;
                        case ConsoleKey.Spacebar: a.SetPower(0); break;
                        default: thr = a.Throttle; break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static double CalcTheta(double a, double b, double c)
        {
            return Math.Atan(a / (Math.Sqrt(Math.Pow(b, 2) + Math.Pow(c, 2))));
        }

        private static double CalcAy(double x, double y, double z)
        {
            return Math.Atan(y / (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(z, 2))));
        }

        private static double CalcAz(double x, double y, double z)
        {
            return Math.Atan(z / (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2))));
        }

        private static readonly DateTime Jan1st1970 = new DateTime
    (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

        private static double CalcInterval(double val1, double val2, double t1, double t2)
        {
            return SimpsonRule.IntegrateThreePoint(x => ((val2-val1)/(t2-t1))*x+val2, t1, t2);
            /*double veloc2 = SimpsonRule.IntegrateThreePoint(x => ((val3-val2)/(t3-t2))*x, t2, t3);
            double path = SimpsonRule.IntegrateThreePoint(x => (veloc1+veloc2)/2.0, t1, t3);
           // Console.WriteLine(String.Format("{0:0.#####} - {1:0.#####} =>  {2:0.#####} ", veloc1, veloc2, path));

            return path;
        */}

    }
}
