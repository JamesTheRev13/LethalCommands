using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using LethalCommands.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Netcode;
using UnityEngine;

/* PLUGIN BY BOB SAGET -  INSPIRED BY GAMEMASTER and DANCETOOLS (thank you for the inspiration to use the Command Pattern) - VERY EARLY WORK IN PROGRESS */
// TODO: Very much in need of a heavy refactor - too much spaghetti - DANCETOOLS is a great example to model after
namespace LethalCommands
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public ManualLogSource logger;
        public static Plugin plugin;
        // Command Pattern - https://refactoring.guru/design-patterns/command
        public CommandFactory commandFactory;

        #region Game Fields
        public bool isHost;
        public PlayerControllerB hostPlayerRef;
        public PlayerControllerB myPlayerRef;
        private SelectableLevel currentLevel;
        //public AllItemsList allItemsList;
        #endregion
        #region Command Fields
        public bool noclip = false;
        public bool godMode { get; set; } = false;
        public bool demiGod = false;
        public bool invisibility = false;
        public bool infiniteAmmo = false;
        public bool nightVision = false;
        public bool speedHack = false;
        public bool infiniteSprint = false;
        public bool infiniteJump = false;
        public bool infiniteCredits = false;
        public bool infiniteDeadline = false;
        public bool allDoorsUnlockable = false;
        public bool superJump = false;
        public float jumpForce = (float)13.0;
        public float movementSpeed = (float)4.6;
        public float noclipSpeed = 0.5f;
        public float nightVisionIntensity = 1000f;
        public float nightVisionRange = 10000f;
        public Color nightVisionColor = Color.green;
        #endregion
        private void Awake()
        {
            // TODO: Figure out why patch classes aren't working...? Why does everything have to be in this file...
            // Plugin startup logic
            logger = Logger;
            plugin = this;
            // Create an instance of CommandFactory with access to MainClass and logger
            commandFactory = new CommandFactory(plugin, logger);
            logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPatch(typeof(RoundManager), "Start")]
        [HarmonyPrefix]
        static void setIsHost()
        {
            plugin.isHost = RoundManager.Instance.NetworkManager.IsHost;
            plugin.logger.LogInfo("Host Status: " + plugin.isHost);
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
            // Maybe StartOfRound.localPlayerController?
            // GameNetworkManager.Instance.localPlayerController
            if (__instance?.isHostPlayerObject ?? false)
            {
                plugin.hostPlayerRef = __instance;
                //logger.LogInfo("Found Host Player: " + hostPlayerRef.playerUsername);
            }
            if (plugin.speedHack)
            {
                __instance.movementSpeed = (float)plugin.movementSpeed;
            }
            else __instance.movementSpeed = (float)4.6;
            if (plugin.superJump)
            {
                __instance.jumpForce = (float)plugin.jumpForce;
            }
            else __instance.jumpForce = (float)13.0;
            // TODO: Add postfix that ignores indoors/outdoors rule for night vision
            if (plugin.nightVision && __instance.nightVision)
            {
                __instance.nightVision.enabled = true;
                __instance.nightVision.color = plugin.nightVisionColor;
                __instance.nightVision.intensity = plugin.nightVisionIntensity;
                __instance.nightVision.range = plugin.nightVisionRange;
            }
            else
            {
                __instance.nightVision.enabled = false;
                __instance.nightVision.color = Color.clear;
                __instance.nightVision.intensity = 0f;
                __instance.nightVision.range = 0f;
            }
            
            if (plugin.infiniteSprint)
            {
                Mathf.Clamp(__instance.sprintMeter += 0.02f, 0f, 1f);
            }

            plugin.NoClip();
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Jump_performed")]
        [HarmonyPostfix]
        static void InfiniteJump(ref PlayerControllerB __instance)
        {

            if (plugin.infiniteJump && !__instance.quickMenuManager.isMenuOpen && ((__instance.IsOwner && __instance.isPlayerControlled && (!__instance.IsServer || __instance.isHostPlayerObject)) || __instance.isTestingPlayer) && !__instance.inSpecialInteractAnimation && !__instance.isTypingChat && (__instance.isMovementHindered <= 0 || __instance.isUnderwater) && (!__instance.isPlayerSliding || __instance.playerSlidingTimer > 2.5f) && !__instance.isCrouching)
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
            StartOfRound.Instance.allowLocalPlayerDeath = !plugin.godMode;
            plugin.logger.LogInfo("OverrideDeath: " + plugin.godMode.ToString());
            return !plugin.godMode;
        }

        [HarmonyPatch(typeof(Terminal), nameof(Terminal.RunTerminalEvents))]
        [HarmonyPostfix]
        static void InfiniteCredits(ref int ___groupCredits)
        {
            if (!plugin.isHost) { return; }
            if (plugin.infiniteCredits) { ___groupCredits = 69420; }
        }

        [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.MoveGlobalTime))]
        [HarmonyPostfix]
        static void InfiniteDeadline(ref float ___timeUntilDeadline)
        {
            if (!plugin.isHost) { return; }
            if (plugin.infiniteDeadline) { ___timeUntilDeadline = 5000; }

        }
        [HarmonyPatch(typeof(RoundManager), "AdvanceHourAndSpawnNewBatchOfEnemies")]
        [HarmonyPrefix]
        static void updateCurrentLevelInfo(ref SelectableLevel ___currentLevel)
        {
            plugin.currentLevel = ___currentLevel;
        }

        [HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
        [HarmonyPrefix]
        static void TextChatCommands(HUDManager __instance)
        {
            // TODO: REFACTOR THIS SPAGHETTIIIIIII
            // Define a regular expression pattern to match the number
            string numberExpression = @"\d+";
            string text = __instance.chatTextField.text;
            ICommand command = plugin.commandFactory.CreateCommand(text);
            if (command != null)
            {
                command.SetParameters(text);
                command.Execute();
            }
            //if (text.StartsWith('/'))
            //{
            //    string alertTitle = "Error:";
            //    string alertBody = "Unknown Command";
            //    // Host Only Commands
            //    if (plugin.isHost)
            //    {
                    
            //    }

            //    if (text.ToLower().Contains("speed"))
            //    {
            //        if (text.ToLower().StartsWith("/set"))
            //        {
            //            Match match = Regex.Match(text, numberExpression);
            //            if (match.Success)
            //            {
            //                float speedVal = float.Parse(match.Value);
            //                plugin.movementSpeed = speedVal;

            //                alertTitle = "Movement Speed";
            //                alertBody = "Movement Speed set to: " + plugin.movementSpeed.ToString();
            //            }
            //        } else
            //        {
            //            plugin.speedHack = !plugin.speedHack;
            //            alertTitle = "Speed Hack";
            //            alertBody = "Speed Hack set to: " + plugin.speedHack.ToString();
            //        }

            //    }
            //    if (text.ToLower().Contains("jump"))
            //    {
            //        if (text.ToLower().StartsWith("/set"))
            //        {
            //            Match match = Regex.Match(text, numberExpression);
            //            if (match.Success)
            //            {
            //                float jumpForceVal = float.Parse(match.Value);
            //                plugin.jumpForce = jumpForceVal;

            //                alertTitle = "Jump Force";
            //                alertBody = "Jump Force set to: " + plugin.jumpForce.ToString();
            //            }
            //        }
            //        else
            //        {
            //            plugin.superJump = !plugin.superJump;
            //            alertTitle = "Super Jump";
            //            alertBody = "Super Jump set to: " + plugin.superJump.ToString();
            //        }

            //    }

            //    if (text.ToLower().Contains("god"))
            //    {
            //        plugin.godMode = !plugin.godMode;
            //        alertTitle = "God Mode";
            //        alertBody = "God Mode set to: " + plugin.godMode.ToString();
            //    }
            //    if (text.ToLower().Contains("ammo"))
            //    {
            //        plugin.infiniteAmmo = !plugin.infiniteAmmo;
            //        alertTitle = "Infinite Ammo";
            //        alertBody = "Infinite Ammo set to: " + plugin.infiniteAmmo.ToString();
            //    }
            //    if (text.ToLower().Contains("noclip"))
            //    {
            //        if (text.ToLower().StartsWith("/set"))
            //        {
            //            Match match = Regex.Match(text, numberExpression);
            //            if (match.Success)
            //            {
            //                plugin.noclipSpeed = float.Parse(match.Value);

            //                alertTitle = "Noclip";
            //                alertBody = "Noclip speed set to: " + plugin.noclipSpeed.ToString();
            //            }
            //        } else
            //        {
            //            plugin.noclip = !plugin.noclip;
            //            alertTitle = "Noclip";
            //            alertBody = "Noclip set to: " + plugin.noclip.ToString();
            //        }
            //    }
            //    if (text.ToLower().Contains("vision"))
            //    {
            //        plugin.nightVision = !plugin.nightVision;
            //        alertTitle = "Night Vision";
            //        alertBody = "Night Vision set to: " + plugin.nightVision.ToString();
            //    }
            //    if (text.ToLower().Contains("sprint"))
            //    {
            //        plugin.infiniteSprint = !plugin.infiniteSprint;
            //        alertTitle = "Infinite Sprint";
            //        alertBody = "Infinite Sprint set to: " + plugin.infiniteSprint.ToString();
            //    }
            //    if (text.ToLower().StartsWith("/jumps"))
            //    {
            //        plugin.infiniteJump = !plugin.infiniteJump;
            //        alertTitle = "Infinite Jump";
            //        alertBody = "Infinite Jump set to: " + plugin.infiniteJump.ToString();
            //    }
            //    if (text.ToLower().Contains("credits"))
            //    {
            //        plugin.infiniteCredits = !plugin.infiniteCredits;
            //        alertTitle = "Infinite Credits";
            //        alertBody = "Infinite Credits set to: " + plugin.infiniteCredits.ToString();
            //    }
            //    if (text.ToLower().Contains("deadline"))
            //    {
            //        plugin.infiniteDeadline = !plugin.infiniteDeadline;
            //        alertTitle = "Infinite Deadline";
            //        alertBody = "Infinite Deadline set to: " + plugin.infiniteDeadline.ToString();
            //    }
            //    if (text.ToLower().Contains("unlock"))
            //    {
            //        alertTitle = "Doors";
            //        alertBody = "Unlocked All Doors";
            //        List<DoorLock> doorLocks = FindObjectsOfType<DoorLock>().ToList();

            //        foreach (DoorLock door in doorLocks)
            //        {
            //            plugin.logger.LogInfo("Found Door (" + door.GetInstanceID() + ") Locked? -> " + door.isLocked.ToString());
            //            if (door.isLocked)
            //            {
            //                door.UnlockDoorSyncWithServer();
            //                plugin.logger.LogInfo("Unlocked Door (" + door?.GetInstanceID() + ") -> " + door?.isLocked.ToString());
            //            }
            //        }
            //    }
            //    // Not working
            //    //if (text.ToLower().Contains("door"))
            //    //{
            //    //    alertTitle = "Door";
            //    //    alertBody = "Opened All Doors";
            //    //    Door[] doors = FindObjectsOfType<Door>();
            //    //    foreach (Door door in doors)
            //    //    {
            //    //        logger.LogInfo("Found Door (" + door.GetInstanceID() + ") Opened? -> " + door.IsOpen.ToString());
            //    //        if (!door.IsOpen)
            //    //        {
            //    //            door.isOpen = true;
            //    //            door.IsOpen = true;
            //    //            door.SetDoorState(door.IsOpen);
            //    //            logger.LogInfo("Opened Door (" + door.GetInstanceID() + ") -> " + door.IsOpen.ToString());
            //    //        }
            //    //    }
            //    //}
            //    //if (text.ToLower().Contains("invisib"))
            //    //{
            //    //    invisibility = !invisibility;
            //    //    alertTitle = "Invisibility";
            //    //    alertBody = "Invisibility set to : " + invisibility.ToString();
            //    //}
            //    // TODO: Add a submenu for SPAWNING (enemies, items, etc..)
            //    if (text.ToLower().StartsWith("/item"))
            //    {
            //        if (plugin.myPlayerRef != null || plugin.isHost)
            //        {
            //            string[] words = text.ToLower().Split(' ');
            //            // can spawn using /item <name>  OR  /item <name> <count>
            //            // maybe add another arg for position? to spawn items on other players?
            //            if (words.Length == 2 || words.Length == 3)
            //            {
            //                alertTitle = "Spawn Item";
            //                AllItemsList allItemsList = StartOfRound.Instance.allItemsList;
            //                allItemsList.itemsList
            //                    .ForEach(item =>
            //                    {
            //                        plugin.logger.LogInfo("Item Name: " + item.itemName);
            //                        plugin.logger.LogInfo("Item ID: " + item.itemId);
            //                    }
            //                );
            //                Item item = allItemsList?.itemsList.Find(item => item.itemName.ToLower().Contains(words[1].Trim().ToLower())) ?? null;

            //                plugin.logger.LogInfo("Found Item: " + item?.itemName);

            //                if (item != null)
            //                {
            //                    Vector3 spawnPosition = GameNetworkManager.Instance.localPlayerController.transform.position;
            //                    if (GameNetworkManager.Instance.localPlayerController.isPlayerDead)
            //                    {
            //                        spawnPosition = GameNetworkManager.Instance.localPlayerController.spectatedPlayerScript.transform.position;
            //                    }
                                
            //                    int count = 1;

            //                    if (words.Length == 3)
            //                    {
            //                        try
            //                        {
            //                            var countInput = int.Parse(words[2]);
            //                            if (countInput > 0)
            //                            {
            //                                count = countInput;
            //                            }
            //                        } catch 
            //                        {
            //                            count = 0;
            //                            alertBody = "Invalid Item Count";
            //                        }
            //                    }
            //                    for (int i = 0; i < count; i++)
            //                    {
            //                        GameObject itemObj = Instantiate(item.spawnPrefab, spawnPosition, Quaternion.identity);
            //                        itemObj.GetComponent<GrabbableObject>().fallTime = 0f;
            //                        int scrapValue = UnityEngine.Random.Range(60, 200);
            //                        itemObj.AddComponent<ScanNodeProperties>().scrapValue = scrapValue;
            //                        // setting a random scrap value for now, maybe make this configurable?
            //                        itemObj.GetComponent<GrabbableObject>().SetScrapValue(scrapValue);
            //                        if (text.Contains("shotgun") && plugin.infiniteAmmo)
            //                            itemObj.GetComponent<ShotgunItem>().shellsLoaded = 2147483647;
            //                        itemObj.GetComponent<NetworkObject>().Spawn();
            //                        plugin.logger.LogInfo("Attempted to spawn item!");
            //                        alertBody = "Spawned " + item.itemName + " at " + (plugin.isHost ? plugin.hostPlayerRef.playerUsername : plugin.myPlayerRef.playerUsername);
            //                    }
            //                } else
            //                {
            //                    alertBody = "Invalid Item: " + words[1];
            //                }
            //            }
            //        }
            //    }
            //    // TODO: Add a submenu for SPAWNING (enemies, items, etc..)
            //    if (text.ToLower().Contains("nutcracker"))
            //    {
            //        alertTitle = "Spawn Nutcracker";
            //        alertBody = "Spawned Nutcracker at " + (plugin.isHost ? plugin.hostPlayerRef.playerUsername : plugin.myPlayerRef.playerUsername);

            //        foreach (var enemy in plugin.currentLevel.Enemies)
            //        {
            //            if (enemy.enemyType.enemyName.ToLower().Contains("nutcracker"))
            //            {
            //                try
            //                {
            //                    plugin.logger.LogInfo("Attempting to spawn nutcracker at player position: " + (plugin.isHost ? plugin.hostPlayerRef.transform.position.ToString() : plugin.myPlayerRef.transform.position.ToString()));
            //                    //GameObject gameObject = Instantiate(enemy.enemyType.enemyPrefab, hostPlayerRef.transform.position, UnityEngine.Quaternion.Euler(UnityEngine.Vector3.zero), RoundManager.Instance.spawnedScrapContainer);
            //                    //gameObject.GetComponent<NetworkObject>().Spawn();
            //                    RoundManager.Instance.SpawnEnemyOnServer(plugin.isHost ? plugin.hostPlayerRef.transform.position : plugin.myPlayerRef.transform.position, plugin.isHost ? plugin.hostPlayerRef.transform.rotation.y : plugin.myPlayerRef.transform.rotation.y, plugin.currentLevel.Enemies.IndexOf(enemy));
            //                    plugin.logger.LogInfo("Attempted to spawn nutcracker at player position: " + (plugin.isHost ? plugin.hostPlayerRef.transform.position.ToString() : plugin.myPlayerRef.transform.position.ToString()));
            //                }
            //                catch (Exception ex)
            //                {
            //                    plugin.logger.LogError("Failed to spawn Nutcracker!");
            //                    plugin.logger.LogError(ex);
            //                }
            //                break;
            //            }
            //        }
            //    }
            //    // Temporary fix - there has GOT to be an easier way to find which player instance represents your player (I think MoreCompany makes this harder than normal)
            //    if (text.ToLower().StartsWith("/username"))
            //    {
            //        string[] words = text.ToLower().Split(' ');
            //        if (words.Length == 2)
            //        {
            //            var players = StartOfRound.Instance.allPlayerScripts.ToList();
            //            plugin.myPlayerRef = players.Find(player => player.playerUsername.ToLower().Contains(words[1].ToLower())) ?? null;
            //            if (plugin.myPlayerRef != null)
            //            {
            //                alertTitle = "Assign Player Username";
            //                alertBody = "Assigned Current Player to " + plugin.myPlayerRef.playerUsername;
            //                plugin.logger.LogInfo("Matched Player: " + plugin.myPlayerRef.playerUsername);
            //            } else
            //            {
            //                alertTitle = "Error: Assign Player Username";
            //                alertBody = "Invalid Username: " + words[1];
            //                plugin.logger.LogInfo("Invalid Username provided: " + words[1]);
            //            }
            //        }

            //    }
            //    // RoundManager - line 611 has some potentially useful snippits
            //    if (text.ToLower().StartsWith("/teleport"))
            //    {
            //        if (plugin.myPlayerRef != null || plugin.isHost)
            //        {
            //            alertTitle = "Teleport " + (plugin.isHost ? plugin.hostPlayerRef.playerUsername : plugin.myPlayerRef.playerUsername);
            //            string[] words = text.ToLower().Split(' ');

            //            if (words.Length == 2)
            //            {
            //                var players = StartOfRound.Instance.allPlayerScripts.ToList();
            //                var matchedPlayer = players.Find(player => player.playerUsername.ToLower().Contains(words[1].ToLower())) ?? null;
            //                //var playerIndex = players.IndexOf(matchedPlayer);

            //                if (words[1].ToLower().Equals("ship"))
            //                {
            //                    alertBody = "Teleport " + (plugin.isHost ? plugin.hostPlayerRef.playerUsername : plugin.myPlayerRef.playerUsername) + " to Ship";
            //                    plugin.logger.LogInfo("Current player position: " + (plugin.isHost ? plugin.hostPlayerRef.transform.position.ToString() : plugin.myPlayerRef.transform.position.ToString()));
            //                    plugin.logger.LogInfo("Attempting to teleport " + (plugin.isHost ? plugin.hostPlayerRef.playerUsername : plugin.myPlayerRef.playerUsername) + " to position: " + StartOfRound.Instance.playerSpawnPositions[0].transform.position.ToString());
            //                    if (plugin.isHost)
            //                    {
            //                        plugin.hostPlayerRef.TeleportPlayer(StartOfRound.Instance.playerSpawnPositions[0].transform.position, false);
            //                    }
            //                    else plugin.myPlayerRef.TeleportPlayer(StartOfRound.Instance.playerSpawnPositions[0].transform.position, false);
            //                }
            //                if (words[1].ToLower().Equals("inside"))
            //                {
            //                    alertBody = "Teleport " + (plugin.isHost ? plugin.hostPlayerRef.playerUsername : plugin.myPlayerRef.playerUsername) + " to Indoor Entrance";
            //                    plugin.logger.LogInfo("Current player position: " + (plugin.isHost ? plugin.hostPlayerRef.transform.position.ToString() : plugin.myPlayerRef.transform.position.ToString()));
            //                    plugin.logger.LogInfo("Attempting to teleport " + (plugin.isHost ? plugin.hostPlayerRef.playerUsername : plugin.myPlayerRef.playerUsername) + " to position: " + plugin.GetEntrance().ToString());
                                
            //                    if (plugin.isHost)
            //                    {
            //                        plugin.hostPlayerRef.TeleportPlayer(plugin.GetEntrance(), false);
            //                    }
            //                    else plugin.myPlayerRef.TeleportPlayer(plugin.GetEntrance(), false);
            //                }
            //                if (words[1].ToLower().Equals("outside"))
            //                {
            //                    alertBody = "Teleport " + (plugin.isHost ? plugin.hostPlayerRef.playerUsername : plugin.myPlayerRef.playerUsername) + " to Outdoor Entrance";
            //                    plugin.logger.LogInfo("Current player position: " + (plugin.isHost ? plugin.hostPlayerRef.transform.position.ToString() : plugin.myPlayerRef.transform.position.ToString()));
            //                    plugin.logger.LogInfo("Attempting to teleport " + (plugin.isHost ? plugin.hostPlayerRef.playerUsername : plugin.myPlayerRef.playerUsername) + " to position: " + plugin.GetEntrance(true).ToString());

            //                    if (plugin.isHost)
            //                    {
            //                        plugin.hostPlayerRef.TeleportPlayer(plugin.GetEntrance(true), false);
            //                    }
            //                    else plugin.myPlayerRef.TeleportPlayer(plugin.GetEntrance(true), false);

            //                }
            //                if (matchedPlayer != null)
            //                {

            //                    alertBody = "Teleport " + (plugin.isHost ? plugin.hostPlayerRef.playerUsername : plugin.myPlayerRef.playerUsername) + " to " + matchedPlayer.playerUsername;
            //                    plugin.logger.LogInfo("Current player position: " + (plugin.isHost ? plugin.hostPlayerRef.transform.position.ToString() : plugin.myPlayerRef.transform.position.ToString()));
            //                    plugin.logger.LogInfo("Attempting to teleport " + (plugin.isHost ? plugin.hostPlayerRef.playerUsername : plugin.myPlayerRef.playerUsername) + " to " + matchedPlayer.playerUsername + " at position: " + matchedPlayer.transform.position.ToString());

            //                    if (plugin.isHost)
            //                    {
            //                        plugin.hostPlayerRef.TeleportPlayer(matchedPlayer.transform.position, false);
            //                    }
            //                    else plugin.myPlayerRef.TeleportPlayer(matchedPlayer.transform.position, false);
            //                }
            //            }
            //        } else
            //        {
            //            alertTitle = "Error: No Player Assigned";
            //            alertBody = "Use /username yourusernamehere";
            //        }
                    
            //    }
            //    // sends notice to user about what they have done
            //    HUDManager.Instance.DisplayTip(alertTitle, alertBody);
            //    // Hide the message from chat
            //    __instance.chatTextField.text = "";
            //    return;
            //}
        }
        [HarmonyPatch(typeof(RoundManager), "EnemyCannotBeSpawned")]
        [HarmonyPrefix]
        // Ignores if spawning is disabled, enemy power levels, and maxCount
        static bool OverrideEnemySpawn()
        {
            // Should add this behind a toggle... may cause some unintended funky behavior
            plugin.logger.LogInfo("Force Spawned Enemy -> EnemyCannotBeSpawned: false");
            return false;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "SetNightVisionEnabled")]
        [HarmonyPostfix]
        static void enableNightVision(ref PlayerControllerB __instance)
        {
            __instance.nightVision.enabled = plugin.nightVision;
        }

        [HarmonyPatch(typeof(ShotgunItem), "ShootGun")]
        [HarmonyPostfix]
        static void ammoOverride(ref int ___shellsLoaded)
        {
            if (plugin.infiniteAmmo)
                ___shellsLoaded = 2147483647;
        }

        //[HarmonyPatch(typeof(DoorLock), "Update")]
        //[HarmonyPostfix]
        //static void noKeyRequired(ref bool ___isLocked, ref InteractTrigger ___doorTrigger)
        //{
        //    if (___isLocked)
        //    {
        //        if (GameNetworkManager.Instance == null || GameNetworkManager.Instance.localPlayerController == null)
        //        {
        //            return;
        //        }

        //        if (StartOfRound.Instance.localPlayerUsingController)
        //        {
        //            ___doorTrigger.disabledHoverTip = "Use key: [R-trigger]";
        //        }
        //        else
        //        {
        //            ___doorTrigger.disabledHoverTip = "Use key: [ LMB ]";
        //        }

        //        ___doorTrigger.timeToHoldSpeedMultiplier = (float)100.0;

        //    }
        //}

        Vector3 GetEntrance(bool getOutsideEntrance = false)
        {
            EntranceTeleport[] array = FindObjectsOfType<EntranceTeleport>(includeInactive: false);
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].entranceId != 0)
                {
                    continue;
                }

                if (!getOutsideEntrance)
                {
                    if (!array[i].isEntranceToBuilding)
                    {
                        return array[i].entrancePoint.position;

                    }
                }
                else if (array[i].isEntranceToBuilding)
                {
                    return array[i].entrancePoint.position;
                }
            }

            Debug.LogError("Main entrance position could not be found. Returning origin.");
            return Vector3.zero;
        }
        // NoClip originally by Non-Lethal-Company
        void NoClip()
        {
            
            var player = GameNetworkManager.Instance.localPlayerController;

            var camera = player?.gameplayCamera.transform ?? null;

            var collider = player?.GetComponent<CharacterController>() as Collider ?? null;
            if (collider == null)
                return;

            if (noclip)
            {
                collider.enabled = false;
                var dir = new Vector3();

                // Horizontal
                if (UnityInput.Current.GetKey(KeyCode.W))
                    dir += camera.forward;
                if (UnityInput.Current.GetKey(KeyCode.S))
                    dir += camera.forward * -1;
                if (UnityInput.Current.GetKey(KeyCode.D))
                    dir += camera.right;
                if (UnityInput.Current.GetKey(KeyCode.A))
                    dir += camera.right * -1;

                // Vertical
                if (UnityInput.Current.GetKey(KeyCode.Space))
                    dir.y += camera.up.y;
                if (UnityInput.Current.GetKey(KeyCode.LeftControl))
                    dir.y += camera.up.y * -1;

                var prevPos = player.transform.localPosition;
                if (prevPos.Equals(Vector3.zero))
                    return;

                var newPos = prevPos + dir * (noclipSpeed * Time.deltaTime);
                player.transform.localPosition = newPos;
            }
            else
            {
                // if (!_hasDisabledCollider)
                //     return;

                collider.enabled = true;
                // _hasDisabledCollider = false;

            }
        }
        /// <summary>
        /// Register Plugin Commands using Command Pattern (https://refactoring.guru/design-patterns/command)
        /// </summary>
        void RegisterCommands()
        {

        }
    }
}