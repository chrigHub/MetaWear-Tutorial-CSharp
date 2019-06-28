using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Timers;

namespace NetCoreExamples
{
    public class ArduinoBridge
    {
     //Power Settings
        private int throttle = 1000; //power
        private int aileron = 1500; //linksrechts
        private int elevator = 1500; //vor/zurück
        private int rudder = 1500; //rotation
        

        private BridgeConfiguration config = new BridgeConfiguration();

        private SerialPort arduino;

        private bool readingEnabled;

        /// <summary>
        /// Returns an integer representation of the current throttle level (1000 = OFF, 2000=max)
        /// </summary>
        public int Throttle
        {
            get { return throttle; }
            private set
            {
                if (value > 2000) throttle = 2000;
                else if (value < 1000) throttle = 1000;
                else throttle = value;
            }
        }
        /// <summary>
        /// Returns the current left/right movement commands. (1500 = no movement, >1500 = move right)
        /// </summary>
        public int LR
        {
            get { return aileron; }
            private set { aileron = value; }
        }
        /// <summary>
        /// Returns the current forward/backward movement commands. (1500 = no movement, >1500 = move forward)
        /// </summary>
        public int FB
        {
            get { return elevator; }
            private set { elevator = value; }
        }
        /// <summary>
        /// Returns the current rotation movement commands. (1500 = no rotation)
        /// </summary>
        public int Rotation
        {
            get { return rudder; }
            private set { rudder = value; }
        }
        
        public bool Connected
        {
            get
            {
                if (arduino == null) return false;
                else return arduino.IsOpen;
            }
            set
            {
                if (value == arduino.IsOpen) return;
                if (value)
                {
                    arduino.Open();
                    readingEnabled = true;
                    Task.Run(() => ReadResponse());
                }
                else
                {
                    arduino.Close();
                    readingEnabled = false;
                }
            }
        }

        /// <summary>
        /// Returns the drone throttle level in percent.
        /// </summary>
        public int PowerLevel
        {
            get { return Convert.ToInt32(2000 - throttle / 10); }
        }

        public string Status
        {
            get { return String.Format("({0} | {1} | {2} | {3})", Throttle, LR, FB, Rotation); }
        }
        /// <summary>
        /// Opens a new Port and connects to an arduino.
        /// </summary>
        /// <param name="port"></param>
        public ArduinoBridge(int port)
        {
            arduino = new SerialPort("COM" + port, 115200);
            try
            {
                if (!arduino.IsOpen)
                {
                    arduino.Open();
                    readingEnabled = true;
                    Task.Run(()=>ReadResponse());
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Fehler beim Verbindungsaufbau zum Arduino.\n" + e.Message);
            }

            Task.Delay(1000); // TODO: geht?
        }

        private void ReadResponse()
        {
            while (readingEnabled)
            {
                try
                {
                    Console.WriteLine(arduino.ReadLine());
                }
                catch { }
            }
        }
        /// <summary>
        /// Writes the currently set commands to the arduino.
        /// </summary>
        private void Write()
        {
            arduino.Write(String.Format("{0},{1},{2},{3}\n", Throttle, LR, FB, Rotation));
        }

        /// <summary>
        /// Changes the power level by the input amount.
        /// </summary>
        /// <param name="amount">Negative to decrease power, positive to increase.</param>
        private void ChangePower(int amount)
        {
            if (!Connected) throw new Exception("Drone powered off.");
            Throttle = Throttle + amount * config.Tg;
            Write();
        }

        public void IncreasePower(int amount)
        {
            ChangePower(amount);
        }

        public void IncreasePower()
        {
            ChangePower(1);
        }

        public void DecreasePower(int amount)
        {
            ChangePower(-amount);
        }

        public void DecreasePower()
        {
            ChangePower(-1);
        }

        /// <summary>
        /// Sets the powerlevel to a certain percentage.
        /// </summary>
        /// <param name="percentage">Use values between 0 and 1 only.</param>
        public void SetPower(double percentage)
        {
            if (!Connected) throw new Exception("Drone powered off.");
            if (percentage > 1 || percentage < 0) throw new Exception("Invalid Percentage");
            Throttle = 1000 + Convert.ToInt32(percentage * 1000);
            Write();
        }
        /// <summary>
        /// Moves the drone to the left or the right with the specified power (amount).
        /// </summary>
        /// <param name="amount"></param>
        public void MoveLR(int amount)
        {
            if (!Connected) throw new Exception("Drone powered off.");
            LR = amount * config.Ag + config.Init_ag;
            Write();
        }

        public void MoveLeft(int amount)
        {
            MoveLR(-amount);
        }

        public void MoveLeft()
        {
            MoveLeft(1);
        }

        public void MoveRight(int amount)
        {
            MoveLR(amount);
        }

        public void MoveRight()
        {
            MoveRight(1);
        }
        /// <summary>
        /// Moves the drone forward/backward with the specified power (amount).
        /// </summary>
        /// <param name="amount"></param>
        public void MoveFB(int amount)
        {
            if (!Connected) throw new Exception("Drone powered off.");
            FB = amount * config.Eg + config.Init_eg;
            Write();
        }

        public void MoveForward(int amount)
        {
            MoveFB(amount);
        }

        public void MoveForward()
        {
            MoveForward(1);
        }

        public void MoveBackward(int amount)
        {
            MoveFB(-amount);
        }

        public void MoveBackward()
        {
            MoveBackward(1);
        }

        public void Move(int pitch, int roll)
        {
            if (!Connected) throw new Exception("Drone powered off.");
            FB = pitch * config.Eg + config.Init_eg;
            LR = roll * config.Ag + config.Init_ag;
            Write();
        }

        /// <summary>
        /// Rotates the drone with the specified speed (amount).
        /// </summary>
        /// <param name="amount"></param>
        public void Rotate(int amount)
        {
            if (!Connected) throw new Exception("Drone powered off.");
            Rotation += amount * config.Rg;
            Write();
        }

        /// <summary>
        /// Resets throttle, aileron, elevator and rudder to the default values.
        /// </summary>
        public void Reset()
        {
            throttle = config.Init_tg;
            aileron = config.Init_ag;
            elevator = config.Init_eg;
            rudder = config.Init_rg;
        }

        public void SetConfiguration(BridgeConfiguration c)
        {
            if (c == null) return;
            config = c;
        }
    }
}
