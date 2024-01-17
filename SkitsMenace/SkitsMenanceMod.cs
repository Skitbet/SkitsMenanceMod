using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SkitsMenace.Patches;
using System.Collections.Generic;

namespace SkitsMenace
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class SkitsMenanceMod : BaseUnityPlugin
    {
        private const string modGUID = "Skitbet.SkitsMenance";
        private const string modName = "SkitsMenance";
        private const string modVersion = "1.0.1";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static SkitsMenanceMod instance { get; private set; }

        internal ManualLogSource logger;

        internal List<SpawnableEnemyWithRarity> IndoorEnemyCache;
        internal List<SpawnableEnemyWithRarity> OutdoorEnemyCache;
        internal Dictionary<string, int> CurrentLevelDefaultRarities;


        internal bool EnemyCached; // doing this so we dont recache every new round

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            logger = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            logger.LogInfo("Patching SkitsMenance Mod...");

            harmony.PatchAll(typeof(RoundManagerPatch));

            logger.LogInfo("Patched SkitsMenance Mod!");


        }

    }
}
