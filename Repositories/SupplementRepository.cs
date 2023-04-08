using RobotService.Models.Contracts;
using RobotService.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotService.Repositories
{
    public class SupplementRepository : IRepository<ISupplement>
    {
        private readonly List<ISupplement> models;

        public SupplementRepository()
        {
            models = new List<ISupplement>();
        }

        public void AddNew(ISupplement model)
        {
            models.Add(model);
        }

        public ISupplement FindByStandard(int interfaceStandard)
        {
            return models.FirstOrDefault(x => x.InterfaceStandard == interfaceStandard);
        }

        public IReadOnlyCollection<ISupplement> Models()
            => models.AsReadOnly();

        public bool RemoveByName(string typeName)
        {
            ISupplement supplementToRemove = models.FirstOrDefault(s => s.GetType().Name == typeName);
            if(supplementToRemove == null)
            {
                return false;
            }
            models.Remove(supplementToRemove);
            return true;
        }
    }
}
