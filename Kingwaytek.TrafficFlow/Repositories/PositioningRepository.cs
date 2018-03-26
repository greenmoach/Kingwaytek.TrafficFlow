using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingwaytek.TrafficFlow.Repositories;

namespace Kingwaytek.TrafficFlow
{
    public class PositioningRepository : RepositoryBase<TaoyuanTrafficEntities, Positioning>
    {
        public void AddOrUpdate(Positioning entity)
        {
            DbSet.AddOrUpdate(entity);
            Context.SaveChanges();
        }
    }
}