using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    [SerializeField]
    private PlayerBehaviour player;

    private void OnJump(InputValue value)
    {
        Debug.Log($"jump to {value.Get<float>()}");
        player.OnJumpChanged(value.Get<float>() != 0);
    }

    private void OnPrimaryAction()
    {
        Debug.Log("primary action");
    }

    private void OnMovement(InputValue value)
    {
        Debug.Log($"movement {value.Get<Vector2>()}");
        player.OnMoveChanged(value.Get<Vector2>());
    }

    private void OnDash()
    {
        player.OnDashChanged();
    }
}
