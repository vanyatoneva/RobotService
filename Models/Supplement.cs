using RobotService.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotService.Models
{
    public abstract class Supplement : ISupplement
    {

        protected Supplement(int interfaceStandart, int batteryUsage)
        {
            InterfaceStandard = interfaceStandart;
            BatteryUsage = batteryUsage;
        }
        public int InterfaceStandard { get; private set; }

        public int BatteryUsage { get; private set; }
    }
}
