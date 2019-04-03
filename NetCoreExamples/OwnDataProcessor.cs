using MbientLab.MetaWear;
using MbientLab.MetaWear.Core;
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
            AngularVelocity array1;
            float xgy = 0, ygy = 0, zgy = 0;
            //int i = 1;
            int j = 1;
            await gyro.AngularVelocity.AddRouteAsync(source => source.Stream(data => 
            {
                array1 = data.Value<AngularVelocity>();
                //Console.WriteLine(i++ +": " + array1);
                xgy += array1.X;
                ygy += array1.Y;
                zgy += array1.Z;
            }));
            Acceleration array2;
            float xacc = 0, yacc = 0, zacc = 0;
            await acc.Acceleration.AddRouteAsync(source => source.Stream(data2 =>
            {
                array2 = data2.Value<Acceleration>();
                xacc = array2.X;
                yacc = array2.Y;
                zacc = array2.Z;
                //Console.WriteLine(j++ + ": " + array2);
            }));
            //Console.WriteLine(array1.ToString());
            gyro.AngularVelocity.Start();
            acc.Acceleration.Start();
            gyro.Start();
            acc.Start();
            //TODO Synchronise gyro and acc to get matching data
            for (int i = 0; i < 1000; i++)
            {
                await Task.Delay(50);
                Console.WriteLine(String.Format("{0} {1} {2}:{3} {4} {5}", xgy, ygy, zgy, xacc, yacc, zacc));
                xgy = ygy = zgy = 0;
            }
            gyro.Stop();
            acc.Stop();
            Console.WriteLine();
            //Console.WriteLine(x1);
            Console.ReadLine();
        }
    }
}
