using MbientLab.MetaWear;
using MbientLab.MetaWear.Core;
using MbientLab.MetaWear.Data;
using MbientLab.MetaWear.Sensor;
using MbientLab.MetaWear.Sensor.GyroBmi160;
using MbientLab.MetaWear.Core.SensorFusionBosch;
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
            acc.Configure(odr: 25f);
            gyro.Configure(odr: OutputDataRate._25Hz);
            MergedData mergedata = new MergedData();
            await gyro.AngularVelocity.AddRouteAsync(source => source.Stream(data =>
            {
                AngularVelocity gyroData = data.Value<AngularVelocity>();
                mergedata.gyUpdate(gyroData);
                //Console.Clear();
                Console.WriteLine(mergedata.toString());
            }));

            await acc.Acceleration.AddRouteAsync(source => source.Stream(data =>
            {
                Acceleration accData = data.Value<Acceleration>();
                mergedata.acUpdate(accData);
                //Console.Clear();
                Console.WriteLine(mergedata.toString());
            }));
            //Console.WriteLine(array1.ToString());
            //gyro.AngularVelocity.Start();
            //acc.Acceleration.Start
            gyro.Start();
            acc.Start();
            //TODO Synchronise gyro and acc to get matching data
            await Task.Delay(15000);
            gyro.Stop();
            acc.Stop();

            Console.WriteLine();
            //Console.WriteLine(x1);
            Console.ReadLine();
        }
    }

    public class MergedData
    {
        public MergedData ()
        {
            Xac = 0f;
            Yac = 0f;
            Zac = 0f;
            Xgy = 0f;
            Ygy = 0f;
            Zgy = 0f;
            PrevAC = null;
            PrevGY = null;
            Timestamp = DateTime.Now;
        }
        public float Xac { get; set; }
        public float Yac { get; set; }
        public float Zac { get; set; }
        public float Xgy { get; set; }
        public float Ygy { get; set; }
        public float Zgy { get; set; }
        public Acceleration PrevAC { get; set; }
        public AngularVelocity PrevGY { get; set; }
        public DateTime Timestamp { get; set; }

        public void gyUpdate(AngularVelocity gyData)
        {
            DateTime timeref = DateTime.Now;
            float timedif = (float)timeref.Subtract(Timestamp).TotalSeconds;
            Console.WriteLine(timedif);
            if (PrevAC != null)
            { 
                Xac += PrevAC.X * timedif;
                Yac += PrevAC.Y * timedif;
                Zac += PrevAC.Z * timedif;
            }
            Xgy += gyData.X * timedif;
            Ygy += gyData.Y * timedif;
            Zgy += gyData.Z * timedif;
            Timestamp = timeref;
        }

        public void acUpdate(Acceleration acData)
        {
            DateTime timeref = DateTime.Now;
            float timedif = (float)timeref.Subtract(Timestamp).TotalSeconds;
            Console.WriteLine(timedif);
            if (PrevGY != null)
            {
                Xgy += PrevGY.X * timedif;
                Ygy += PrevGY.Y * timedif;
                Zgy += PrevGY.Z * timedif;
            }
            Xac += acData.X * timedif;
            Yac += acData.Y * timedif;
            Zac += acData.Z * timedif;
            Timestamp = timeref;
        }

        public String toString()
        {
            return String.Format("Pitch: {0}  Roll: {1}  Yaw: {2}\nLeft/Right: {3}  Forwards/Backwards: {4}  Up/Down: {5}", Xgy, Ygy, Zgy, Xac, Yac, Zac);
        }
    }
}
