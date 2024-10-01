using ModeDetectionService.Class.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.OperatingMode
{
    public class Frequency
    {
        /// <summary>
        /// Количество интервалов
        /// </summary>
        public int IntervalCount { get; set; }

        /// <summary>
        /// Минимальное значение в массиве DataArray
        /// </summary>
        public float MinValue { get; private set; }

        /// <summary>
        /// Максимальное значение в массиве DataArray
        /// </summary>
        public float MaxValue { get; private set; }

        /// <summary>
        /// Значение единичного интервала
        /// </summary>
        public float IntervalValue { get; private set; }

        /// <summary>
        /// Массив значений за период измерения
        /// </summary>
        public float[] DataArray { get; set; }

        /// <summary>
        /// Массив значений интервалов и распределений
        /// </summary>
        public float[,] BinsArray { get; set; }

        /// <summary>
        /// Плотность (количество) точек на отрезке (интервале)
        /// </summary>
        public int densityVertex;

        public Frequency(int intervalCount, Crate crate)
        {
            DataArray = crate.GetArrayValues();
            IntervalCount = intervalCount;
            MinValue = SetMinValue();
            MaxValue = SetMaxValue();
            IntervalValue = SetIntervalValue();
            BinsArray = SetArrayIntervals();
            densityVertex = SetDensityVertex();
        }

        /// <summary>
        /// Метод изменяет объект Frequency по новому значению IntervalCount
        /// </summary>
        /// <param name="intervalCount">Новое значение IntervalCount</param>
        public void ResizeFrequency(int intervalCount)
        {
            IntervalCount = intervalCount;
            IntervalValue = SetIntervalValue();
            BinsArray = SetArrayIntervals();
        }

        /// <summary>
        /// Метод изменяет объект Frequency по новому значению MaxValue
        /// </summary>
        public void ResizeFrequency()
        {
            MaxValue = SetResizeMaxValue();
            IntervalValue = SetIntervalValue();
            BinsArray = SetArrayIntervals();
        }

        /// <summary>
        /// Получение нового максимального значения за период измерения
        /// </summary>
        /// <returns>Максимального значение</returns>
        private float SetResizeMaxValue()
        {
            int rows = BinsArray.GetUpperBound(0);
            float temp = 0;

            for (int i = rows; i >= 0; i--)
            {
                if (BinsArray[i, 1] >= densityVertex)
                {
                    if (i == rows)
                    {
                        temp = BinsArray[i, 0];
                    }
                    else
                    {
                        temp = BinsArray[i + 1, 0];
                    }

                    break;
                }
            }

            return temp;
        }

        /// <summary>
        /// Метод получения массива значений интервалов и распределений
        /// </summary>
        /// <returns>Массив значений интервалов и распределений</returns>
        private float[,] SetArrayIntervals()
        {
            float[,] result = new float[IntervalCount, 2];

            for (int i = 0; i < IntervalCount; i++)
            {
                if (i == 0)
                {
                    result[i, 0] = MinValue;
                    continue;
                }

                if (i == (IntervalCount - 1))
                {
                    result[i, 0] = MaxValue;
                    continue;
                }

                result[i, 0] = result[i - 1, 0] + IntervalValue;
            }

            SetFuncFrequency(ref result);

            return result;
        }

        /// <summary>
        /// Метод заполнения массива частоты распределения по интервалам
        /// </summary>
        /// <param name="result">Массив значений интервалов и распределений</param>
        private void SetFuncFrequency(ref float[,] result)
        {
            for (int i = 0; i < DataArray.Length; i++)
            {
                for (int j = 0; j < IntervalCount; j++)
                {
                    if (j == 0)
                    {
                        if (result[0, 0] >= DataArray[i])
                        {
                            result[0, 1] += 1;
                        }
                    }
                    else
                    {
                        if (result[j, 0] >= DataArray[i] && DataArray[i] > result[j - 1, 0])
                        {
                            result[j, 1] += 1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Получение максимального значения за период измерения
        /// </summary>
        /// <returns>Максимального значение</returns>
        private float SetMaxValue()
        {
            float temp = 0;

            for (int i = 0; i < DataArray.Length; i++)
            {
                if (temp < DataArray[i])
                    temp = DataArray[i];
            }

            return temp;
        }

        /// <summary>
        /// Получение минимального значения за период измерения
        /// </summary>
        /// <returns>Минимальное значение</returns>
        private float SetMinValue()
        {
            float temp = DataArray[0];

            for (int i = 1; i < DataArray.Length; i++)
            {
                if (temp > DataArray[i])
                    temp = DataArray[i];
            }

            return temp;
        }

        /// <summary>
        /// Значение интервала распределения
        /// </summary>
        /// <returns>Значение единичного интервала</returns>
        private float SetIntervalValue()
        {
            return (MaxValue - MinValue) / IntervalCount;
        }

        /// <summary>
        /// Метод расчета плотности (количества) точек на интервале
        /// </summary>
        /// <param name="countVertex">Количество всех точек</param>
        /// <param name="intervalCount">Количество интервалов</param>
        /// <returns>Возвращает плотность точек на интервале</returns>
        private int SetDensityVertex()
        {
            return (int)((DataArray.Length / IntervalCount) * 0.2);
        }
    }
}
