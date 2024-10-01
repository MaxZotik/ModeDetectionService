using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModeDetectionService.Class.Constants;
using ModeDetectionService.Class.ModbusTCP.Packets;

namespace ModeDetectionService.Class.ModbusTCP.Packets
{
    public class Packet
    {
        private readonly byte address;

        /// <summary>
        /// Метод преобразования бит для отправки модбасу
        /// </summary>
        /// <param name="function"></param>
        /// <param name="register"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public byte[] MakePacket(byte function, ushort register, ushort count)
        {
            return new byte[] {
                address,                                //slave address
                function,                               //function code
                (byte)(register >> Constant.BYTE),      //start register high
                (byte)register,                         //start register low
                (byte)(count >> Constant.BYTE),         //# of registers high
                (byte)count                             //# of registers low
            };
        }


        public byte[] MakeMBAP(ushort count)
        {
            byte[] idBytes = BitConverter.GetBytes((short)Constant.ID);

            return new byte[] {
                idBytes[0],                         //message id high byte
                idBytes[1],                         //message id low byte
                0,                                  //protocol id high byte
                0,                                  //protocol id low byte
                (byte)(count >> Constant.BYTE),     //length high byte
                (byte)(count)                       //length low byte
            };
        }

    }
}
