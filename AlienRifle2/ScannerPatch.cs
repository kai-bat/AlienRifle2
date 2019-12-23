﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace AlienRifle2
{
    [HarmonyPatch(typeof(PDAScanner))]
    [HarmonyPatch(nameof(PDAScanner.Unlock))]
    public static class ScannerPatch
    {
        public static bool Prefix(ref PDAScanner.EntryData entryData, ref bool unlockBlueprint)
        {
            if (entryData.key == TechType.PrecursorPrisonArtifact7)
            {
                unlockBlueprint = true;
                entryData.blueprint = MainPatch.rifleTech;

                string key = "EncyDesc_PrecursorPrisonArtifact7";

                string oldEncy = Language.main.Get(key);

                if (Language.main.currentLanguage == "English")
                {
                    Language.main.strings[key] = oldEncy +
                        "\n\nDespite this, it was possible to synthesise a blueprint for the rifle, but it will require a Stasis Rifle to use as a base.";
                }
            }

            return true;
        }
    }
}
