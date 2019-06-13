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
            ArduinoBridge a = new ArduinoBridge(port);
            Console.WriteLine("Bereit");

            acc.Configure(odr: 25f);
            double pathX = 0, pathY = 0;


            Acceleration currAcc, prevAcc;
            long[] secVals = new long[1000000];
            double[] xVals = new double[1000000]; double[] yVals = new double[1000000];
            double[] zVals = new double[1000000];
            // int i = 0;
            int z = 0;
            float xacc = 0, yacc = 0, zacc = 0;
            double xAccAvg = 0, yAccAvg = 0, zAccAvg = 0;

            await acc.Acceleration.AddRouteAsync(source => source.Stream(data2 =>
            {
                currAcc = data2.Value<Acceleration>();
                xacc = currAcc.X * 9.80665f;
                yacc = currAcc.Y* 9.80665f;
                zacc = currAcc.Z* 9.80665f;

                xVals[z] = xacc;
                yVals[z] = yacc;
                zVals[z] = zacc;

                if (z >= 5)
                {
                    

                   Console.WriteLine("P: " + (pathX + pathY));
                   // Console.WriteLine(String.Format("X:{0:0.##} Y:{1:0.##} Z:{2:0.##}", pathX, pathY, pathZ));
                }

                if (z < 50)
                {
                    xAccAvg += xacc;
                    yAccAvg += yacc;
                    zAccAvg += zacc;
                }
                if (z == 50)
                {
                    xAccAvg = xAccAvg / 50.0;
                    yAccAvg = yAccAvg / 50.0;
                    zAccAvg = zAccAvg / 50.0;

                }
                double Ax = (CalcAx(xacc, yacc, zacc) * 180) / Math.PI;
                double Ay = (CalcAy(xacc, yacc, zacc) * 180) / Math.PI;
                a.MoveLR(Convert.ToInt32(Ax));
                a.MoveFB(Convert.ToInt32(-Ay));
                Console.WriteLine(a.Status);
                String direction = "";
                if (Ax > 0)
                    direction = "LEFT";
                else if (Ax < 0)
                    direction = "RIGHT";
                Console.WriteLine(String.Format("{0:0.#####} {1:0.#####} " + direction, Ay, Math.Abs(Ax)));

                // zacc = (float)(Math.Truncate((double)array2.Z * 10.0) / 10.0);
                // yacc  = (float)(Math.Truncate((double)array2.Y * 10.0) / 10.0);
                // xacc = (float)(Math.Truncate((double)array2.X * 10.0) / 10.0);
                //  Console.WriteLine(j++ + ": " + array2);
            }));
            acc.Acceleration.Start();
            acc.Start();
            z = 0;
              //TODO Synchronise gyro and acc to get matching data
            for (int i = 0; i < 10000; i++)

            {
                await Task.Delay(50);
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
                if (z > 55)
                {
                    

                    //  Console.WriteLine(String.Format("Vals: {0:0.#####} {1:0.#####} {2:0.#####} {3:0.#####} {4:0.####}", xVals[z - 2], xVals[z - 1], (Math.Abs(secVals[z - 2] - secVals[z-3])) / 1000.0, (Math.Abs(secVals[z-1] - secVals[z - 3])) / 1000.0, z)); 


                    //Console.WriteLine((secVals[z - 1] - secVals[z - 2]) / 1000.0);
                    // (secVals[z] - secVals[z - 1]) / 1000
                }
                
            }
            acc.Stop();
            Console.WriteLine();
            //Console.WriteLine(x1);
            Console.ReadLine();
        }

        private static double CalcAx(double x, double y, double z)
        {
            return Math.Atan(x / (Math.Sqrt(Math.Pow(y, 2) + Math.Pow(z, 2))));
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
