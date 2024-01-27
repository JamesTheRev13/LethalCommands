using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using LethalCommands.Commands;
using LethalCommands.Enumerators;
using UnityEngine;

namespace LethalCommands.Patches;
[HarmonyPatch(typeof(PlayerControllerB))]
public class PlayerControllerBPatches
{
    #region Patches
    [HarmonyPatch(nameof(PlayerControllerB.Update))]
    [HarmonyPrefix]
    static void PlayerUpdate(ref PlayerControllerB __instance)
    {
        if (Plugin.Instance.speedHack)
        {
            __instance.movementSpeed = (float)Plugin.Instance.movementSpeed;
        }
        else __instance.movementSpeed = (float)4.6;
        if (Plugin.Instance.superJump)
        {
            __instance.jumpForce = (float)Plugin.Instance.jumpForce;
        }
        else __instance.jumpForce = (float)13.0;
        if (Plugin.Instance.nightVision && __instance.nightVision)
        {
            __instance.nightVision.enabled = true;
            __instance.nightVision.color = Plugin.Instance.nightVisionColor;
            __instance.nightVision.intensity = Plugin.Instance.nightVisionIntensity;
            __instance.nightVision.range = Plugin.Instance.nightVisionRange;
        }
        else
        {
            __instance.nightVision.enabled = false;
            __instance.nightVision.color = Color.clear;
            __instance.nightVision.intensity = 0f;
            __instance.nightVision.range = 0f;
        }

        if (Plugin.Instance.infiniteSprint)
        {
            Mathf.Clamp(__instance.sprintMeter += 0.02f, 0f, 1f);
        }

        //if (UnityInput.Current.GetKeyUp(KeyCode.KeypadPlus) && __instance.isPlayerDead)
        //{
        //    ICommand command = Plugin.Instance.commandFactory.CreateCommand("/respawn");
        //    command.SetParameters("/respawn");
        //    command.Execute();
        //}

        NoClip();
    }

    // Disallow jump if in noclip
    [HarmonyPatch(nameof(PlayerControllerB.Jump_performed))]
    [HarmonyPrefix]
    static bool NoClipNoJump(ref PlayerControllerB __instance)
    {

        return !Plugin.Instance.noclip;
    }
    // Disallow crouch if in noclip
    [HarmonyPatch(nameof(PlayerControllerB.Crouch_performed))]
    [HarmonyPrefix]
    static bool NoClipNoCrouch(ref PlayerControllerB __instance)
    {

        return !Plugin.Instance.noclip;
    }

    [HarmonyPatch(nameof(PlayerControllerB.AllowPlayerDeath))]
    [HarmonyPrefix]
    static bool OverrideDeath()
    {
        StartOfRound.Instance.allowLocalPlayerDeath = !Plugin.Instance.godMode;
        return !Plugin.Instance.godMode;
    }

    [HarmonyPatch(nameof(PlayerControllerB.SetNightVisionEnabled))]
    [HarmonyPostfix]
    static void enableNightVision(ref PlayerControllerB __instance)
    {
        __instance.nightVision.enabled = Plugin.Instance.nightVision;
    }

    [HarmonyPatch(nameof(PlayerControllerB.Jump_performed))]
    [HarmonyPostfix]
    static void InfiniteJump(ref PlayerControllerB __instance)
    {
        if (Plugin.Instance.infiniteJump && !Plugin.Instance.noclip && !__instance.quickMenuManager.isMenuOpen && ((__instance.IsOwner && __instance.isPlayerControlled && (!__instance.IsServer || __instance.isHostPlayerObject)) || __instance.isTestingPlayer) && !__instance.inSpecialInteractAnimation && !__instance.isTypingChat && (__instance.isMovementHindered <= 0 || __instance.isUnderwater) && (!__instance.isPlayerSliding || __instance.playerSlidingTimer > 2.5f) && !__instance.isCrouching)
        {
            __instance.playerSlidingTimer = 0f;
            __instance.isJumping = true;
            __instance.movementAudio.PlayOneShot(StartOfRound.Instance.playerJumpSFX);

            __instance.jumpCoroutine = __instance.StartCoroutine(PlayerEnumerators.PlayerJump(__instance, Plugin.Instance.jumpForce));
        }
    }
    #endregion

    #region Methods
    // NoClip originally by Non-Lethal-Company
    static void NoClip()
    {
        var player = GameNetworkManager.Instance.localPlayerController;

        var camera = player?.gameplayCamera.transform ?? null;

        var collider = player?.GetComponent<CharacterController>() as Collider ?? null;
        if (collider == null || player.isTypingChat || player.inTerminalMenu || player.quickMenuManager.isMenuOpen)
            return;

        if (Plugin.Instance.noclip)
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

            var newPos = prevPos + dir * (Plugin.Instance.noclipSpeed * Time.deltaTime);
            player.transform.localPosition = newPos;
        }
        else
        {
            collider.enabled = true;

        }
    }
    #endregion
}

