using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace NoDamage
{
    [BepInPlugin(PLUGIN_ID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_ID = "com.headpilgrim.nodamage";
        public const string PLUGIN_NAME = "No Ship Damage";
        public const string PLUGIN_VERSION = "1.0.0";

        private void Awake()
        {
            Logger.LogInfo("No Ship Damage mod loaded!");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_ID);
        }
    }
}