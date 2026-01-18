using HarmonyLib;
using UnityEngine;

namespace noDamage
{
    internal class Patches
    {
        [HarmonyPatch(typeof(BoatDamage))]
        private static class BoatDamagePatch
        {
            // Block collision damage
            [HarmonyPatch("Impact")]
            [HarmonyPrefix]
            public static bool ImpactPrefix()
            {
                return false;
            }

            // Block daily wear damage
            [HarmonyPatch("DailyDamage")]
            [HarmonyPrefix]
            public static bool DailyDamagePrefix()
            {
                return false;
            }

            // Block water overflow from waves
            [HarmonyPatch("Overflow")]
            [HarmonyPrefix]
            public static bool OverflowPrefix()
            {
                return false;
            }

            // Reset damage values on Start
            [HarmonyPatch("Start")]
            [HarmonyPostfix]
            public static void StartPostfix(BoatDamage __instance)
            {
                __instance.hullDamage = 0f;
                __instance.waterLevel = 0f;
                __instance.durabilityDays = 999999f;
                __instance.impactDamageMult = 0f;
                __instance.wearSteepness = 0f;
                __instance.waterIntakeRate = 0f;
                __instance.minimumImpactVelocity = 99999f;
            }
        }
    }
}