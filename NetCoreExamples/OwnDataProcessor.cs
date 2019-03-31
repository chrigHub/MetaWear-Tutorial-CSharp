using MbientLab.MetaWear;
using MbientLab.MetaWear.Data;
using MbientLab.MetaWear.Sensor;
using MbientLab.MetaWear.Sensor.GyroBmi160;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreExamples
{
    class OwnDataProcessor
    {
        private static AngularVelocity array1;

        static async Task RunAsync(string[] args)
        {
            var metawear = await ScanConnect.Connect(args[0]);

            Console.WriteLine($"Configuring {args[0]}...");

            var acc = metawear.GetModule<IAccelerometer>();
            var gyro = metawear.GetModule<IGyroBmi160>();

            acc.Configure(odr: 100f);
            gyro.Configure(odr: OutputDataRate._100Hz);
            //AngularVelocity array1;
            double x1 = 0;
            int i = 1;
            int j = 1;
            await gyro.AngularVelocity.AddRouteAsync(source => source.Stream(data => 
            {
                array1 = data.Value<AngularVelocity>();
                Console.WriteLine(i++ +": " + array1);
                x1 = array1.X;

            }));
            Acceleration array2;
            await acc.Acceleration.AddRouteAsync(source => source.Stream(data2 =>
            {
                array2 = data2.Value<Acceleration>();
                Console.WriteLine(j++ + ": " + array2);
            }));
            //Console.WriteLine(array1.ToString());
            Console.WriteLine("test1");
            gyro.AngularVelocity.Start();
            acc.Acceleration.Start();
            Console.WriteLine("test2");
            gyro.Start();
            acc.Start();
            //TODO Synchronise gyro and acc to get matching data
            await Task.Delay(15000);

            gyro.Stop();
            acc.Stop();
            Console.WriteLine();
            Console.WriteLine(x1);
            Console.ReadLine();
        }
    }
}
