using BepInEx.Logging;
using GameNetcodeStuff;
using System;
using System.Linq;

namespace LethalCommands.Commands.Player;
// Command Pattern - https://refactoring.guru/design-patterns/command
public class RespawnCommand : CommandBase
{
    public RespawnCommand()
    {
        CommandTitle = "Respawn";
    }

    protected override bool ValidateParameters()
    {
        return parameters.Length == 1;
    }

    protected override void ExecuteCommand()
    {
        // Not working as intended- spawns player back, but other players can't see respawned player
        var player = GameNetworkManager.Instance.localPlayerController;
        ManualLogSource.LogInfo($"player: {player.playerUsername}");
        var playerIndex = StartOfRound.Instance.allPlayerScripts.ToList().IndexOf(player);
        ManualLogSource.LogInfo($"playerIndex: {playerIndex}");
        var spectatedPlayer = player.spectatedPlayerScript ?? null;

        if (player.isPlayerDead)
        {
            //player.ResetPlayerBloodObjects();
            //player.isClimbingLadder = false;
            //player.ResetZAndXRotation();
            //player.thisController.enabled = true;
            //player.health = 100;
            //player.disableLookInput = false;
            //player.isPlayerDead = false;
            //player.isPlayerControlled = true;
            //player.isInsideFactory = spectatedPlayer?.isInsideFactory ?? false;
            //player.isInHangarShipRoom = player.isInElevator = spectatedPlayer?.isInHangarShipRoom ?? true;
            //player.wasInElevatorLastFrame = spectatedPlayer?.wasInElevatorLastFrame ?? false;
            ////StartOfRound.Instance.SetPlayerObjectExtrapolate(false);
            //player.localVisor.position = spectatedPlayer?.localVisor?.position ?? StartOfRound.Instance.playerSpawnPositions[0].position;
            //player.thisPlayerModelArms.enabled = true;
            //player.IsInspectingItem = false;
            //player.inTerminalMenu = false;
            //player.setPositionOfDeadPlayer = false;
            //player.snapToServerPosition = true;
            //player.TeleportPlayer(spectatedPlayer?.transform?.position ?? StartOfRound.Instance.GetPlayerSpawnPosition(playerIndex));
            //player.setPositionOfDeadPlayer = false;
            //player.DisablePlayerModel(player.gameObject, true, false);
            //player.helmetLight.enabled = true;
            //player.Crouch(false);
            //player.criticallyInjured = false;
            //if (player.playerBodyAnimator != null)
            //    player.playerBodyAnimator.SetBool("Limp", value: false);

            //player.bleedingHeavily = false;
            //player.activatingItem = false;
            //player.twoHanded = false;
            //player.inSpecialInteractAnimation = false;
            //player.disableSyncInAnimation = false;
            //player.inAnimationWithEnemy = null;
            //player.holdingWalkieTalkie = false;
            //player.speakingToWalkieTalkie = false;

            //player.isSinking = false;
            //player.isUnderwater = false;
            //player.sinkingValue = 0f;
            //player.statusEffectAudio.Stop();
            //player.DisableJetpackControlsLocally();
            //player.health = 100;



            //player.isPlayerDead = false;
            //player.isPlayerControlled = true;
            //player.thisPlayerModelArms.enabled = true;
            //player.localVisor.position = spectatedPlayer?.localVisor?.position ?? StartOfRound.Instance.playerSpawnPositions[0].position;
            //player.DisablePlayerModel(player.gameObject, true, false);
            //player.isInsideFactory = spectatedPlayer?.isInsideFactory ?? false;
            //player.IsInspectingItem = false;
            //player.inTerminalMenu = false;
            //player.setPositionOfDeadPlayer = false;
            //player.snapToServerPosition = true;
            //if (player.IsOwner)
            //{
            //    HUDManager.Instance.gasHelmetAnimator.SetBool("gasEmitting", value: false);
            //    player.hasBegunSpectating = false;
            //    HUDManager.Instance.RemoveSpectateUI();
            //    HUDManager.Instance.gameOverAnimator.SetTrigger("revive");
            //    player.hinderedMultiplier = 1f;
            //    player.isMovementHindered = 0;
            //    player.sourcesCausingSinking = 0;
            //    player.reverbPreset = StartOfRound.Instance.shipReverb;
            //}
            //player.bleedingHeavily = false;
            //player.criticallyInjured = false;
            //player.playerBodyAnimator.SetBool("Limp", value: false);
            //player.health = 100;
            //HUDManager.Instance.UpdateHealthUI(100, hurtPlayer: false);
            //player.spectatedPlayerScript = null;

            //SoundManager.Instance.earsRingingTimer = 0f;
            //player.voiceMuffledByEnemy = false;
            //SoundManager.Instance.playerVoicePitchTargets[playerIndex] = 1f;
            //SoundManager.Instance.SetPlayerPitch(1f, playerIndex);
            //if (player.currentVoiceChatIngameSettings == null)
            //{
            //    StartOfRound.Instance.RefreshPlayerVoicePlaybackObjects();
            //}
            //HUDManager.Instance.audioListenerLowPass.enabled = false;
            //if (player.currentVoiceChatIngameSettings != null)
            //{
            //    if (player.currentVoiceChatIngameSettings.voiceAudio == null)
            //    {
            //        player.currentVoiceChatIngameSettings.InitializeComponents();
            //    }

            //    if (player.currentVoiceChatIngameSettings.voiceAudio == null)
            //    {
            //        return;
            //    }

            //    player.currentVoiceChatIngameSettings.voiceAudio.GetComponent<OccludeAudio>().overridingLowPass = false;
            //}
            //StartOfRound.Instance.SetSpectateCameraToGameOverMode(enableGameOver: false, player);
            //StartOfRound.Instance.UpdatePlayerVoiceEffects();

            //HUDManager.Instance.HideHUD(hide: false);
            //StartOfRound.Instance.livingPlayers++;
            //StartOfRound.Instance.allPlayersDead = false;

            StartOfRound.Instance.allPlayersDead = false;
            player.ResetZAndXRotation();
            player.isPlayerDead = false;
            player.isPlayerControlled = true;
            player.thisPlayerModelArms.enabled = true;
            player.localVisor.position = spectatedPlayer?.localVisor?.position ?? StartOfRound.Instance.playerSpawnPositions[playerIndex].transform.position;
            player.TeleportPlayer(spectatedPlayer?.transform?.position ?? StartOfRound.Instance.playerSpawnPositions[playerIndex].position);
            player.setPositionOfDeadPlayer = false;
            player.DisablePlayerModel(player.gameObject, true, false);
            player.helmetLight.enabled = false;
            player.criticallyInjured = false;
            if (player.playerBodyAnimator != null)
            {
                player.playerBodyAnimator.SetBool("Limp", value: false);
            }
            player.bleedingHeavily = false;
            player.activatingItem = false;
            player.twoHanded = false;
            player.inSpecialInteractAnimation = false;
            player.disableSyncInAnimation = false;

            player.isInsideFactory = spectatedPlayer?.isInsideFactory ?? false;
            player.IsInspectingItem = false;
            player.inTerminalMenu = false;
            player.setPositionOfDeadPlayer = false;
            player.snapToServerPosition = true;
            player.mapRadarDotAnimator.SetBool("dead", value: false);
            player.hasBegunSpectating = false;
            player.hinderedMultiplier = 1f;
            player.isMovementHindered = 0;
            player.sourcesCausingSinking = 0;
            player.reverbPreset = StartOfRound.Instance.shipReverb;
            HUDManager.Instance.RemoveSpectateUI();

            if (player.currentVoiceChatIngameSettings == null)
                StartOfRound.Instance.RefreshPlayerVoicePlaybackObjects();

            if (player.currentVoiceChatIngameSettings != null)
            {
                if (player.currentVoiceChatIngameSettings.voiceAudio == null)
                {
                    player.currentVoiceChatIngameSettings.InitializeComponents();
                }

                if (player.currentVoiceChatIngameSettings.voiceAudio == null)
                {
                    return;
                }

                player.currentVoiceChatIngameSettings.voiceAudio.GetComponent<OccludeAudio>().overridingLowPass = false;
            }
            HUDManager.Instance.audioListenerLowPass.enabled = false;
            StartOfRound.Instance.SetSpectateCameraToGameOverMode(enableGameOver: false, player);
            HUDManager.Instance.HideHUD(hide: false);
            StartOfRound.Instance.livingPlayers++;
            StartOfRound.Instance.allPlayersDead = false;

            CommandBody = $"Respawned {player.playerUsername}";
            return;
        }
        CommandBody = $"{player.playerUsername} is not dead.";
        ManualLogSource.LogInfo($"Failed to respawn {player.playerUsername} - player not dead");
    }
}

