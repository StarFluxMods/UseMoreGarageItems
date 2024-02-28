using KitchenLib;
using KitchenLib.Logging;
using KitchenLib.Logging.Exceptions;
using KitchenMods;
using System.Linq;
using System.Reflection;
using KitchenData;
using KitchenLib.Event;
using KitchenLib.References;
using KitchenLib.Utils;
using UnityEngine;
using UseMoreGarageItems.Components;
using UseMoreGarageItems.Views;

namespace UseMoreGarageItems
{
    public class Mod : BaseMod, IModSystem
    {
        public const string MOD_GUID = "com.starfluxgames.usemoregarageitems";
        public const string MOD_NAME = "Use More Garage Items";
        public const string MOD_VERSION = "0.1.1";
        public const string MOD_AUTHOR = "StarFluxGames";
        public const string MOD_GAMEVERSION = ">=1.1.9";

        public static AssetBundle Bundle;
        public static KitchenLogger Logger;

        public Mod() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            Logger.LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
        }

        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            Bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).FirstOrDefault() ?? throw new MissingAssetBundleException(MOD_GUID);
            Logger = InitLogger();

            Events.BuildGameDataEvent += (s, args) =>
            {
                if (!args.firstBuild) return;
                
                Appliance shelf = args.gamedata.Get<Appliance>(ApplianceReferences.GarageShelf);
                GameObject prefab = Bundle.LoadAsset<GameObject>("Notify").AssignMaterialsByNames();
                prefab.transform.parent = shelf.Prefab.transform;
                prefab.transform.localPosition = new Vector3(0, 0, 0);
                AddToRunView view = prefab.AddComponent<AddToRunView>();
                view.prefab = prefab.GetChild(0);
                shelf.Properties.Add(new CAddToRun());
            };
        }
    }
}

