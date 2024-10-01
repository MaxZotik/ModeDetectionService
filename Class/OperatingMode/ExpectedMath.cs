using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using ModeDetectionService.Class.Enums;
using ModeDetectionService.Class.FileLoggings;

namespace ModeDetectionService.Class.OperatingMode
{
    public class ExpectedMath
    {
        /// <summary>
        /// Индекс вершины холостого хода в массиве значений интервалов и распределений BinsArray экземпляра Frequency
        /// </summary>
        public int IndexIdling { get; set; }

        /// <summary>
        /// Индекс вершины проката в массиве значений интервалов и распределений BinsArray экземпляра Frequency
        /// </summary>
        public int IndexRollingMill { get; set; }

        private Frequency frequency;

        public ExpectedMath(Frequency frequency)
        {
            this.frequency = frequency;
            SetVertex();
        }

        /// <summary>
        /// Метод выполняет проверку выполнения определения вершины холостого хода и вершины проката
        /// </summary>
        /// <returns>Возвращает True - вершины определены, False - вершины не определены</returns>
        public bool GetChecking()
        {
            return (IndexIdling != IndexRollingMill && CheckDensity(IndexIdling) && CheckDensity(IndexRollingMill));
        }

        /// <summary>
        /// Метод пересчитывает объект ExpectedMath по новым данным объекта Frequency
        /// </summary>
        public void ResizeExpectedMath()
        {
            SetVertex();
        }


        #region SetVertex()

        //private void SetVertex()
        //{
        //    int rowsBins = frequency.BinsArray.GetUpperBound(0) + 1;
        //    int resultTemp = -1;
        //    int[] tempArray = Array.Empty<int>();

        //    for (int i = 1; i < rowsBins - 1; i++)
        //    {
        //        if (frequency.BinsArray[i, 1] <= SettingsProcess.COUNT_POINT_INTERVAL)
        //        {
        //            if (resultTemp == -1)
        //                continue;
        //        }
        //        else if (frequency.BinsArray[i, 1] > SettingsProcess.COUNT_POINT_INTERVAL && frequency.BinsArray[i, 1] < frequency.densityVertex)
        //        {
        //            if (i + 1 < rowsBins - 1)
        //                continue;
        //        }
        //        else if (frequency.BinsArray[i, 1] >= frequency.densityVertex)
        //        {
        //            if (resultTemp == -1)
        //            {
        //                resultTemp = i;
        //            }
        //            else if (frequency.BinsArray[i, 1] >= frequency.BinsArray[resultTemp, 1])
        //            {
        //                resultTemp = i;
        //            }

        //            if (i + 1 < rowsBins - 1)
        //                continue;
        //        }

        //        if (resultTemp != -1)
        //        {
        //            tempArray = tempArray.Append(resultTemp).ToArray();
        //            resultTemp = -1;
        //        }
        //    }

        //    if (tempArray.Length < 2)
        //    {
        //        IndexIdling = tempArray[0];
        //        IndexRollingMill = tempArray[0];
        //    }
        //    else if (tempArray.Length > 2)
        //    {
        //        int[] tempArrayNew = new int[] { tempArray[0], tempArray[tempArray.Length - 1] };

        //        for (int i = 1; i < tempArray.Length; i++)
        //        {
        //            if (frequency.BinsArray[tempArrayNew[0], 1] < frequency.BinsArray[tempArray[i], 1])
        //            {
        //                tempArrayNew[0] = tempArray[i];
        //            }
        //        }

        //        for (int i = tempArray.Length - 1; i >= 0; i--)
        //        {
        //            if (tempArrayNew[0] != tempArray[i])
        //            {
        //                if (frequency.BinsArray[tempArrayNew[1], 1] < frequency.BinsArray[tempArray[i], 1]
        //                && frequency.BinsArray[tempArray[i], 1] != frequency.BinsArray[tempArrayNew[0], 1])
        //                {
        //                    tempArrayNew[1] = tempArray[i];
        //                }
        //            }
        //        }

        //        if (tempArrayNew[0] < tempArrayNew[1])
        //        {
        //            IndexIdling = tempArrayNew[0];
        //            IndexRollingMill = tempArrayNew[1];
        //        }
        //        else
        //        {
        //            IndexIdling = tempArrayNew[1];
        //            IndexRollingMill = tempArrayNew[0];
        //        }
        //    }
        //    else
        //    {
        //        if (tempArray[0] < tempArray[1])
        //        {
        //            IndexIdling = tempArray[0];
        //            IndexRollingMill = tempArray[1];
        //        }
        //        else
        //        {
        //            IndexIdling = tempArray[1];
        //            IndexRollingMill = tempArray[0];
        //        }
        //    }
        //}

