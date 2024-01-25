using System.Collections;
using GameNetcodeStuff;
using UnityEngine;

namespace LethalCommands.Enumerators;
public class PlayerEnumerators
{
    public static IEnumerator PlayerJump(PlayerControllerB player, float jumpForce)
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
