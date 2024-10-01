using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.Constants
{
    public static class Constant
    {
        public const byte BYTE = 8;
        public const byte FUNC_FOR_READ = 3;
        public const byte FUNC_FOR_WRITE = 16;
        public const byte USHORT_LENGTH = 2;
        public const byte FLOAT_LENGTH = 4;
        public const int DEFAULT_TIME_OUT = 1000;
        public const byte BEGIN_CODE_ERROR = 128;
        public const ushort ID = 0;

        #region Индикаторы состояния канала

        public const UInt32 OVERLOAD_MEASURING_INPUT = 1;       //Таблица 3.7.7 Бит 0
        public const UInt32 BUFFER_FOR_FREQUENCY_16 = 16;        //Таблица 3.7.7 Бит 4
        public const UInt32 BUFFER_FOR_FREQUENCY_64 = 32;        //Таблица 3.7.7 Бит 5
        public const UInt32 BUFFER_FOR_FREQUENCY_512 = 64;       //Таблица 3.7.7 Бит 6
        public const UInt32 CABLE_BREAKAGE = 1024;              //Таблица 3.7.7 Бит 10
        public const UInt32 ELECTRICAL_SHORT = 2048;            //Таблица 3.7.7 Бит 11

        #endregion

        public const int BACKLOG = 30;
    }
}