        #endregion
        /// <summary>
        /// Метод получения точек холостого хода и проката
        /// </summary>
        /// <returns>Массив точек</returns>
        private void SetVertex()
        {
            try
            {
                int rowsBins = frequency.BinsArray.GetUpperBound(0) + 1;
                int resultTemp = -1;
                int[] tempArray = Array.Empty<int>();

                for (int i = 1; i < rowsBins - 1; i++)
                {
                    if (frequency.BinsArray[i, 1] <= SettingsProcess.COUNT_POINT_INTERVAL)
                    {
                        if (resultTemp == -1)
                            continue;
                    }
                    else
                    {
                        if (resultTemp == -1)
                        {
                            resultTemp = i;
                        }
                        else if (frequency.BinsArray[i, 1] >= frequency.BinsArray[resultTemp, 1])
                        {
                            resultTemp = i;
                        }

                        if (i + 1 < rowsBins - 1)
                            continue;
                    }

                    if (resultTemp != -1)
                    {
                        tempArray = tempArray.Append(resultTemp).ToArray();
                        resultTemp = -1;
                    }
                }

                if (tempArray.Length < 2)
                {
                    IndexIdling = tempArray[0];
                    IndexRollingMill = tempArray[0];
                }
                else if (tempArray.Length > 2)
                {
                    int[] tempArrayNew = new int[] { tempArray[0], tempArray[tempArray.Length - 1] };

                    for (int i = 1; i < tempArray.Length; i++)
                    {
                        if (frequency.BinsArray[tempArrayNew[0], 1] < frequency.BinsArray[tempArray[i], 1])
                        {
                            tempArrayNew[0] = tempArray[i];
                        }
                    }

                    for (int i = tempArray.Length - 1; i >= 0; i--)
                    {
                        if (tempArrayNew[0] != tempArray[i])
                        {
                            if (frequency.BinsArray[tempArrayNew[1], 1] < frequency.BinsArray[tempArray[i], 1]
                            && frequency.BinsArray[tempArray[i], 1] != frequency.BinsArray[tempArrayNew[0], 1])
                            {
                                tempArrayNew[1] = tempArray[i];
                            }
                        }
                    }

                    if (tempArrayNew[0] < tempArrayNew[1])
                    {
                        IndexIdling = tempArrayNew[0];
                        IndexRollingMill = tempArrayNew[1];
                    }
                    else
                    {
                        IndexIdling = tempArrayNew[1];
                        IndexRollingMill = tempArrayNew[0];
                    }
                }
                else
                {
                    if (tempArray[0] < tempArray[1])
                    {
                        IndexIdling = tempArray[0];
                        IndexRollingMill = tempArray[1];
                    }
                    else
                    {
                        IndexIdling = tempArray[1];
                        IndexRollingMill = tempArray[0];
                    }
                }               
            }
            catch (Exception ex)
            {
                new FileLogging().WriteLogAdd($"Ошибка вычисления вершин холостого хода и проката:\n{ex.Source};\n{ex.TargetSite};\n{ex.Message}", LoggingStatus.ERRORS);
            }
            
        }


        /// <summary>
        /// Метод проверяет соотношение количества точек в функции распределения с заданным коэфициентом
        /// </summary>
        /// <param name="point">Индекс вершины функции распределения</param>
        /// <returns>Возвращает результат сравнения True - результат удовлетворяет False - результат не удовлетворяет</returns>
        private bool CheckDensity(int point)
        {
            float sumVertex = SumDensity(point);
            float sumPoint = SumPointArray();

            float result = sumVertex / sumPoint;

            return result >= SettingsProcess.COEFFICIENT_DENSITY_POINT;
        }

        /// <summary>
        /// Метод считает количество точек в массиве
        /// </summary>
        /// <returns>Количество точек в массиве</returns>
        private float SumPointArray()
        {
            float result = 0.0f;

            try
            {
                int rows = frequency.BinsArray.GetUpperBound(0) + 1;

                for (int i = 0; i < rows; i++)
                {
                    result += frequency.BinsArray[i, 1];
                }
            }
            catch(Exception ex) 
            {
                new FileLogging().WriteLogAdd($"Ошибка вычисления количество точек в массиве:\n{ex.Source};\n{ex.TargetSite};\n{ex.Message}", LoggingStatus.ERRORS);
            }

            return result;
        }


        /// <summary>
        /// Метод считает количество точек в функции распределения
        /// </summary>
        /// <param name="index">Индекс вершины функции распределения</param>
        /// <returns>Количество точек в функции распределения</returns>
        private float SumDensity(int index)
        {
            try
            {
                int rows = frequency.BinsArray.GetUpperBound(0) + 1;
                float result = frequency.BinsArray[index, 1];

                for (int i = index + 1; i < rows; i++)
                {
                    if (frequency.BinsArray[i, 1] <= SettingsProcess.COUNT_POINT_INTERVAL)
                    {
                        result += frequency.BinsArray[i, 1];
                        break;
                    }

                    result += frequency.BinsArray[i, 1];
                }

                for (int i = index - 1; i >= 0; i--)
                {
                    if (frequency.BinsArray[i, 1] <= SettingsProcess.COUNT_POINT_INTERVAL)
                    {
                        result += frequency.BinsArray[i, 1];
                        break;
                    }

                    result += frequency.BinsArray[i, 1];
                }

                return result;
            }
            catch (Exception ex)
            {
                new FileLogging().WriteLogAdd($"Ошибка вычисления количество точек в функции распределения:\n{ex.Source};\n{ex.TargetSite};\n{ex.Message}", LoggingStatus.ERRORS);
            }

            return 0.0f;
        }
    }
}
