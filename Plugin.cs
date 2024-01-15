using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using LethalCommands.Commands;
using UnityEngine;

/* PLUGIN BY BOB SAGET -  INSPIRED BY GAMEMASTER, DANCETOOLS, and NON-LETHAL-COMPANY - VERY EARLY WORK IN PROGRESS */
namespace LethalCommands
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public ManualLogSource logger;
        public static Plugin plugin;
        // Command Pattern - https://refactoring.guru/design-patterns/command
        public CommandFactory commandFactory;

        #region Command Fields
        public bool noclip = false;
        public bool godMode = false;
        //public bool demiGod = false;
        //public bool invisibility = false;
        public bool infiniteAmmo = false;
        public bool nightVision = false;
        public bool speedHack = false;
        public bool infiniteSprint = false;
        public bool infiniteJump = false;
        public bool infiniteCredits = false;
        public bool infiniteDeadline = false;
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
            // Plugin startup logic
            logger = Logger;
            plugin = this;
            // Create an instance of CommandFactory to help us churn out some Commands!
            commandFactory = new CommandFactory(plugin, logger);
            logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
        [HarmonyPrefix]
        static void TextChatCommands(HUDManager __instance)
        {
            // TODO: add a command history, and allow UP/DOWN key navigation of command history
            string text = __instance.chatTextField.text;
            ICommand command = plugin.commandFactory.CreateCommand(text);
            if (command != null && text.StartsWith('/'))
            {
                command.SetParameters(text);
                command.Execute();
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Update))]
        [HarmonyPrefix]
        // I'd refactor this - this runs way too frequently (every frame?), and is really easy to cause performance/stability issues here
        // Try to move individial command/toggle logic to patch logic? IDK man this just doesn't sit right
        static void toggleCheck(ref PlayerControllerB __instance)
        {
            // Move to switch statement for performance...?
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
            //if (!GameNetworkManager.Instance.localPlayerController.IsHost) { return; }
            if (plugin.infiniteCredits) { ___groupCredits = 69420; }
        }

        [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.MoveGlobalTime))]
        [HarmonyPostfix]
        static void InfiniteDeadline(ref float ___timeUntilDeadline)
        {
            //if (!GameNetworkManager.Instance.localPlayerController.IsHost) { return; }
            if (plugin.infiniteDeadline) { ___timeUntilDeadline = 5000; }

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

        public Vector3 GetEntrance(bool getOutsideEntrance = false)
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
            if (collider == null || player.isTypingChat)
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
                collider.enabled = true;

            }
        }
    }
}