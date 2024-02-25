using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UseMoreGarageItems.Components;

namespace UseMoreGarageItems.Systems
{
    public class ClearEmptyMarkedShelf : GameSystemBase, IModSystem
    {
        private EntityQuery Pedestals;
        protected override void Initialise()
        {
            base.Initialise();
            Pedestals = GetEntityQuery(new ComponentType[]
            {
                typeof(CItemHolder),
                typeof(CAddToRun)
            });
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> pedestals = Pedestals.ToEntityArray(Allocator.Temp);
            
            foreach (Entity pedestal in pedestals)
            {
                if (!Require(pedestal, out CItemHolder cItemHolder) || !Require(pedestal, out CAddToRun cAddToRun)) continue;
                if (cItemHolder.HeldItem != Entity.Null || !cAddToRun.shouldAdd) continue;
                
                cAddToRun.shouldAdd = false;
                EntityManager.SetComponentData(pedestal, cAddToRun);
            }
        }
    }
}