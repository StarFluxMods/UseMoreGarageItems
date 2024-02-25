using Kitchen;
using KitchenMods;
using MessagePack;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UseMoreGarageItems.Components;

namespace UseMoreGarageItems.Views
{
    public class AddToRunView : UpdatableObjectView<AddToRunView.ViewData>
    {
        public class UpdateView : IncrementalViewSystemBase<ViewData>, IModSystem
        {
            private EntityQuery Views;
            protected override void Initialise()
            {
                base.Initialise();
                Views = GetEntityQuery(new QueryHelper().All(typeof(CAddToRun), typeof(CLinkedView)));
            }

            protected override void OnUpdate()
            {
                using var entities = Views.ToEntityArray(Allocator.Temp);
                using var views = Views.ToComponentDataArray<CLinkedView>(Allocator.Temp);

                for (var i = 0; i < views.Length; i++)
                {
                    var view = views[i];
                    if (Require(entities[i], out CAddToRun cAddToRun))
                    {
                        ViewData data = new ViewData
                        {
                            shouldAdd = cAddToRun.shouldAdd
                        };

                        SendUpdate(view, data);
                    }
                }
            }
        }

        [MessagePackObject(false)]
        public struct ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)] public bool shouldAdd;

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<AddToRunView>();

            public bool IsChangedFrom(ViewData cached)
            {
                return shouldAdd != cached.shouldAdd;
            }
        }

        public GameObject prefab;
        
        protected override void UpdateData(ViewData data)
        {
            prefab.SetActive(data.shouldAdd);
        }
    }
}