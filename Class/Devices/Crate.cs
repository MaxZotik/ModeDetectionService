using ModeDetectionService.Class.Enums;
using ModeDetectionService.Class.FileLoggings;
using ModeDetectionService.Class.ModbusTCP.ClientTCP;
using ModeDetectionService.Class.OperatingMode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ModeDetectionService.Class.Test;

namespace ModeDetectionService.Class.Devices
{
    public class Crate
    {
        /// <summary>
        /// Номер клети
        /// </summary>
        public int NumberCrate {  get; private set; }

        /// <summary>
        /// Уставка холостого хода
        /// </summary>
        public float SetPointCrateIdling { get; private set; }

        /// <summary>
        /// Уставка проката
        /// </summary>
        public float SetPointCrateRolling {  get; private set; }

        /// <summary>
        /// Наименование режима работы клети
        /// </summary>
        public ModeCrates SetModeCrate { get; private set; }

        public float CurrentValue {  get; private set; }

        public CrateStartTimeOut CrateStartTime { get; set; }

        public static List<Crate> CratesList { get; set; }

        public List<float> ValuesList {  get; set; }

        private ModeCrates modeCratesTemp;
        private ModeCrates modeCratesOldTemp;

        //public CSVfile csvFile;

        static Crate()
        {
            CratesList = GetCratesList();
        }

        public Crate(int numberCrate, float setPointCrateIdling = 0.0f, float setPointCrateRolling = 0.0f, ModeCrates modeCrates = ModeCrates.NoData) 
        {
            modeCratesTemp = ModeCrates.NoData;
            modeCratesOldTemp = ModeCrates.NoData;
            NumberCrate = numberCrate;
            SetPointCrateIdling = setPointCrateIdling;
            SetPointCrateRolling = setPointCrateRolling;
            SetModeCrate = modeCrates;     
            ValuesList = new List<float>();
            CrateStartTime = new CrateStartTimeOut(SettingsProcess.TIME_GETTING_VALUES, SettingsProcess.TIME_GETTING_VALUE_REPEAT, SettingsProcess.TIME_GETTING_VALUES_PAUSE);
            CurrentValue = 0.0f;
            //csvFile = new CSVfile(NumberCrate);
        }

        /// <summary>
        /// Метод устанавливает уставки холостого хода и проката
        /// </summary>
        /// <param name="setPoint">Объект значений уставок</param>
        public void SetSetModeCrate(SetPoint setPoint)
        {
            if (setPoint.SetPointIdling != 0.0f && setPoint.SetPointRollingMill != 0.0f)
            {
                SetPointCrateIdling = setPoint.SetPointIdling;
                SetPointCrateRolling = setPoint.SetPointRollingMill;
            }
            
            if (SetPointCrateIdling == 0.0f || SetPointCrateRolling == 0.0f && SetModeCrate != ModeCrates.Stop)
            {
                CrateStartTime.ResetSetTimeNext();
            }

            ArrayValuesClear();
            //csvFile.WriteFileCVSobject();
        }

        /// <summary>
        /// Метод отпределяет и устанавливает режим работы клети
        /// </summary>
        /// <param name="value">Значение параметра от МВК Modbus</param>
        /// <param name="rotation">Значение оборотов от МВК Modbus</param>
        public void DefineModeCrate(float value, float rotation)
        {
            CurrentValue = value;

            if (rotation == 0.0f)
            {
                SetModeCrate = ModeCrates.Stop;
                modeCratesTemp = ModeCrates.NoData;
                modeCratesOldTemp = ModeCrates.NoData;
                SetPointCrateIdling = 0.0f;
                SetPointCrateRolling = 0.0f;
                CrateStartTime.ResetSetTime();
                ArrayValuesClear();
                //csvFile.CSVobjectListClear();
            }
            else
            {
                if(SetModeCrate == ModeCrates.Stop)
                {
                    SetPointCrateIdling = 0.0f;
                    SetPointCrateRolling = 0.0f;
                    SetModeCrate = ModeCrates.NoData;
                    CrateStartTime.ResetSetTime();
                }
                else if (SetPointCrateIdling == 0.0f && SetPointCrateRolling == 0.0f)
                {
                    SetModeCrate = ModeCrates.NoData;
                }
                else if ((SetPointCrateRolling * 3) < value)
                {
                    SetModeCrate = ModeCrates.Rolling;
                }
                else if (SetPointCrateIdling < value && SetPointCrateRolling > value)
                {
                    modeCratesTemp = ModeCrates.NoMode;
                }
                else if (SetPointCrateIdling > value)
                {
                    modeCratesTemp = ModeCrates.Idling;
                }
                else if (SetPointCrateRolling < value)
                {
                    modeCratesTemp = ModeCrates.Rolling;
                }

                if (SetModeCrate != modeCratesTemp && SetModeCrate != modeCratesOldTemp)
                {
                    SetModeCrate = modeCratesOldTemp;
                }

                modeCratesOldTemp = modeCratesTemp;
            }

            if (SetModeCrate != ModeCrates.Stop && CrateStartTime.TimeOut)
            {
                WriteValueList(value);
            }

            //csvFile.CSVobjectAllListAdd(value.ToString(), rotation.ToString(), SetPointCrateIdling.ToString(), SetPointCrateRolling.ToString(), SetModeCrate.ToString());
            //csvFile.WriteFileCVSobjectAll();
        }

        /// <summary>
        /// Метод заполняет список значений в интервале для расчета режима работы клети
        /// </summary>
        /// <param name="value"></param>
        private void WriteValueList(float value)
        {
            ValuesList.Add(value);
        }

        /// <summary>
        /// Метод заполняет список работающих Клетей
        /// </summary>
        /// <returns>Возвращает список Клетей</returns>
        private static List<Crate> GetCratesList()
        {
            List<Crate> list = new List<Crate>();

            List<MVKDevice> devices = new List<MVKDevice>();
            devices.InsertRange(0, MVKDevice.MVKDevicesList);

            for (int i = 0; i < devices.Count; i++)
            {
                for (int k = (i+1); k < devices.Count; k++)
                {
                    if (devices[i].Crate == devices[k].Crate)
                    {
                        devices.RemoveAt(k);
                        k--;
                    }
                }
            }

            for (int i = 0; i < devices.Count; i++)
            {
                list.Add(new Crate(int.Parse(devices[i].Crate)));
            }

            return list;
        }


        /// <summary>
        /// Метод конвертирует Список значений в массив значений
        /// </summary>
        /// <returns>Возвращает массив значений</returns>
        public float[] GetArrayValues()
        {
            float[] array = new float[ValuesList.Count];

            ValuesList.CopyTo(array);

            return array;
        }

        /// <summary>
        /// Метод очищает список значений
        /// </summary>
        public void ArrayValuesClear()
        {
            ValuesList.Clear();
        }
    }
}
