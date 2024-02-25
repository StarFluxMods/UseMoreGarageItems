using UseMoreGarageItems.Components;
using Kitchen;
using KitchenData;
using KitchenLib.References;
using KitchenMods;
using Unity.Entities;

namespace UseMoreGarageItems.Systems
{
    public class MarkLoadbleItem : ItemInteractionSystem, IModSystem
    {
        protected override InteractionType RequiredType => InteractionType.Act;

        protected override bool IsPossible(ref InteractionData data)
        {
            return Require(data.Target, out CAppliance cAppliance)
                   && Has<CAddToRun>(data.Target)
                   && Require(data.Target, out CItemHolder cItemHolder)
                   && cAppliance.ID == ApplianceReferences.GarageShelf
                   && cItemHolder.HeldItem != Entity.Null
                   && Require(cItemHolder.HeldItem, out CItem cItem)
                   && cItem.Category == ItemCategory.Crates;
        }

        protected override void Perform(ref InteractionData data)
        {
            if (!Require(data.Target, out CAddToRun cAddToRun)) return;
            
            cAddToRun.shouldAdd = !cAddToRun.shouldAdd;
            EntityManager.SetComponentData(data.Target, cAddToRun);
        }
    }
}