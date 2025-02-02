using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
namespace BBAchievements
{
    [HarmonyPatch]
    class AchievementsPatches
    {
        private static Dictionary<string, bool> datas = new Dictionary<string, bool>();
        [HarmonyPatch(typeof(GameLoader), "Initialize")]
        [HarmonyPostfix]
        private static void SetupData()
        {
            datas = new Dictionary<string, bool>
            {
                { "BBA_StealthPerfection1", true },
                { "BBA_StealthPerfection2", true }
            };
        }
        [HarmonyPatch(typeof(PlaceholderWinManager), "BeginPlay")]
        [HarmonyPrefix]
        private static void OnWin()
        {
            Achievement.Get("BBA_VictorySong").Unlock();
            foreach (var data in datas)
            {
                if (data.Value)
                    Achievement.Get(data.Key).Unlock();
            }
            if (CoreGameManager.Instance.inventoryChallenge && CoreGameManager.Instance.mapChallenge && CoreGameManager.Instance.timeLimitChallenge)
                Achievement.Get("BBA_NotBBCR").Unlock();
        }
        [HarmonyPatch(typeof(Baldi_StateBase), "PlayerInSight")]
        [HarmonyPostfix]
        private static void StealthPerfectionFail()
        {
            datas["BBA_StealthPerfection1"] = false;
            datas["BBA_StealthPerfection2BBA_StealthPerfection1"] = false;
        }
        [HarmonyPatch(typeof(Baldi), "Hear")]
        [HarmonyPrefix]
        private static void StealthPerfection2Fail(int value)
        {
            datas["BBA_StealthPerfection2"] = false;
        }
        [HarmonyPatch(typeof(DetentionRoomFunction), "Activate")]
        [HarmonyPrefix]
        private static void GetDetention(float time)
        {
            if (time == 99)
                Achievement.Get("BBA_99Seconds").Unlock();
        }
    }
}
