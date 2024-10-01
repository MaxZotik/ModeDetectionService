using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeDetectionService.Class.Test
{
    public class CSVobject
    {
        public string Time { get; set; }

        public string Value { get; set; }

        public string Rolling { get; set; }

        
        public CSVobject(string value, string rolling)
        {
            Time = DateTime.Now.ToString("HH:mm:ss.fff");
            Value = value;
            Rolling = rolling;
            
        }

        public override string ToString()
        {
            return $@"{Time};{Value};{Rolling}";
        }
    }
}
