using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModeDetectionService.Class.FileSettings;

namespace ModeDetectionService.Class.Devices
{
    public class MVKDevice
    {
        /// <summary>
        /// IP адрес утсроуйства МВК
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Port устройства МВК
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// Последовательность передачи байт
        /// </summary>
        public string Endian { get; set; }

        /// <summary>
        /// Номер клети
        /// </summary>
        public string Crate { get; set; }

        /// <summary>
        /// Номер МВК
        /// </summary>
        public string NumberMVK { get; set; }

        /// <summary>
        /// Номер канала утсроуйства МВК
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// Частота утсроуйства МВК
        /// </summary>
        public string Frequency { get; set; }

        /// <summary>
        /// Параметр утсроуйства МВК
        /// </summary>
        public string Parameter { get; set; }

        /// <summary>
        /// Адрес утсроуйства МВК
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Полученое значение канала МВК
        /// </summary>
        public float Value { get; set; }

        public static List<MVKDevice> MVKDevicesList { get; set; }

        static MVKDevice()
        {
            MVKDevicesList = FileSetting.MVKSettingLoad();
        }

        public MVKDevice(string ip, string port, string endian, string crate, string numberMVK, string channel, string frequency, string parameter, string address)
        {
            IP = ip;
            Port = port;
            Endian = endian;
            Crate = crate;
            NumberMVK = numberMVK;
            Channel = channel;
            Frequency = frequency;
            Parameter = parameter;
            Address = address;
            Value = 0;
        }
    }
}
