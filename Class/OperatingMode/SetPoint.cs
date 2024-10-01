using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.OperatingMode
{
    public class SetPoint
    {
        /// <summary>
        /// Уставка холостого хода
        /// </summary>
        public float SetPointIdling { get; set; }

        /// <summary>
        /// Уставка проката
        /// </summary>
        public float SetPointRollingMill { get; set; }

        private readonly Frequency frequency;
        private readonly ExpectedMath expectedMath;
        private readonly Dispersion dispersion;

        public SetPoint(Frequency frequency, ExpectedMath expectedMath, Dispersion dispersion)
        {
            this.frequency = frequency;
            this.expectedMath = expectedMath;
            this.dispersion = dispersion;
            SetPointIdling = SetSetPointIdling();
            SetPointRollingMill = SetSetPointRollingMill();
            NewSetPoint();
        }

        /// <summary>
        /// Метод расчитывает уставку проката
        /// </summary>
        /// <returns>Значение уставки проката</returns>
        private float SetSetPointRollingMill()
        {
            if (expectedMath.GetChecking() && dispersion.CheckingRatioValue)
            {
                return frequency.BinsArray[expectedMath.IndexRollingMill, 0] - (SettingsProcess.COEFFICIENT_GET_POINT * dispersion.SigmaRollingMill);
            }

            return 0.0f;
        }

        /// <summary>
        /// Метод расчитывает уставку холостого хода
        /// </summary>
        /// <returns>Значение уставки холостого хода</returns>
        private float SetSetPointIdling()
        {
            return frequency.BinsArray[expectedMath.IndexIdling, 0] + (SettingsProcess.COEFFICIENT_GET_POINT * dispersion.SigmaIdling);
        }

        /// <summary>
        /// Метод пересчета уставок холостого хода и проката
        /// </summary>
        private void NewSetPoint()
        {
            if (SetPointRollingMill > 0.0f && SetPointIdling > 0.0f)
            {
                float pointXX = SetPointRollingMill - SetPointIdling;

                SetPointIdling += (pointXX * 0.5f);

                SetPointRollingMill -= (pointXX * 0.2f);
            }          
        }
    }
}
