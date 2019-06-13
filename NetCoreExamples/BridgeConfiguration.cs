using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreExamples
{
    public class BridgeConfiguration
    {
        //Steps
        private int tg = 1;
        private int ag = 1;
        private int eg = 1;
        private int rg = 1;

        //Start values
        private int init_tg = 1000;
        private int init_ag = 1500;
        private int init_eg = 1500;
        private int init_rg = 1500;

        //Max values
        private int max_tg = 1900;
        private int max_ag = 2000;
        private int max_eg = 2000;
        private int max_rg = 2000;

        /// <summary>
        /// Step size for power
        /// </summary>
        public int Tg { get => tg; }

        /// <summary>
        /// Step size for left/right
        /// </summary>
        public int Ag { get => ag; }

        /// <summary>
        /// Step size for forward/backward
        /// </summary>
        public int Eg { get => eg; }

        /// <summary>
        /// Step size for rotation
        /// </summary>
        public int Rg { get => rg; }
        public int Init_tg { get => init_tg; }
        public int Init_ag { get => init_ag;}
        public int Init_eg { get => init_eg; }
        public int Init_rg { get => init_rg; }
        public int Max_tg { get => max_tg; }
        public int Max_ag { get => max_ag; }
        public int Max_eg { get => max_eg; }
        public int Max_rg { get => max_rg; }

        public BridgeConfiguration(int stepTG, int stepAG, int stepEG, int stepRG, int i_TG, int i_AG, int i_EG,
            int i_RG, int m_TG, int m_AG, int m_EG, int m_RG)
        {
            tg = stepTG;
            ag = stepAG;
            eg = stepEG;
            rg = stepRG;

            init_tg = i_TG;
            init_ag = i_AG;
            init_eg = i_EG;
            init_rg = i_RG;
        }
        /// <summary>
        /// Default Configuration.
        /// </summary>
        public BridgeConfiguration()
        {

        }
    }
}
//channel 8 > 1700 ---> RESET