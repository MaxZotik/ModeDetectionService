using ModeDetectionService.Class.Devices;
using ModeDetectionService.Class.Enums;
using ModeDetectionService.Class.FileLoggings;
using ModeDetectionService.Class.ModbusTCP.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.ModbusTCP.ClientTCP
{
    public class WorkClient
    {
        object _lock = new object();

        public ModbusClient modbusClient;
        private int indexNumberMVK;
        public ClientTimeOut clientTimeOut = new ClientTimeOut();

        /// <summary>
        /// Список клиентов TCP
        /// </summary>
        public static List<WorkClient> MyModbusTCPList { get; set; }

        static WorkClient()
        {
            MyModbusTCPList = CreateMyModbusTCP();
        }

        public WorkClient() { }


        public WorkClient(ModbusClient modbusClient, int indexNumberMVK)
        {
            this.modbusClient = modbusClient;
            this.indexNumberMVK = indexNumberMVK;
        }

        /// <summary>
        /// Метод создает список клиентов TCP для опроса устройств МВК 
        /// </summary>
        /// <returns>Возвращает список клиентов TCP</returns>
        static List<WorkClient> CreateMyModbusTCP()
        {
            int count = DSPCounter.MVKCounterList.Count;

            List<WorkClient> list = new List<WorkClient>(count);

            for (int i = 0; i < count; i++)
            {
                list.Insert(i, new WorkClient(new ModbusClient(DSPCounter.MVKCounterList[i].IPAddress, int.Parse(DSPCounter.MVKCounterList[i].Port)), i));
            }

            return list;
        }


        /// <summary>
        /// Метод получения значения значения параметров устройства МВК
        /// </summary>
        /// <param name="modbusClientDatabase">Устройство МВК</param>
        /// <param name="mvk">Объект клиента TCP</param>
        public void ReadDataWithMVK(MVKDevice modbusClientDatabase, ModbusClient mvk)
        {
            lock (_lock)
            {
                try
                {
                    if (ReadConditionMVK(modbusClientDatabase, mvk))
                    {
                        float[] value = mvk.ReadHoldingFloat(ushort.Parse(modbusClientDatabase.Address.ToString()),
                        modbusClientDatabase.Endian == "3210" ? Endians.Endians_3210 :
                        modbusClientDatabase.Endian == "0123" ? Endians.Endians_0123 : Endians.Endians_2301);

                        if (value[0] >= 0)
                        {
                            modbusClientDatabase.Value = value[0];

                            RepositoryDatabase.WriteListDB.Add(modbusClientDatabase);
                        }
                    }                    
                }
                catch (Exception ex)
                {                   
                    new FileLogging().WriteLogAdd($"Ошибка преобразования полученных параметров МВК! - {ex.Message}", LoggingStatus.ERRORS);
                }
            }
        }

        /// <summary>
        /// Метод получения значения счетчика выполненых процедур МВК
        /// </summary>
        /// <param name="index">Индех устройства МВК в списке объектов DSPCounter</param>
        /// <param name="mvk">Устройство МВК</param>
        /// <returns>True - если счетчик изменился и False - если счетчик не изменился</returns>
        public bool ReadCounterWithMVK(int index, ModbusClient mvk)
        {
            lock (_lock)
            {
                UInt32 value = 0;
                bool isChecked = false;

                try
                {
                    value = mvk.ReadHoldingUInt(ushort.Parse(DSPCounter.MVKCounterList[index].StartAddress.ToString()),
                        DSPCounter.MVKCounterList[index].Endians == "3210" ? Endians.Endians_3210 :
                        DSPCounter.MVKCounterList[index].Endians == "0123" ? Endians.Endians_0123 : Endians.Endians_2301);

                    if (value != 0 && value != DSPCounter.MVKCounterList[index].Parameters)
                    {
                        DSPCounter.MVKCounterList[index].Parameters = value;
                        isChecked = true;
                    }
                }
                catch (Exception ex)
                {
                    new FileLogging().WriteLogAdd($"Ошибка преобразования полученных параметров счетчика выполненых процедур МВК! - {ex.Message}", LoggingStatus.ERRORS);
                }

                return isChecked;
            }
        }

        /// <summary>
        /// Метод получения состояния канала МВК
        /// </summary>
        /// <param name="modbusClientDatabase">Устройство МВК</param>
        /// <param name="mvk">Объект клиента TCP</param>
        /// <returns>True - если канал рабочий и False - если канал не работает</returns>
        public bool ReadConditionMVK(MVKDevice modbusClientDatabase, ModbusClient mvk)
        {
            lock (_lock)
            {
                UInt32 value = 0;
                float valueVoltage = 0.0f;

                if (modbusClientDatabase.Parameter == "Частота вращения")
                {
                    return true;
                }

                float[] valueVoltageArray = mvk.ReadHoldingFloat(ushort.Parse(AddressRegister.SetupAddressVoltage(modbusClientDatabase.Channel.ToString())),
                            modbusClientDatabase.Endian == "3210" ? Endians.Endians_3210 :
                            modbusClientDatabase.Endian == "0123" ? Endians.Endians_0123 : Endians.Endians_2301);

                valueVoltage = valueVoltageArray[0];

                try
                {
                    value = mvk.ReadHoldingUInt(ushort.Parse(AddressRegister.SetupAddress(modbusClientDatabase.Channel)),
                        modbusClientDatabase.Endian == "3210" ? Endians.Endians_3210 :
                        modbusClientDatabase.Endian == "0123" ? Endians.Endians_0123 : Endians.Endians_2301);
                }
                catch (Exception ex)
                {
                    new FileLogging().WriteLogAdd($"Ошибка преобразования полученного параметра состояния канала МВК! - {ex.Message}", LoggingStatus.ERRORS);
                }

                return ConditionMath.StatusChannel(value, modbusClientDatabase, valueVoltage);
            }
        }


        /// <summary>
        /// Метод для перезапуска клиента TCP
        /// </summary>
        private void StartConnect()
        {
            modbusClient.ConnectTCP();
        }

        /// <summary>
        /// Метод содержит логику опрома устройств МВК
        /// </summary>
        public void MVKtempOne()
        {
            if (modbusClient.IsConnect)
            {
                if (!ReadCounterWithMVK(indexNumberMVK, modbusClient))
                {
                    return;
                }
                else
                {
                    string key = DSPCounter.MVKCounterList[indexNumberMVK].IPAddress;
                    int count = RepositoryDatabase.DatabaseDictionary[key].Count;

                    for (int i = 0; i < count; i++)
                    {
                        ReadDataWithMVK(RepositoryDatabase.DatabaseDictionary[key][i], modbusClient);
                    }
                }
            }
            else
            {
                if (!clientTimeOut.TimeOut)
                {
                    Thread th = new Thread(StartConnect);
                    th.Start();
                }
            }

            //new Thread(() => Print()).Start();
        }


        public static void Print()
        {
            foreach (MVKDevice mvk in RepositoryDatabase.WriteListDB)
            {
                new FileLogging().WriteLogAdd($"RepositoryDatabase.WriteListDB: {mvk.IP} : {mvk.Crate} : {mvk.NumberMVK} : {mvk.Parameter} : {mvk.Value}", LoggingStatus.NOTIFY);
            }
        }
    }
}
