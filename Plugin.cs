using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

/* PLUGIN BY BOB SAGET -  INSPIRED BY GAMEMASTER - VERY EARLY WORK IN PROGRESS */
namespace LethalCommands
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource logger;
        #region Game Fields
        internal static bool isHost;
        internal static PlayerControllerB hostPlayerRef;
        internal static PlayerControllerB myPlayerRef;
        private static SelectableLevel currentLevel;
        #endregion
        #region Command Fields
        internal static bool noclip = false;
        internal static bool godMode = false;
        internal static bool demiGod = false;
        internal static bool invisibility = false;
        internal static bool nightVision = false;
        internal static bool speedHack = false;
        internal static bool infiniteSprint = false;
        internal static bool infiniteJump = false;
        internal static bool infiniteCredits = false;
        internal static bool infiniteDeadline = false;
        internal static bool superJump = false;
        internal static float jumpForce = (float)13.0;
        internal static float movementSpeed = (float)4.6;
        internal static float nightVisionIntensity = 1000f;
        internal static float nightVisionRange = 10000f;
        internal static Color nightVisionColor = Color.green;
        #endregion
        private void Awake()
        {
            // TODO: Figure out why patch classes aren't working...? Why does everything have to be in this file...
            // Plugin startup logic
            logger = Logger;
            logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPatch(typeof(RoundManager), "Start")]
        [HarmonyPrefix]
        static void setIsHost()
        {
            isHost = RoundManager.Instance.NetworkManager.IsHost;
            logger.LogInfo("Host Status: " + isHost);
        }

        //[HarmonyPatch(typeof(PlayerControllerB), "Start")]
        //[HarmonyPostfix]
        //static void sethostPlayerRef(ref PlayerControllerB __instance)
        //{
        //    hostPlayerRef = __instance;
        //    logger.LogInfo("Current Player: " + hostPlayerRef.playerUsername);
        //}

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Update))]
        [HarmonyPrefix]
        // I'd refactor this - this runs way too frequently, and is really easy to cause performance/stability issues here
        // Try to move individial command/toggle logic to patch logic? IDK man this just doesn't sit right
        static void MovementCheats(ref PlayerControllerB __instance)
        {
            // Need to find a reliable way to find the host player without checking every Update - Start isn't reliable either
            if (__instance?.isHostPlayerObject ?? false)
            {
                hostPlayerRef = __instance;
                //logger.LogInfo("Found Host Player: " + hostPlayerRef.playerUsername);
            }
            if (speedHack)
            {
                __instance.movementSpeed = (float)movementSpeed;
            }
            else __instance.movementSpeed = (float)4.6;
            if (superJump)
            {
                __instance.jumpForce = (float)jumpForce;
            }
            else __instance.jumpForce = (float)13.0;
            // TODO: Add postfix that ignores indoors/outdoors rule for night vision
            if (nightVision && __instance.nightVision)
            {
                __instance.nightVision.enabled = true;
                __instance.nightVision.color = nightVisionColor;
                __instance.nightVision.intensity = nightVisionIntensity;
                __instance.nightVision.range = nightVisionRange;
            }
            else
            {
                __instance.nightVision.enabled = false;
                __instance.nightVision.color = Color.clear;
                __instance.nightVision.intensity = 0f;
                __instance.nightVision.range = 0f;
            }
            
            if (infiniteSprint)
            {
                Mathf.Clamp(__instance.sprintMeter += 0.02f, 0f, 1f);
            }
            //if (noclip)
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Jump_performed")]
        [HarmonyPostfix]
        static void InfiniteJump(ref PlayerControllerB __instance)
        {

            if (infiniteJump && !__instance.quickMenuManager.isMenuOpen && ((__instance.IsOwner && __instance.isPlayerControlled && (!__instance.IsServer || __instance.isHostPlayerObject)) || __instance.isTestingPlayer) && !__instance.inSpecialInteractAnimation && !__instance.isTypingChat && (__instance.isMovementHindered <= 0 || __instance.isUnderwater) && (!__instance.isPlayerSliding || __instance.playerSlidingTimer > 2.5f) && !__instance.isCrouching)
            {
                __instance.playerSlidingTimer = 0f;
                __instance.isJumping = true;
                //__instance.sprintMeter = Mathf.Clamp(__instance.sprintMeter - 0.08f, 0f, 1f);
                __instance.movementAudio.PlayOneShot(StartOfRound.Instance.playerJumpSFX);
                if (__instance.jumpCoroutine != null)
                {
                    __instance.StopCoroutine(__instance.jumpCoroutine);
                }

                __instance.jumpCoroutine = __instance.StartCoroutine(__instance.PlayerJump());
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "AllowPlayerDeath")]
        [HarmonyPrefix]
        static bool OverrideDeath()
        {
            StartOfRound.Instance.allowLocalPlayerDeath = !godMode;
            
            return !godMode;
        }

        [HarmonyPatch(typeof(Terminal), nameof(Terminal.RunTerminalEvents))]
        [HarmonyPostfix]
        static void InfiniteCredits(ref int ___groupCredits)
        {
            if (!isHost) { return; }
            if (infiniteCredits) { ___groupCredits = 69420; }
        }

        [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.MoveGlobalTime))]
        [HarmonyPostfix]
        static void InfiniteDeadline(ref float ___timeUntilDeadline)
        {
            if (!isHost) { return; }
            if (infiniteDeadline) { ___timeUntilDeadline = 5000; }

        }
        [HarmonyPatch(typeof(RoundManager), "AdvanceHourAndSpawnNewBatchOfEnemies")]
        [HarmonyPrefix]
        static void updateCurrentLevelInfo(ref SelectableLevel ___currentLevel)
        {
            currentLevel = ___currentLevel;
        }

        [HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
        [HarmonyPrefix]
        static void TextChatCommands(HUDManager __instance)
        {
            // TODO: REFACTOR THIS SPAGHETTIIIIIII
            // Define a regular expression pattern to match the number
            string numberExpression = @"\d+";
            string text = __instance.chatTextField.text;
            if (text.StartsWith('/'))
            {
                string alertTitle = "Error:";
                string alertBody = "Unknown Command";
                // Host Only Commands
                if (isHost)
                {
                    
                }

                if (text.ToLower().Contains("speed"))
                {
                    if (text.ToLower().StartsWith("/set"))
                    {
                        Match match = Regex.Match(text, numberExpression);
                        if (match.Success)
                        {
                            float speedVal = float.Parse(match.Value);
                            movementSpeed = speedVal;

                            alertTitle = "Movement Speed";
                            alertBody = "Movement Speed set to: " + movementSpeed.ToString();
                        }
                    } else
                    {
                        speedHack = !speedHack;
                        alertTitle = "Speed Hack";
                        alertBody = "Speed Hack set to: " + speedHack.ToString();
                    }

                }
                if (text.ToLower().Contains("jump"))
                {
                    if (text.ToLower().StartsWith("/set"))
                    {
                        Match match = Regex.Match(text, numberExpression);
                        if (match.Success)
                        {
                            float jumpForceVal = float.Parse(match.Value);
                            jumpForce = jumpForceVal;

                            alertTitle = "Jump Force";
                            alertBody = "Jump Force set to: " + jumpForce.ToString();
                        }
                    }
                    else
                    {
                        superJump = !superJump;
                        alertTitle = "Super Jump";
                        alertBody = "Super Jump set to: " + superJump.ToString();
                    }

                }

                if (text.ToLower().Contains("god"))
                {
                    godMode = !godMode;
                    alertTitle = "God Mode";
                    alertBody = "God Mode set to: " + godMode.ToString();
                }
                //if (text.ToLower().Contains("noclip"))
                //{
                //    noclip = !noclip;
                //    alertTitle = "Noclip";
                //    alertBody = "Noclip set to: " + noclip.ToString();
                // I don't wanna do the math to make this work :( Quaternions this, versors that...
                //}
                if (text.ToLower().Contains("vision"))
                {
                    nightVision = !nightVision;
                    alertTitle = "Night Vision";
                    alertBody = "Night Vision set to: " + nightVision.ToString();
                }
                if (text.ToLower().Contains("sprint"))
                {
                    infiniteSprint = !infiniteSprint;
                    alertTitle = "Infinite Sprint";
                    alertBody = "Infinite Sprint set to: " + infiniteSprint.ToString();
                }
                if (text.ToLower().StartsWith("/jumps"))
                {
                    infiniteJump = !infiniteJump;
                    alertTitle = "Infinite Jump";
                    alertBody = "Infinite Jump set to: " + infiniteJump.ToString();
                }
                if (text.ToLower().Contains("credits"))
                {
                    infiniteCredits = !infiniteCredits;
                    alertTitle = "Infinite Credits";
                    alertBody = "Infinite Credits set to: " + infiniteCredits.ToString();
                }
                if (text.ToLower().Contains("deadline"))
                {
                    infiniteDeadline = !infiniteDeadline;
                    alertTitle = "Infinite Deadline";
                    alertBody = "Infinite Deadline set to: " + infiniteDeadline.ToString();
                }
                //if (text.ToLower().Contains("invisib"))
                //{
                //    invisibility = !invisibility;
                //    alertTitle = "Invisibility";
                //    alertBody = "Invisibility set to : " + invisibility.ToString();
                //}
                // TODO: Add a submenu for SPAWNING (enemies, items, etc..)
                if (text.ToLower().Contains("shotgun"))
                {
                    //if (myPlayerRef != null || isHost)
                    //{
                    //    alertTitle = "Spawn Shotgun";
                    //    alertBody = "Spawned Shotgun at " + (isHost ? hostPlayerRef.playerUsername : myPlayerRef.playerUsername);
                    //    foreach (var enemy in currentLevel.Enemies)
                    //    {
                    //        if (enemy.enemyType.enemyName.ToLower().Contains("nutcracker"))
                    //        {
                    //            try
                    //            {
                    //                logger.LogInfo("Attempting to spawn shotgun for " + (isHost ? hostPlayerRef.playerUsername : myPlayerRef.playerUsername));
                    //                logger.LogInfo("Attempting to spawn nutcracker at player position: " + (isHost ? hostPlayerRef.transform.position.ToString() : myPlayerRef.transform.position.ToString()));
                    //                GameObject gameObject = Instantiate(enemy.enemyType.enemyPrefab, isHost ? hostPlayerRef.transform.position : myPlayerRef.transform.position, UnityEngine.Quaternion.Euler(UnityEngine.Vector3.zero));
                    //                NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
                    //                networkObject.Spawn();
                    //                logger.LogInfo("Attempted to spawn nutcracker at player position: " + (isHost ? hostPlayerRef.transform.position.ToString() : myPlayerRef.transform.position.ToString()));
                    //                logger.LogInfo("Actual nutcracker position: " + gameObject.transform.position);
                    //                NutcrackerEnemyAI nutcracker = gameObject.GetComponent<NutcrackerEnemyAI>();
                    //                GrabbableObject gun = gameObject.GetComponent<ShotgunItem>();
                    //                logger.LogInfo("Attempting to force drop Shotgun");
                    //                nutcracker.DropGun(hostPlayerRef.transform.position);

                    //                //networkObject.Despawn();
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                logger.LogError("Failed to spawn Nutcracker!");
                    //                logger.LogError(ex);
                    //            }
                    //            break;
                    //        }
                    //    }
                    //}
                }
                // TODO: Add a submenu for SPAWNING (enemies, items, etc..)
                if (text.ToLower().Contains("nutcracker"))
                {
                    alertTitle = "Spawn Nutcracker";
                    alertBody = "Spawned Nutcracker at " + (isHost ? hostPlayerRef.playerUsername : myPlayerRef.playerUsername);

                    foreach (var enemy in currentLevel.Enemies)
                    {
                        if (enemy.enemyType.enemyName.ToLower().Contains("nutcracker"))
                        {
                            try
                            {
                                logger.LogInfo("Attempting to spawn nutcracker at player position: " + (isHost ? hostPlayerRef.transform.position.ToString() : myPlayerRef.transform.position.ToString()));
                                //GameObject gameObject = Instantiate(enemy.enemyType.enemyPrefab, hostPlayerRef.transform.position, UnityEngine.Quaternion.Euler(UnityEngine.Vector3.zero), RoundManager.Instance.spawnedScrapContainer);
                                //gameObject.GetComponent<NetworkObject>().Spawn();
                                RoundManager.Instance.SpawnEnemyOnServer(isHost ? hostPlayerRef.transform.position : myPlayerRef.transform.position, isHost ? hostPlayerRef.transform.rotation.y : myPlayerRef.transform.rotation.y, currentLevel.Enemies.IndexOf(enemy));
                                logger.LogInfo("Attempted to spawn nutcracker at player position: " + (isHost ? hostPlayerRef.transform.position.ToString() : myPlayerRef.transform.position.ToString()));
                            }
                            catch (Exception ex)
                            {
                                logger.LogError("Failed to spawn Nutcracker!");
                                logger.LogError(ex);
                            }
                            break;
                        }
                    }
                }
                // Temporary fix - there has GOT to be an easier way to find which player instance represents your player (I think MoreCompany makes this harder than normal)
                if (text.ToLower().StartsWith("/username"))
                {
                    string[] words = text.ToLower().Split(' ');
                    if (words.Length == 2)
                    {
                        var players = StartOfRound.Instance.allPlayerScripts.ToList();
                        myPlayerRef = players.Find(player => player.playerUsername.ToLower().Contains(words[1].ToLower())) ?? null;
                        if (myPlayerRef != null)
                        {
                            alertTitle = "Assign Player Username";
                            alertBody = "Assigned Current Player to " + myPlayerRef.playerUsername;
                            logger.LogInfo("Matched Player: " + myPlayerRef.playerUsername);
                        } else
                        {
                            alertTitle = "Error: Assign Player Username";
                            alertBody = "Invalid Username: " + words[1];
                            logger.LogInfo("Invalid Username provided: " + words[1]);
                        }
                    }

                }
                if (text.ToLower().StartsWith("/teleport"))
                {
                    if (myPlayerRef != null || isHost)
                    {
                        alertTitle = "Teleport " + (isHost ? hostPlayerRef.playerUsername : myPlayerRef.playerUsername);
                        string[] words = text.ToLower().Split(' ');

                        if (words.Length == 2)
                        {
                            var players = StartOfRound.Instance.allPlayerScripts.ToList();
                            var matchedPlayer = players.Find(player => player.playerUsername.ToLower().Contains(words[1].ToLower())) ?? null;
                            //var playerIndex = players.IndexOf(matchedPlayer);

                            if (words[1].ToLower().Equals("ship"))
                            {
                                alertBody = "Teleport " + (isHost ? hostPlayerRef.playerUsername : myPlayerRef.playerUsername) + " to Ship";
                                logger.LogInfo("Current player position: " + (isHost ? hostPlayerRef.transform.position.ToString() : myPlayerRef.transform.position.ToString()));
                                logger.LogInfo("Attempting to teleport " + (isHost ? hostPlayerRef.playerUsername : myPlayerRef.playerUsername) + " to position: " + StartOfRound.Instance.playerSpawnPositions[0].transform.position.ToString());
                                if (isHost)
                                {
                                    hostPlayerRef.TeleportPlayer(StartOfRound.Instance.playerSpawnPositions[0].transform.position, false);
                                }
                                else myPlayerRef.TeleportPlayer(StartOfRound.Instance.playerSpawnPositions[0].transform.position, false);
                            }
                            if (matchedPlayer != null)
                            {

                                alertBody = "Teleport " + (isHost ? hostPlayerRef.playerUsername : myPlayerRef.playerUsername) + " to " + matchedPlayer.playerUsername;
                                logger.LogInfo("Current player position: " + (isHost ? hostPlayerRef.transform.position.ToString() : myPlayerRef.transform.position.ToString()));
                                logger.LogInfo("Attempting to teleport " + (isHost ? hostPlayerRef.playerUsername : myPlayerRef.playerUsername) + " to " + matchedPlayer.playerUsername + " at position: " + matchedPlayer.transform.position.ToString());

                                if (isHost)
                                {
                                    hostPlayerRef.TeleportPlayer(matchedPlayer.transform.position, false);
                                }
                                else myPlayerRef.TeleportPlayer(matchedPlayer.transform.position, false);
                            }
                        }
                    } else
                    {
                        alertTitle = "Error: No Player Assigned";
                        alertBody = "Use /username yourusernamehere";
                    }
                    
                }
                // sends notice to user about what they have done
                HUDManager.Instance.DisplayTip(alertTitle, alertBody);
                // Hide the message from chat
                __instance.chatTextField.text = "";
                return;
            }
        }
        [HarmonyPatch(typeof(RoundManager), "EnemyCannotBeSpawned")]
        [HarmonyPostfix]
        // Ignores if spawning is disabled, enemy power levels, and maxCount
        static bool OverrideEnemySpawn()
        {
            // Should add this behind a toggle... may cause some unintended funky behavior
            logger.LogInfo("Force Spawned Enemy -> EnemyCannotBeSpawned: false");
            return false;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "SetNightVisionEnabled")]
        [HarmonyPostfix]
        static void enableNightVision(ref PlayerControllerB __instance)
        {
            __instance.nightVision.enabled = nightVision;
        }
    }
}