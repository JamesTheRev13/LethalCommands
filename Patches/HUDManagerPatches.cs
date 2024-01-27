
using BepInEx;
using HarmonyLib;
using LethalCommands.Commands;
using System;
using UnityEngine;

namespace LethalCommands.Patches;

[HarmonyPatch(typeof(HUDManager))]
public class HUDManagerPatches
{
    [HarmonyPatch(nameof(HUDManager.SubmitChat_performed))]
    [HarmonyPrefix]
    static void TextChatCommands(HUDManager __instance)
    {
        string text = __instance.chatTextField.text;

        if (text.StartsWith('/'))
        {
            try
            {
                ICommand command = Plugin.Instance.commandFactory.CreateCommand(text.ToLower());
                if (command != null)
                {
                    Plugin.Instance.logger.LogInfo($"Executing Command: {command?.CommandTitle}");
                    command.SetParameters(text);
                    command.Execute();
                    DisplayCommandLog(command.CommandTitle, command.CommandBody);
                }
            }
            catch (InvalidOperationException ex)
            {
                Plugin.Instance.logger.LogError($"Not Allowed: {text}. Error: {ex.Message}");
                DisplayCommandLog("Not Allowed", ex.Message);
            }
            catch (ArgumentException ex)
            {
                Plugin.Instance.logger.LogError($"Invalid Command: {text}. Error: {ex.Message}");
                DisplayCommandLog("Invalid Command", ex.Message);
            }
            catch (Exception ex)
            {
                Plugin.Instance.logger.LogError($"Unexpected Error: {text}. Error: {ex.Message}");
                DisplayCommandLog("Error", ex.Message);
            }
        }
    }

    [HarmonyPatch(nameof(HUDManager.Update))]
    [HarmonyPrefix]
    static void CommandHistoryEvents(HUDManager __instance)
    {
        var localPlayer = GameNetworkManager.Instance.localPlayerController;
        if (localPlayer.isTypingChat && Plugin.Instance.commandHistory.Count > 0)
        {
            if (UnityInput.Current.GetKeyUp(KeyCode.UpArrow) && Plugin.Instance.currentCommandIndex < Plugin.Instance.commandHistory.Count - 1)
            {
                Plugin.Instance.currentCommandIndex++;
                string commandText = Plugin.Instance.commandHistory[Plugin.Instance.currentCommandIndex];
                __instance.chatTextField.text = commandText;
            }

            if (UnityInput.Current.GetKeyUp(KeyCode.DownArrow) && Plugin.Instance.currentCommandIndex > 0)
            {
                Plugin.Instance.currentCommandIndex--;
                string commandText = Plugin.Instance.commandHistory[Plugin.Instance.currentCommandIndex];
                __instance.chatTextField.text = commandText;
            }
        }
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

    static void DisplayCommandLog(string title, string body)
    {
        HUDManager.Instance.DisplayTip(title, body);
        HUDManager.Instance.chatTextField.text = "";
    }
}

