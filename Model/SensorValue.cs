using System;
using System.Collections.Generic;
using System.Text;

namespace odsmicroservice.Model
{
    public class SensorValue
    {
        public int Id { get; set; }
        public int SensorId { get; set; }
        public double Value { get; set; }
        public DateTime DateTime { get; set; }
    }
}
