using ModeDetectionService.Class.Constants;
using ModeDetectionService.Class.Devices;
using ModeDetectionService.Class.Enums;
using ModeDetectionService.Class.FileLoggings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.ModbusTCP.Math
{
    public static class ConditionMath
    {
        private static List<MVKDevice> conditionMVK = new List<MVKDevice>();
        public static bool StatusChannel(UInt32 value, MVKDevice mvk, float voltageValue)
        {           
            int measuringInput = ((value & Constant.OVERLOAD_MEASURING_INPUT) == Constant.OVERLOAD_MEASURING_INPUT) ? 1 : 0;
            int cableBreakage = ((value & Constant.CABLE_BREAKAGE) == Constant.CABLE_BREAKAGE) ? 1 : 0;
            int electricalShort = ((value & Constant.ELECTRICAL_SHORT) == Constant.ELECTRICAL_SHORT) ? 1 : 0;
            int bufferForFrequency16 = ((value & Constant.BUFFER_FOR_FREQUENCY_16) == Constant.BUFFER_FOR_FREQUENCY_16) ? 1 : 0;
            int bufferForFrequency64 = ((value & Constant.BUFFER_FOR_FREQUENCY_64) == Constant.BUFFER_FOR_FREQUENCY_64) ? 1 : 0;
            int bufferForFrequency512 = ((value & Constant.BUFFER_FOR_FREQUENCY_512) == Constant.BUFFER_FOR_FREQUENCY_512) ? 1 : 0;

            bool temp = (measuringInput == 0 && cableBreakage == 0 && electricalShort == 0 && bufferForFrequency16 == 0 &&
                bufferForFrequency64 == 0 && bufferForFrequency512 == 0) ? true : false;
                
            if (!temp)
            {
                bool result = false;

                for (int i = 0; i < conditionMVK.Count; i++)
                {
                    if (conditionMVK[i].Crate == mvk.Crate && conditionMVK[i].NumberMVK == mvk.NumberMVK && conditionMVK[i].Channel == mvk.Channel)
                    {
                        result = true;
                        break;
                    }
                }

                if (!result)
                {
                    conditionMVK.Add(mvk);

                    if (measuringInput != 0)
                        WriteStatusError(mvk, $"Перегрузка измерительного входа!");
                    if (cableBreakage != 0)
                        WriteStatusVoltageError(mvk, voltageValue, $"Постоянное напряжение на датчике больше максимально допустимого, вероятен обрыв кабеля!");
                    if (electricalShort != 0)
                        WriteStatusVoltageError(mvk, voltageValue, $"Постоянное напряжение на датчике меньше минимально допустимого, вероятно короткое замыкание в кабеле!");
                    if (bufferForFrequency16 != 0)
                        WriteStatusError(mvk, $"Еще не заполнен буфер для частоты дискретизации 16 Гц; фильтры с нижней частотой менее 3.9 Гц не активны!");
                    if (bufferForFrequency64 != 0)
                        WriteStatusError(mvk, $"Еще не заполнен буфер для частоты дискретизации 64 Гц; фильтры с нижней частотой менее 25 Гц не активны!");
                    if (bufferForFrequency512 != 0)
                        WriteStatusError(mvk, $"Еще не заполнен буфер для частоты дискретизации 512 Гц; все фильтры не активны!");
                }
            }
            else
            {
                int result = -1;

                for (int i = 0; i < conditionMVK.Count; i++)
                {
                    if (conditionMVK[i].Crate == mvk.Crate && conditionMVK[i].NumberMVK == mvk.NumberMVK && conditionMVK[i].Channel == mvk.Channel)
                    {
                        result = i;
                        break;
                    }
                }

                if (result != -1)
                {
                    try
                    {
                        conditionMVK.RemoveAt(result);
                        WriteStatusError(mvk, $"Работа измерительного канала востановлена!");
                    }
                    catch (Exception ex)
                    {
                        new FileLogging().WriteLogAdd($"{ex.Message}", LoggingStatus.ERRORS);
                    }
                }
            }



            return temp;
        }

        private static void WriteStatusError(MVKDevice mvk, string str)
        {
            new FileLogging().WriteLogAdd($"Устройство: " +
                $"IP: {mvk.IP}, " +
                $"Клеть: {mvk.Crate}, " +
                $"МВК: {mvk.NumberMVK}, " +
                $"Канал: {mvk.Channel}, Состояние канала: {str}", LoggingStatus.ERRORS);            
        }

        private static void WriteStatusVoltageError(MVKDevice mvk, float voltageValue, string str)
        {
            new FileLogging().WriteLogAdd($"Устройство: " +
                $"IP: {mvk.IP}, Клеть: {mvk.Crate}, МВК: {mvk.NumberMVK}, " +
                $"Канал: {mvk.Channel}, Состояние канала: напряжение: {voltageValue}; {str}", LoggingStatus.ERRORS);
        }

    }
}
