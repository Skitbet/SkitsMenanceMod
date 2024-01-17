using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkitsMenace.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch
    {

        [HarmonyPatch(nameof(RoundManager.LoadNewLevel))]
        [HarmonyPrefix]
        static void ModifyLevel(ref SelectableLevel newLevel)
        {
            if (SkitsMenanceMod.instance.CurrentLevelDefaultRarities != null)
            {
                SkitsMenanceMod.instance.CurrentLevelDefaultRarities.Clear();
                SkitsMenanceMod.instance.CurrentLevelDefaultRarities = null;
            }
            SkitsMenanceMod.instance.CurrentLevelDefaultRarities = GetDefaultRarities(newLevel);

            if (!SkitsMenanceMod.instance.EnemyCached)
            {
                SkitsMenanceMod.instance.logger.LogInfo("Caching....");
                SkitsMenanceMod.instance.EnemyCached = true;
                SkitsMenanceMod.instance.IndoorEnemyCache = FixIndoorEnemySpawns();
                SkitsMenanceMod.instance.OutdoorEnemyCache = FixOutDoorEnemySpawns();
            }

            newLevel.Enemies = SkitsMenanceMod.instance.IndoorEnemyCache;
            newLevel.OutsideEnemies = SkitsMenanceMod.instance.OutdoorEnemyCache;


     /*       foreach (SpawnableEnemyWithRarity spawnable in newLevel.Enemies)
            {
                SkitsMenanceMod.instance.logger.LogInfo(spawnable.enemyType.enemyName + " " + spawnable.rarity);
            }

            foreach (SpawnableEnemyWithRarity spawnable in newLevel.OutsideEnemies)
            {
                SkitsMenanceMod.instance.logger.LogInfo(spawnable.enemyType.enemyName + " " + spawnable.rarity);
            }*/

        }

        internal static Dictionary<string, int> GetDefaultRarities(SelectableLevel newLevel)
        {
            Dictionary<string, int> toReturn = new Dictionary<string, int>();

            foreach (SpawnableEnemyWithRarity spawnable in newLevel.Enemies)
            {
                toReturn.Add(spawnable.enemyType.enemyName, (spawnable.rarity <= 5 ? UnityEngine.Random.Range(6, 100) : spawnable.rarity));
            }

            foreach (SpawnableEnemyWithRarity spawnable in newLevel.OutsideEnemies)
            {
                toReturn.Add(spawnable.enemyType.enemyName, (spawnable.rarity <= 5 ? UnityEngine.Random.Range(6, 100) : spawnable.rarity));
            }

            return toReturn;
        }

        internal static List<SpawnableEnemyWithRarity> FixIndoorEnemySpawns()
        {
            SkitsMenanceMod.instance.logger.LogInfo("Caching indoor enemies.");
            List<SpawnableEnemyWithRarity> returnList = new List<SpawnableEnemyWithRarity>();

            returnList.Add(getEntitySpawner<JesterAI>());
            returnList.Add(getEntitySpawner<DressGirlAI>());
            returnList.Add(getEntitySpawner<SandSpiderAI>());
            returnList.Add(getEntitySpawner<CrawlerAI>());
            returnList.Add(getEntitySpawner<BlobAI>());
            returnList.Add(getEntitySpawner<CentipedeAI>());
            returnList.Add(getEntitySpawner<FlowermanAI>());
            returnList.Add(getEntitySpawner<HoarderBugAI>());
            returnList.Add(getEntitySpawner<LassoManAI>());
            returnList.Add(getEntitySpawner<PufferAI>());
            returnList.Add(getEntitySpawner<SpringManAI>());
            returnList.Add(getEntitySpawner<NutcrackerEnemyAI>());

            SkitsMenanceMod.instance.logger.LogInfo("Cached indoor enemies.");

            return returnList;
        }

        internal static List<SpawnableEnemyWithRarity> FixOutDoorEnemySpawns()
        {
            SkitsMenanceMod.instance.logger.LogInfo("Caching outdoor enemies.");

            List<SpawnableEnemyWithRarity> returnList = new List<SpawnableEnemyWithRarity>();

            returnList.Add(getEntitySpawner<DressGirlAI>());
            returnList.Add(getEntitySpawner<SandWormAI>());
            returnList.Add(getEntitySpawner<MouthDogAI>());
            returnList.Add(getEntitySpawner<BaboonBirdAI>());
            returnList.Add(getEntitySpawner<DocileLocustBeesAI>());
            returnList.Add(getEntitySpawner<DoublewingAI>());
            returnList.Add(getEntitySpawner<ForestGiantAI>());

            SkitsMenanceMod.instance.logger.LogInfo("Cached outdoor enemies.");

            return returnList;
        }
        internal static SpawnableEnemyWithRarity getEntitySpawner<T>() where T : EnemyAI
        {
            T refToEnemy = null;
            foreach (UnityEngine.Object o in Resources.FindObjectsOfTypeAll(typeof(T)))
            {
                refToEnemy = (T)o;
            }
            SpawnableEnemyWithRarity spawnable = new SpawnableEnemyWithRarity();

            if (refToEnemy != null)
            {
 /*               foreach (var pair in SkitsMenanceMod.instance.CurrentLevelDefaultRarities)
                {
                    Console.WriteLine($"Enemy Type: {pair.Key.name}, Value: {pair.Value}");
                }*/
                spawnable.enemyType = refToEnemy.enemyType;
                int rarity;
                // for some reason enemies arent in rarities sometimes
                if (SkitsMenanceMod.instance.CurrentLevelDefaultRarities.ContainsKey(refToEnemy.enemyType.enemyName))
                {
                    rarity = SkitsMenanceMod.instance.CurrentLevelDefaultRarities[refToEnemy.enemyType.enemyName];
                } 
                else
                {
                    SkitsMenanceMod.instance.logger.LogInfo("Couldn't find cached rareity for this level for the " + spawnable.enemyType.enemyName + ", defaulting to random value.");
                    rarity = UnityEngine.Random.Range(6, 100);
                }
                spawnable.rarity = rarity;
            }
            return spawnable;
        }
    }
}
