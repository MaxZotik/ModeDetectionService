using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModeDetectionService.Class.FileSettings;

namespace ModeDetectionService.Class.OperatingMode
{
    public static class SettingsProcess
    {
        public static readonly int INTERVAL_ONE;        //Количество интервалов 1 уровня
        public static readonly int INTERVAL_TWO;

        public static readonly double TIME_GETTING_VALUES;
        public static readonly double TIME_GETTING_VALUES_PAUSE;
        public static readonly double TIME_GETTING_VALUE_REPEAT;

        public static readonly int COEFFICIENT_GET_POINT;
        public static readonly int COEFFICIENT_CHECKS_VERTEX;

        public static readonly int COUNT_POINT_INTERVAL;
        public static readonly int COEFFICIENT_KNOCK;

        public static readonly int PORT_SERVER;

        public const float COEFFICIENT_DENSITY_POINT = 0.1f;

        static SettingsProcess()
        {
            List<string> list = FileSetting.ServiceSettingLoad();

            INTERVAL_ONE = int.Parse(list[0]);
            INTERVAL_TWO = int.Parse(list[1]);

            TIME_GETTING_VALUES = double.Parse(list[2]);
            TIME_GETTING_VALUES_PAUSE = double.Parse(list[3]);
            TIME_GETTING_VALUE_REPEAT = double.Parse(list[4]);

            COEFFICIENT_GET_POINT = int.Parse(list[5]);
            COEFFICIENT_CHECKS_VERTEX = int.Parse(list[6]);
            COUNT_POINT_INTERVAL = int.Parse(list[7]);
            COEFFICIENT_KNOCK = int.Parse(list[8]);

            PORT_SERVER = int.Parse(list[9]);
        }
    }
}
