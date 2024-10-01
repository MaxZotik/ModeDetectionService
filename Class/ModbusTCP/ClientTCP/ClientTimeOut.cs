using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.ModbusTCP.ClientTCP
{
    public class ClientTimeOut
    {
        private DateTime time;
        private double interval;

        public bool TimeOut
        {
            get
            {
                DateTime now = DateTime.Now;

                if (time <= now)
                {
                    time = now.AddSeconds(interval);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public ClientTimeOut() : this(30) { }

        public ClientTimeOut(double count)
        {
            interval = count;
            time = DateTime.Now;
        }
    }
}
