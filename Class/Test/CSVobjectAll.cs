using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.Test
{
    public class CSVobjectAll : CSVobject
    {
        public string SetPointCrateIdling { get; set; }

        public string SetPointCrateRolling { get; set; }

        public string SetModeCrate { get; set; }

        public CSVobjectAll(string value, string rotation, string setPointCrateIdling, string setPointCrateRolling, string setModeCrate) : base(value, rotation)
        { 
            SetPointCrateIdling = setPointCrateIdling;
            SetPointCrateRolling = setPointCrateRolling;
            SetModeCrate = setModeCrate;
        }

        public override string ToString()
        {
            return $@"{Time};{Value};{Rolling};{SetPointCrateIdling};{SetPointCrateRolling};{SetModeCrate}";
        }
    }
}
