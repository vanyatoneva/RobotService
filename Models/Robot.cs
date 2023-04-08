using RobotService.Models.Contracts;
using RobotService.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotService.Models
{
    public abstract class Robot : IRobot
    {
        private string model;
        private int battteryCapacity;
        private readonly List<int> interfaceStandarts;

        protected Robot(string model, int battteryCapacity, int convertionCapacityIndex)
        {
            Model = model;
            BatteryCapacity = battteryCapacity;
            BatteryLevel = BatteryCapacity;
            ConvertionCapacityIndex = convertionCapacityIndex;
            interfaceStandarts = new List<int>();
        }

        public string Model
        {
            get => model;
            private set
            {
                if(string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(ExceptionMessages.ModelNullOrWhitespace);
                }
                model = value;  
            }
        }

        public int BatteryCapacity
        {
            get => battteryCapacity;
            private set
            {
                if(value < 0)
                {
                    throw new ArgumentException(ExceptionMessages.BatteryCapacityBelowZero);
                }
                battteryCapacity = value;
            }
        }

        public int BatteryLevel { get; private set; }

        public int ConvertionCapacityIndex { get; private set; }

        public IReadOnlyCollection<int> InterfaceStandards => interfaceStandarts.AsReadOnly();

        public void Eating(int minutes)
        {
            int energyProduced = minutes * ConvertionCapacityIndex;
            BatteryLevel += energyProduced;
            if(BatteryLevel > BatteryCapacity)
            {
                BatteryLevel = BatteryCapacity; 
            }
        }

        public bool ExecuteService(int consumedEnergy)
        {
            if(consumedEnergy > BatteryLevel)
            {
                return false;
            }
            BatteryLevel -= consumedEnergy;
            return true;
        }

        public void InstallSupplement(ISupplement supplement)
        {
            interfaceStandarts.Add(supplement.InterfaceStandard);
            BatteryCapacity -= supplement.BatteryUsage;
            BatteryLevel -= supplement.BatteryUsage;
        }

        public override string ToString()
        {
            string interfaceTypes = InterfaceStandards.Any() ? String.Join(" ", InterfaceStandards) : "none";
            StringBuilder sb = new();
            sb.AppendLine($"{this.GetType().Name} {this.Model}:");
            sb.AppendLine($"--Maximum battery capacity: {BatteryCapacity}");
            sb.AppendLine($"--Current battery level: {BatteryLevel}");
            sb.AppendLine($"--Supplements installed: {interfaceTypes}");
            return sb.ToString().TrimEnd();
        }

    }
}
