using ModeDetectionService.Class.Devices;
using ModeDetectionService.Class.Enums;
using ModeDetectionService.Class.FileLoggings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.OperatingMode
{
    public class ActionProcess
    {
        private Frequency frequency;
        private ExpectedMath expectedMath;
        private Dispersion dispersion;
        public SetPoint setPoint;
        private Crate crate;

        public ActionProcess(Crate crate)
        {
            this.crate = crate;
            frequency = new Frequency(SettingsProcess.INTERVAL_ONE, crate);
            expectedMath = new ExpectedMath(frequency);
            dispersion = new Dispersion(frequency, expectedMath);
            setPoint = ActionCalculation();
            crate.SetSetModeCrate(setPoint);
        }

        /// <summary>
        /// Метод логики расчета уставки
        /// </summary>
        /// <returns>Возвращает объект уставки</returns>
        private SetPoint ActionCalculation()
        {
            Print(0);

            for (int i = 0; i <= 3; i++)
            {               
                if (expectedMath.GetChecking() && dispersion.CheckingRatioValue)
                {                   
                    break;
                }
                else if (i == 0)
                {
                    frequency.ResizeFrequency(SettingsProcess.INTERVAL_TWO);
                    expectedMath.ResizeExpectedMath();
                    dispersion.ResizeDispersion();
                    Print(i + 1);
                }
                else if (i == 1)
                {
                    frequency.ResizeFrequency(SettingsProcess.INTERVAL_ONE);
                    frequency.ResizeFrequency();
                    expectedMath.ResizeExpectedMath();
                    dispersion.ResizeDispersion();
                    Print(i + 1);
                }
                else if (i == 2)
                {
                    frequency.ResizeFrequency(SettingsProcess.INTERVAL_TWO);
                    expectedMath.ResizeExpectedMath();
                    dispersion.ResizeDispersion();
                    Print(i + 1);
                }
            }
            
            return new SetPoint(frequency, expectedMath, dispersion);
        }

        private void Print(int i)
        {
            new FileLogging().WriteLogAdd($"--- Результат расчета уставки --- \nКлеть N {crate.NumberCrate}\n" +
                $"Итерация расчета = {i}\n" +
                $"Кол-во интервалов = {frequency.IntervalCount} : Макс = {frequency.MaxValue} : Мин = {frequency.MinValue} : Интервал = {frequency.IntervalValue} : Плотность: {frequency.densityVertex}\n" +
                $"Значение ХХ = {frequency.BinsArray[expectedMath.IndexIdling, 0]} : Значение проката = {frequency.BinsArray[expectedMath.IndexRollingMill,0]}\n" +
                $"Сигма ХХ = {dispersion.SigmaIdling} : Сигма проката = {dispersion.SigmaRollingMill} : Результат = {dispersion.CheckingRatioValue}", LoggingStatus.NOTIFY);                   
        }
    }
}
