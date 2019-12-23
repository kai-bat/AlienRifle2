using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using UnityEngine;
using Harmony;

namespace AlienRifle2
{
    public static class MainPatch
    {
        public static TechType rifleTech;
        public static AssetBundle bundle;

        public static void Patch()
        {
            bundle = AssetBundle.LoadFromFile("QMods/AlienRifle2/alienrifle");

            rifleTech = TechTypeHandler.AddTechType("AlienRifleWeapon", "Alien Rifle", "An ancient weapon found in an alien facility", ImageUtils.LoadSpriteFromFile("QMods/AlienRifle2/Assets/alienrifle.png"), false);

            PrefabHandler.RegisterPrefab(new RiflePrefab("AlienRifleWeapon", "WorldEntities/Tools/AlienRifle", rifleTech));

            CraftDataHandler.SetEquipmentType(rifleTech, EquipmentType.Hand);
            TechData data = new TechData();
            data.Ingredients = new List<Ingredient>()
            {
                new Ingredient(TechType.StasisRifle, 1),
                new Ingredient(TechType.Magnetite, 3),
                new Ingredient(TechType.PlasteelIngot, 2),
                new Ingredient(TechType.PrecursorIonCrystal, 1)
            };
            data.craftAmount = 1;

            CraftDataHandler.SetTechData(rifleTech, data);
            CraftDataHandler.SetItemSize(rifleTech, 2, 2);
            CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "StasisRifleMods", "Stasis Rifle Upgrades", ImageUtils.LoadSpriteFromFile("QMods/AlienRifle2/Assets/stasisrifleupgrades.png"));
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Workbench, rifleTech, "StasisRifleMods", "Alien Rifle");

            HarmonyInstance inst = HarmonyInstance.Create("Kylinator25.AlienRifle.V2");
            inst.PatchAll(typeof(MainPatch).Assembly);
        }
    }
}