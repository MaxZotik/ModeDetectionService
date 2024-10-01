using Microsoft.Win32;
using ModeDetectionService.Class.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.ModbusTCP.Math
{
    public static class AddressRegister
    {
        public static string SetupAddress(string channel)
        {
            return ((int)Register.ChannelStatusIndicators + (512 * (int.Parse(channel) - 1))).ToString();           
        }

        public static string SetupAddressVoltage(string channel)
        {
            return ((int)Register.VoltageOnSensor + (512 * (int.Parse(channel) - 1))).ToString();
        }
    }
}
