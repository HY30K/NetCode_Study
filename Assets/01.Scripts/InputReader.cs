using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(fileName = "New Input Reader", menuName = "SO/Input/InputReader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    public event Action<bool> PrimaryFireEvent;
    public event Action<Vector2> MovementEvent;
    public Vector2 AimPosition { get; private set; }

    private Controls _controlAction;

    private void OnEnable()
    {
        if (_controlAction == null)
        {
            _controlAction = new Controls();
            _controlAction.Player.SetCallbacks(this); //�÷��̾� ��ǲ�� �߻��ϸ� �� �ν��Ͻ��� �������ְ�
        }

        _controlAction.Player.Enable(); //�÷��̾� �Է� Ȱ��ȭ
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        MovementEvent?.Invoke(value);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PrimaryFireEvent?.Invoke(true);
        }
        else if (context.canceled)
        {
            PrimaryFireEvent?.Invoke(false);
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        AimPosition = context.ReadValue<Vector2>();
    }
}
