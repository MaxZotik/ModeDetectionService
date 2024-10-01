using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ModeDetectionService.Class.Constants;
using ModeDetectionService.Class.ModbusTCP.Packets;
using ModeDetectionService.Class.Enums;
using ModeDetectionService.Class.FileLoggings;

namespace ModeDetectionService.Class.ModbusTCP.ClientTCP
{
    public class ModbusClient
    {
        private string iP;
        private int iPort;
        private Socket socket;
        private readonly IPAddress iPAddress;
        private readonly IPEndPoint endPoint;
        public bool IsConnect { get; set; }

        /// <summary>
        /// Конструктор экземпляра
        /// </summary>
        /// <param name="ipAddress">IP адрес</param>
        /// <param name="port">Номер порта, по умолчанию - "502"</param>
        public ModbusClient(string ipAddress, int port = 502)
        {
            this.iP = ipAddress;
            this.iPort = port;
            iPAddress = IPAddress.Parse(ipAddress);
            endPoint = new IPEndPoint(iPAddress, port);
            IsConnect = false;
        }

        /// <summary>
        /// Метод устанавливает соединение с устройством
        /// </summary>
        public void ConnectTCP()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Connect(this.endPoint);
                socket.SendTimeout = Constant.DEFAULT_TIME_OUT;
                socket.ReceiveTimeout = Constant.DEFAULT_TIME_OUT;
                IsConnect = true;
                new FileLogging().WriteLogAdd($"Соединение с утройством МВК установлено. IP: {iP}, Порт: {iPort}", LoggingStatus.ACTION);
            }
            catch (SocketException se)
            {
                IsConnect = false;
                    new FileLogging().WriteLogAdd($"Соединение с утройством МВК не установлено. IP: {iP}, Порт: {iPort}; {se.Message}", LoggingStatus.ERRORS);
            }
            catch(Exception ex)
            {
                IsConnect = false;
                new FileLogging().WriteLogAdd($"Соединение с утройством МВК не установлено. IP: {iP}, Порт: {iPort}; {ex.Message}", LoggingStatus.ERRORS);
            }
        }

        /// <summary>
        /// Метод отправки и получения пакета Modbus
        /// </summary>
        /// <param name="packet">Пакет для оправки byte[]</param>
        /// <returns>Пакет пулученых byte[]</returns>
        private byte[] SendReceive(byte[] packet)
        {
            try
            {
                byte[] mbap = new byte[7];
                byte[] response;
                ulong count;

                socket.Send(packet);
                socket.Receive(mbap, 0, mbap.Length, SocketFlags.None);
                count = mbap[4];
                count <<= Constant.BYTE;
                count += mbap[5];
                response = new byte[count - 1];
                socket.Receive(response, 0, response.Count(), SocketFlags.None);

                return response;
            }
            catch (SocketException se)
            {
                new FileLogging().WriteLogAdd($"Соединение по IP: {iP}, Порт: {iPort} разорвано! {se.Message}", LoggingStatus.ERRORS);
                IsConnect = false;
                return new byte[1] { 0 };
            }
            catch (Exception ex)
            {
                new FileLogging().WriteLogAdd($"Соединение по IP: {iP}, Порт: {iPort} разорвано! {ex.Message}", LoggingStatus.ERRORS);
                IsConnect = false;
                return new byte[1] { 0 };
            }
        }

        /// <summary>
        /// Метод чтения данных от Modbus
        /// </summary>
        /// <param name="function"></param>
        /// <param name="register"></param>
        /// <param name="count"></param>
        /// <returns>Пакет byte[]</returns>
        private byte[] Read(byte function, ushort register, ushort count)
        {
            Packet packets = new Packet(); 

            byte[] rtn;
            byte[] packet = packets.MakePacket(function, register, count);
            byte[] mbap = packets.MakeMBAP((ushort)packet.Count());
            byte[] response = SendReceive(mbap.Concat(packet).ToArray());

            if (response[0] == 0)
            {
                return response;
            }

            rtn = new byte[response[1]];
            Array.Copy(response, 2, rtn, 0, rtn.Length);
            return rtn;
        }

        /// <summary>
        /// Метод получения расчетных параметров МВК
        /// </summary>
        /// <param name="register">Адрес регистра</param>
        /// <param name="endians">Последовательность передачи байт</param>
        /// <param name="count">Коэфициент длинны пакета. По умолчанию = 1</param>
        /// <returns>Возвращает расчетный параметр МВК</returns>
        public float[] ReadHoldingFloat(ushort register, Endians endians = Endians.Endians_2301, ushort count = 1)
        {
            byte[] rVal = Read(Constant.FUNC_FOR_READ, register, (ushort)(count * Constant.USHORT_LENGTH));

            if (rVal[0] == 0 && rVal.Length == 1)
            {
                return new float[1] { 0 };
            }

            float[] values = new float[rVal.Length / 4];

            try
            {               
                for (int i = 0; i < rVal.Length; i += Constant.FLOAT_LENGTH)
                {
                    if (endians == Endians.Endians_2301)
                    {
                        values[i / 4] = BitConverter.ToSingle(new byte[] { rVal[i + 1], rVal[i], rVal[i + 3], rVal[i + 2] }, 0);
                    }
                    else if (endians == Endians.Endians_0123)
                    {
                        values[i / 4] = BitConverter.ToSingle(new byte[] { rVal[i + 3], rVal[i + 2], rVal[i + 1], rVal[i] }, 0);
                    }
                    else
                    {
                        values[i / 4] = BitConverter.ToSingle(new byte[] { rVal[i], rVal[i + 1], rVal[i + 2], rVal[i + 3] }, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                new FileLogging().WriteLogAdd(ex.Message, LoggingStatus.ERRORS);
            }

            return values;
        }

        /// <summary>
        /// Метод получения параметров счетчика выполненых процедур МВК
        /// </summary>
        /// <param name="register">Адрес регистра</param>
        /// <param name="endians">Последовательность передачи байт</param>
        /// <param name="count">Коэфициент длинны пакета. По умолчанию = 1</param>
        /// <returns>Возвращает состояние счетчика выполненых процедур</returns>
        public uint ReadHoldingUInt(ushort register, Endians endians = Endians.Endians_2301, ushort count = 1)
        {
            UInt32 values = 0;

            try
            {
                byte[] rVal = Read(Constant.FUNC_FOR_READ, register, (ushort)(count * Constant.USHORT_LENGTH));

                if (rVal[0] == 0 && rVal.Length == 1)
                {
                    return values;
                }

                for (int i = 0; i < rVal.Length; i += Constant.FLOAT_LENGTH)
                {
                    if (endians == Endians.Endians_2301)
                    {
                        values = BitConverter.ToUInt32(new byte[] { rVal[i + 1], rVal[i], rVal[i + 3], rVal[i + 2] }, 0);
                    }
                    else if (endians == Endians.Endians_0123)
                    {
                        values = BitConverter.ToUInt32(new byte[] { rVal[i + 3], rVal[i + 2], rVal[i + 1], rVal[i] }, 0);
                    }
                    else
                    {
                        values = BitConverter.ToUInt32(new byte[] { rVal[i], rVal[i + 1], rVal[i + 2], rVal[i + 3] }, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                new FileLogging().WriteLogAdd(ex.Message, LoggingStatus.ERRORS);
            }

            return values;
        }
    }
}
