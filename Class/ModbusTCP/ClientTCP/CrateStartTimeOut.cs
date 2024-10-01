using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.ModbusTCP.ClientTCP
{
    public class CrateStartTimeOut
    {
        private DateTime time;
        private DateTime timePause;
        private DateTime timeRepeat;

        private double interval;
        private double pause;
        private double repeat;

        public bool TimeOut
        {
            get
            {
                DateTime now = DateTime.Now;

                if (timePause == time && time > now)
                {
                    return true;
                }
                else if (timePause < now && time > now)
                {
                    return true;
                }
                else
                {
                    if (timeRepeat < now)
                    {
                        time = now.AddSeconds(interval);
                        timePause = now.AddSeconds(interval);
                        timeRepeat = time.AddSeconds(repeat);
                    }

                    return false;
                }
            }
        }

        public CrateStartTimeOut(double interval, double repeat, double pause = 0.0d)
        {
            this.interval = interval;
            this.pause = pause;
            this.repeat = repeat;

            timePause = DateTime.Now.AddSeconds(pause);
            time = timePause.AddSeconds(interval);
            timeRepeat = time.AddSeconds(repeat);
        }

        /// <summary>
        /// Метод перезапускает отсчет времени с нова
        /// </summary>
        public void ResetSetTime()
        {
            timePause = DateTime.Now.AddSeconds(pause);
            time = timePause.AddSeconds(interval);
            timeRepeat = time.AddSeconds(repeat);
        }

        /// <summary>
        /// Метод перезапускает отсчет времени с нова без паузы
        /// </summary>
        public void ResetSetTimeNext()
        {
            timePause = DateTime.Now.AddSeconds(interval);
            time = DateTime.Now.AddSeconds(interval);
            timeRepeat = time.AddSeconds(repeat);
        }
    }
}
