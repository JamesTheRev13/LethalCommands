using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using LethalCommands.Commands;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/* PLUGIN BY BOB SAGET -  INSPIRED BY GAMEMASTER, DANCETOOLS, and NON-LETHAL-COMPANY - VERY EARLY WORK IN PROGRESS */
namespace LethalCommands;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public ManualLogSource logger;
    public static Plugin plugin;
    // Command Pattern - https://refactoring.guru/design-patterns/command
    public CommandFactory commandFactory;

    #region Command Fields
    public List<ICommand> commandHistory = new();
    public int currentCommandIndex = -1;
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
    public bool infiniteBattery = false;
    public bool superJump = false;
    public float jumpForce = (float)13.0;
    public float movementSpeed = (float)4.6;
    public float noclipSpeed = 10.0f;
    public float nightVisionIntensity = 1000f;
    public float nightVisionRange = 10000f;
    public Color nightVisionColor = Color.green;
    #endregion
    private void Awake()
    {
        logger = Logger;
        plugin = this;
        commandFactory = new CommandFactory(plugin, logger);

        logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(Plugin));
    }
    /* 
     * TODO: Break out patches, this file is too long
     */
    [HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
    [HarmonyPrefix]
    static void TextChatCommands(HUDManager __instance)
    {
        // TODO: add a command history, and allow UP/DOWN key navigation of command history
        string text = __instance.chatTextField.text;
        if (text.StartsWith('/'))
        {
            ICommand command = plugin.commandFactory.CreateCommand(text.ToLower());
            if (command != null)
            {
                command.SetParameters(text);
                command.Execute();
                plugin.logger.LogInfo($"Executed Command: {text}");
                plugin.currentCommandIndex = -1;
                plugin.commandHistory.Insert(0, command);
                plugin.logger.LogInfo($"Added Command to Command History: {text}");
            }
        }
    }
    [HarmonyPatch(typeof(HUDManager), "Update")]
    [HarmonyPrefix]
    static void CommandHistoryEvents(HUDManager __instance)
    {
        var localPlayer = GameNetworkManager.Instance.localPlayerController;
        if (localPlayer.isTypingChat && plugin.commandHistory.Count > 0)
        {
            if (UnityInput.Current.GetKeyUp(KeyCode.UpArrow) && plugin.currentCommandIndex < plugin.commandHistory.Count - 1)
            {
                plugin.currentCommandIndex++;
                var commandText = plugin.commandHistory[plugin.currentCommandIndex].GetCommand();
                __instance.chatTextField.text = commandText;
            }

            if (UnityInput.Current.GetKeyUp(KeyCode.DownArrow) && plugin.currentCommandIndex > 0)
            {
                plugin.currentCommandIndex--;
                var commandText = plugin.commandHistory[plugin.currentCommandIndex].GetCommand();
                __instance.chatTextField.text = commandText;
            }
        }
        
    }
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Update))]
    [HarmonyPrefix]
    static void ToggleCheck(ref PlayerControllerB __instance)
    {
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

        if (UnityInput.Current.GetKeyUp(KeyCode.KeypadPlus) && __instance.isPlayerDead)
        {
            ICommand command = plugin.commandFactory.CreateCommand("/respawn");
            command.SetParameters("/respawn");
            command.Execute();
        }

        plugin.NoClip();
    }

    [HarmonyPatch(typeof(PlayerControllerB), "Jump_performed")]
    [HarmonyPostfix]
    static void InfiniteJump(ref PlayerControllerB __instance)
    {

        if (plugin.infiniteJump && !plugin.noclip && !__instance.quickMenuManager.isMenuOpen && ((__instance.IsOwner && __instance.isPlayerControlled && (!__instance.IsServer || __instance.isHostPlayerObject)) || __instance.isTestingPlayer) && !__instance.inSpecialInteractAnimation && !__instance.isTypingChat && (__instance.isMovementHindered <= 0 || __instance.isUnderwater) && (!__instance.isPlayerSliding || __instance.playerSlidingTimer > 2.5f) && !__instance.isCrouching)
        {
            __instance.playerSlidingTimer = 0f;
            __instance.isJumping = true;
            __instance.movementAudio.PlayOneShot(StartOfRound.Instance.playerJumpSFX);

            __instance.jumpCoroutine = __instance.StartCoroutine(plugin.PlayerJump(__instance));
        }
    }
    // Disallow jump if in noclip
    [HarmonyPatch(typeof(PlayerControllerB), "Jump_performed")]
    [HarmonyPrefix]
    static bool NoClipNoJump(ref PlayerControllerB __instance)
    {

        return !plugin.noclip;
    }
    // Disallow crouch if in noclip
    [HarmonyPatch(typeof(PlayerControllerB), "Crouch_performed")]
    [HarmonyPrefix]
    static bool NoClipNoCrouch(ref PlayerControllerB __instance)
    {

        return !plugin.noclip;
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
        if (plugin.infiniteCredits) { ___groupCredits = 69420; }
    }

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.MoveGlobalTime))]
    [HarmonyPostfix]
    static void InfiniteDeadline(ref float ___timeUntilDeadline)
    {
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


    [HarmonyPatch(typeof(GrabbableObject), "Update")]
    [HarmonyPrefix]
    static void batteryOverride(ref Battery ___insertedBattery)
    {
        if (plugin.infiniteBattery)
        {
            ___insertedBattery.charge = 100.0f;
            ___insertedBattery.empty = false;
        }       
    }

    [HarmonyPatch(typeof(QuickMenuManager), "Debug_SpawnItem")]
    [HarmonyPrefix]
    static bool DebugSpawnItemOverride(ref int ___itemToSpawnId, ref Transform[] ___debugEnemySpawnPositions)
    {
        plugin.Debug_SpawnItem(___itemToSpawnId, ___debugEnemySpawnPositions);

        return false;
    }

    [HarmonyPatch(typeof(QuickMenuManager), "Start")]
    [HarmonyPrefix]
    static void EnableDebugMenu()
    {
        var menuManager = FindObjectOfType<QuickMenuManager>();
        menuManager.Debug_SetEnemyDropdownOptions();
        menuManager.Debug_SetAllItemsDropdownOptions();
    }

    [HarmonyPatch(typeof(QuickMenuManager), "Debug_ToggleTestRoom")]
    [HarmonyPrefix]
    static bool DebugTestRoomOverride()
    {
        StartOfRound.Instance.Debug_EnableTestRoomServerRpc(StartOfRound.Instance.testRoom == null);
        plugin.logger.LogInfo($"[DEBUG MENU] Test Room: {StartOfRound.Instance.testRoom == null}");

        return false;
    }

    [HarmonyPatch(typeof(QuickMenuManager), "Debug_ToggleAllowDeath")]
    [HarmonyPrefix]
    static bool DebugAllowDeathOverride()
    {
        StartOfRound.Instance.Debug_ToggleAllowDeathServerRpc();
        plugin.logger.LogInfo($"[DEBUG MENU] God Mode: {StartOfRound.Instance.allowLocalPlayerDeath}");
        return false;
    }

    [HarmonyPatch(typeof(QuickMenuManager), "Debug_SpawnEnemy")]
    [HarmonyPrefix]
    static bool DebugSpawnEnemyOverride(ref int ___enemyTypeId, ref SelectableLevel ___testAllEnemiesLevel, ref int ___enemyToSpawnId, ref int ___numberEnemyToSpawn, ref Transform[] ___debugEnemySpawnPositions)
    {
        plugin.Debug_SpawnEnemy(___enemyTypeId, ___testAllEnemiesLevel, ___enemyToSpawnId, ___numberEnemyToSpawn, ___debugEnemySpawnPositions);

        return false;
    }

    [HarmonyPatch(typeof(QuickMenuManager), "CanEnableDebugMenu")]
    [HarmonyPostfix]
    static bool DebugCanEnableDebugMenuOverride(bool canEnable, ref int ___enemyTypeId, ref SelectableLevel ___testAllEnemiesLevel, ref int ___enemyToSpawnId, ref int ___numberEnemyToSpawn, ref Transform[] ___debugEnemySpawnPositions)
    {
        return NetworkManager.Singleton.IsServer;
    }
    //[HarmonyPatch(typeof(HUDManager), "EnableChat_performed")]
    //[HarmonyPostfix]
    //static void EnableChatOnDeath(CallbackContext context)
    //{
    //    var localPlayer = GameNetworkManager.Instance.localPlayerController;
    //    if (((CallbackContext)(ref context)).performed && !(localPlayer == null) && ((localPlayer.IsOwner && (!localPlayer.IsServer || localPlayer.isHostPlayerObject)) || localPlayer.isTestingPlayer) && !localPlayer.inTerminalMenu)
    //    {
    //        ShipBuildModeManager.Instance.CancelBuildMode();
    //        localPlayer.isTypingChat = true;
    //        ((Selectable)chatTextField).Select();
    //        PingHUDElement(Chat, 0.1f, 1f, 1f);
    //        ((Behaviour)(object)typingIndicator).enabled = true;
    //    }
    //}

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
        if (collider == null || player.isTypingChat || player.inTerminalMenu || player.quickMenuManager.isMenuOpen)
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

    public void Debug_SpawnItem(int itemToSpawnId, Transform[] debugEnemySpawnPositions)
    {
        if (NetworkManager.Singleton.IsConnectedClient && NetworkManager.Singleton.IsServer)
        {
            GameObject obj = Instantiate(StartOfRound.Instance.allItemsList.itemsList[itemToSpawnId].spawnPrefab, GameNetworkManager.Instance.localPlayerController.transform.position, Quaternion.identity, StartOfRound.Instance.propsContainer);
            obj.GetComponent<GrabbableObject>().fallTime = 0f;
            obj.GetComponent<NetworkObject>().Spawn();
            logger.LogInfo($"[DEBUG MENU] Spawned {StartOfRound.Instance.allItemsList.itemsList[itemToSpawnId].itemName} at {GameNetworkManager.Instance.localPlayerController.transform.position}");
        }
    }

    public void Debug_SpawnEnemy(int enemyTypeId, SelectableLevel testAllEnemiesLevel, int enemyToSpawnId, int numberEnemyToSpawn, Transform[] debugEnemySpawnPositions)
    {
        if (!NetworkManager.Singleton.IsConnectedClient || !NetworkManager.Singleton.IsServer)
        {
            return;
        }

        EnemyType enemyType = null;
        Vector3 spawnPosition = Vector3.zero;
        switch (enemyTypeId)
        {
            case 0:
                enemyType = testAllEnemiesLevel.Enemies[enemyToSpawnId].enemyType;
                spawnPosition = ((!(StartOfRound.Instance.testRoom != null)) ? RoundManager.Instance.insideAINodes[UnityEngine.Random.Range(0, RoundManager.Instance.insideAINodes.Length)].transform.position : debugEnemySpawnPositions[enemyTypeId].position);
                break;
            case 1:
                enemyType = testAllEnemiesLevel.OutsideEnemies[enemyToSpawnId].enemyType;
                spawnPosition = ((!(StartOfRound.Instance.testRoom != null)) ? RoundManager.Instance.outsideAINodes[UnityEngine.Random.Range(0, RoundManager.Instance.outsideAINodes.Length)].transform.position : debugEnemySpawnPositions[enemyTypeId].position);
                break;
            case 2:
                enemyType = testAllEnemiesLevel.DaytimeEnemies[enemyToSpawnId].enemyType;
                spawnPosition = ((!(StartOfRound.Instance.testRoom != null)) ? RoundManager.Instance.outsideAINodes[UnityEngine.Random.Range(0, RoundManager.Instance.outsideAINodes.Length)].transform.position : debugEnemySpawnPositions[enemyTypeId].position);
                break;
        }

        if (!(enemyType == null))
        {
            for (int i = 0; i < numberEnemyToSpawn && i <= 50; i++)
            {
                RoundManager.Instance.SpawnEnemyGameObject(spawnPosition, 0f, -1, enemyType);
                logger.LogInfo($"[DEBUG MENU] Spawned {enemyType.enemyName} at {spawnPosition}");
            }
        }
    }

    public IEnumerator PlayerJump(PlayerControllerB player)
    {
        player.playerBodyAnimator.SetBool("Jumping", value: true);
        yield return new WaitForSeconds(0.15f);
        player.fallValue = jumpForce;
        player.fallValueUncapped = jumpForce;
        yield return new WaitForSeconds(0.1f);
        player.isJumping = false;
        player.isFallingFromJump = true;
        yield return new WaitUntil(() => player.thisController.isGrounded);
        player.playerBodyAnimator.SetBool("Jumping", value: false);
        player.isFallingFromJump = false;
        //player.PlayerHitGroundEffects();
        player.jumpCoroutine = null;
    }
}