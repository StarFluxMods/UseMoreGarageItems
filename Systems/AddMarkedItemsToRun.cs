using UseMoreGarageItems.Components;
using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace UseMoreGarageItems.Systems
{
    public class AddMarkedItemsToRun : FranchiseSystem, IModSystem
    {
        private EntityQuery Pedestals;

        private EntityQuery Selectors;
        
        protected override void Initialise()
        {
            base.Initialise();
            Pedestals = GetEntityQuery(new ComponentType[]
            {
                typeof(CItemHolder),
                typeof(CAddToRun)
            });
            Selectors = GetEntityQuery(new ComponentType[]
            {
                typeof(SBeginGameSelector),
                typeof(CSelectorActivated),
                typeof(CGroupSelector)
            });
            RequireForUpdate(Selectors);
        }

        protected override void OnUpdate()
        {
            using NativeArray<CItemHolder> nativeArray = Pedestals.ToComponentDataArray<CItemHolder>(Allocator.Temp);
            using NativeArray<Entity> pedestals = Pedestals.ToEntityArray(Allocator.Temp);

            if (!GetComponentOfSingletonHolder<CItemLayoutMap, SSelectedLayoutPedestal>(out CItemLayoutMap citemLayoutMap)) return;
            if (GetComponentOfSingletonHolder<CSpeedrun, SSelectedLayoutPedestal>(out CSpeedrun cspeedrun)) return;
            
            foreach (Entity pedestal in pedestals)
            {
                if (!Require(pedestal, out CItemHolder cItemHolder) || !Require(pedestal, out CAddToRun cAddToRun)) continue;
                if (!HasComponent<CProvidesLoadoutItem>(cItemHolder.HeldItem) || !cAddToRun.shouldAdd) continue;
                
                EntityManager.GetBuffer<CStartingItem>(citemLayoutMap.Layout).Add(new CStartingItem
                {
                    ID = GetComponent<CProvidesLoadoutItem>(cItemHolder.HeldItem).ID
                });
                EntityManager.DestroyEntity(cItemHolder.HeldItem);
                cAddToRun.shouldAdd = false;
                EntityManager.SetComponentData(pedestal, cAddToRun);
            }
        }
    }
}