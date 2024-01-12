//using GameNetcodeStuff;
//using HarmonyLib;

//namespace LethalCompanyPluginTemplate.Patches
//{
//    [HarmonyPatch(typeof(PlayerControllerB))]
//    public class PlayerControllerBPatch
//    {
//        //public StartOfRound playersManager;

//        [HarmonyPatch("Start")]
//        [HarmonyPrefix]
//        static void SuperRun(ref PlayerControllerB playerControllerB)
//        {
//            playerControllerB.movementSpeed = 30;
//            playerControllerB.jumpForce = 20;
//            Plugin.logger.LogInfo("movementSpeed: " + playerControllerB.movementSpeed);
//            Plugin.logger.LogInfo("jumpForce: " + playerControllerB.jumpForce);
//        }

//        [HarmonyPatch("AllowPlayerDeath")]
//        [HarmonyPrefix]
//        static bool OverrideDeath()
//        {
//            StartOfRound.Instance.allowLocalPlayerDeath = false;
//            Plugin.logger.LogInfo("allowLocalPlayerDeath: " + StartOfRound.Instance.allowLocalPlayerDeath);
//            if (!StartOfRound.Instance.allowLocalPlayerDeath)
//            {
//                return false;
//            }
//            return true;
//        }
//    }
//}
