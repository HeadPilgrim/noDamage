using HarmonyLib;
using UnityEngine;

namespace noDamage
{
    internal class Patches
    {
        internal static Material damageMat;

        // Original BoatDamageFix functionality - adds missing values to modded ships
        [HarmonyPatch(typeof(BoatDamageWater))]
        private static class BoatDamageWaterPatch
        {
            [HarmonyPatch("Start")]
            [HarmonyPrefix]
            public static void StartPrefix(BoatDamageWater __instance)
            {
                // Cache damage material (but we won't apply it to avoid texture glitches)
                if (!damageMat)
                {
                    try
                    {
                        damageMat = SaveLoadManager.instance.GetCurrentObjects()[10]
                            .GetComponentInChildren<HullDamageTexture>()
                            .GetComponent<Renderer>().material;
                    }
                    catch
                    {
                        // Silently fail if material can't be found
                    }
                }

                // If modded ship has no damage values, add them (original mod functionality)
                if (__instance.damage.durabilityDays == 0f)
                {
                    float mass = __instance.damage.GetComponent<Rigidbody>().mass;
                    float num = mass / 12f + 50f;
                    if (num > 500f)
                    {
                        num = mass / 30f + 300f;
                    }
                    __instance.damage.waterUnitsCapacity = Mathf.Round(num);
                    float num2 = Mathf.InverseLerp(60f, 2000f, mass / 20f + 50f);
                    num2 = Mathf.Lerp(120f, 60f, num2);
                    __instance.damage.durabilityDays = Mathf.Round(num2);
                    __instance.damage.wearSteepness = 0.03f;
                    __instance.damage.impactDamageMult = 0.014f;
                    __instance.damage.minimumImpactVelocity = 1.5f;
                }

                // Add button component if missing (but skip the material/texture stuff)
                if (__instance.GetComponent<BoatDamageWaterButton>() == null)
                {
                    MeshCollider meshCollider = __instance.gameObject.AddComponent<MeshCollider>();
                    meshCollider.isTrigger = true;
                    meshCollider.convex = true;
                    __instance.gameObject.AddComponent<BoatDamageWaterButton>();

                    // Only do the texture stuff if we have a valid material
                    if (damageMat != null)
                    {
                        try
                        {
                            GameObject gameObject = __instance.damage.gameObject
                                .GetComponent<SaveableObject>()
                                .GetCleanable().gameObject;
                            GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, gameObject.transform.parent);
                            gameObject2.GetComponent<Renderer>().material = damageMat;
                            UnityEngine.Object.Destroy(gameObject2.GetComponent<CleanableObject>());
                            UnityEngine.Object.Destroy(gameObject2.GetComponent<Collider>());
                            UnityEngine.Object.Destroy(gameObject2.GetComponent<HullPlayerCollider>());
                            gameObject2.AddComponent<HullDamageTexture>().damage = __instance.damage;
                            gameObject2.layer = 2;
                        }
                        catch
                        {
                            // Silently fail if texture setup fails
                        }
                    }
                }
            }
        }

        // Our no-damage patches
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
        }
    }
}