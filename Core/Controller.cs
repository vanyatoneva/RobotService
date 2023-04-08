using RobotService.Core.Contracts;
using RobotService.Models;
using RobotService.Models.Contracts;
using RobotService.Repositories;
using RobotService.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotService.Core
{
    public class Controller : IController
    {
        private SupplementRepository supplements;
        private RobotRepository robots;

        public Controller() 
        {
            supplements = new SupplementRepository();
            robots = new RobotRepository();
        }

        public string CreateRobot(string model, string typeName)
        {
            IRobot newRobot = null;
            switch (typeName)
            {
                case "DomesticAssistant":
                    newRobot = new DomesticAssistant(model);
                    break;
                case "IndustrialAssistant":
                    newRobot = new IndustrialAssistant(model);
                    break;
                default: 
                    return String.Format(OutputMessages.RobotCannotBeCreated, typeName);
            }
            robots.AddNew(newRobot);
            return String.Format(OutputMessages.RobotCreatedSuccessfully, newRobot.GetType().Name ,newRobot.Model);
        }

        public string CreateSupplement(string typeName)
        {
            ISupplement newSupplement;
            switch (typeName)
            {
                case "SpecializedArm":
                    newSupplement = new SpecializedArm();
                    break;
                case "LaserRadar":
                    newSupplement = new LaserRadar();
                    break;
                default:
                    return String.Format(OutputMessages.SupplementCannotBeCreated, typeName);
            }
            supplements.AddNew(newSupplement);
            return String.Format(OutputMessages.SupplementCreatedSuccessfully, typeName);
        }

        public string PerformService(string serviceName, int intefaceStandard, int totalPowerNeeded)
        {
            List<IRobot> robotsSuppportingInterfaceStandart = 
                robots.Models().
                Where(r => r.InterfaceStandards.Contains(intefaceStandard))
                .OrderByDescending(r => r.BatteryLevel)
                .ToList();
            if (!robotsSuppportingInterfaceStandart.Any())
            {
                return String.Format(OutputMessages.UnableToPerform, intefaceStandard);
            }
            int sumOfBatteryLevel = robotsSuppportingInterfaceStandart.Sum(r => r.BatteryLevel);
            if(sumOfBatteryLevel < totalPowerNeeded)
            {
                return String.Format(OutputMessages.MorePowerNeeded, serviceName, (totalPowerNeeded - sumOfBatteryLevel));
            }

            int counter = 0;
            foreach(IRobot robot in robotsSuppportingInterfaceStandart)
            {
                if(robot.BatteryLevel >= totalPowerNeeded)
                {
                    robot.ExecuteService(totalPowerNeeded);
                    totalPowerNeeded = 0;
                    counter++;
                    break;
                }
                totalPowerNeeded -= robot.BatteryLevel;
                robot.ExecuteService(robot.BatteryLevel);
                counter++;
            }

            return String.Format(OutputMessages.PerformedSuccessfully, serviceName, counter);
        }

        public string Report()
        {
            List<IRobot> robotsReport = robots.Models().
                OrderByDescending(r => r.BatteryLevel).
                ThenBy(r => r.BatteryCapacity).
                ToList();
            StringBuilder sb = new();
            foreach(IRobot robot in robotsReport)
            {
                //TODO -- here not sure about new line
                sb.Append(robot.ToString());
                sb.AppendLine();
            }

            return sb.ToString().TrimEnd();
        }

        public string RobotRecovery(string model, int minutes)
        {
            int robotsFed = 0;
            List<IRobot> robotsToFeed = robots.Models().
                Where((r => (r.Model == model && (double)r.BatteryLevel < r.BatteryCapacity * 0.5))).
                ToList();
            foreach(IRobot robot in robotsToFeed)
            {
                robot.Eating(minutes);
                robotsFed++;
            }
            return String.Format(OutputMessages.RobotsFed, robotsFed);
        }

        public string UpgradeRobot(string model, string supplementTypeName)
        {
            ISupplement supplementToAdd = supplements.Models().FirstOrDefault(s => s.GetType().Name == supplementTypeName);
            int intrerfaceToAdd = supplementToAdd.InterfaceStandard;
            List<IRobot> robotsToUpgrade = robots.Models().
                Where((r => r.InterfaceStandards.Contains(intrerfaceToAdd) == false)).
                Where(r => r.Model == model)
                .ToList();
            if (!robotsToUpgrade.Any())
            {
                return String.Format(OutputMessages.AllModelsUpgraded, model);
            }
            
            IRobot robot = robotsToUpgrade.First();
            robot.InstallSupplement(supplementToAdd);
            
            supplements.RemoveByName(supplementTypeName);
            return String.Format(OutputMessages.UpgradeSuccessful, model, supplementTypeName);
        }
    }
}
