using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCtrl
{

    public PlayerInput inputCtrl;

    public enum KeyState { None, Press, Hold, Release }

    public Vector2 input;

    public KeyState keyJump;
    private bool pressedJump = false;
    public KeyState keyDash;
    private bool pressedDash = false;

    public virtual void OnEnable()
    {
        inputCtrl = new PlayerInput();
        inputCtrl.Player.Enable();

        inputCtrl.Player.Movement.performed += ctx =>
        {
            input = ctx.ReadValue<Vector2>();
        };
        inputCtrl.Player.Movement.canceled += ctx =>
        {
            input = Vector2.zero;
        };

        inputCtrl.Player.Jump.performed += ctx => { pressedJump = true; };
        inputCtrl.Player.Jump.canceled += ctx => { pressedJump = false; };

        inputCtrl.Player.Dash.performed += ctx => { pressedDash = true; };
        inputCtrl.Player.Dash.canceled += ctx => { pressedDash = false; };
    }
    public virtual void OnDisable() { }

    public void UpdateInputStates()
    {
        keyJump = UpdateKeyState(keyJump, pressedJump);
        keyDash = UpdateKeyState(keyDash, pressedDash);
    }
    public bool IsHeld(KeyState state)
    {
        return state == KeyState.Press || state == KeyState.Hold;
    }

    private KeyState UpdateKeyState(KeyState curState, bool pressed)
    {
        if (pressed)
        {
            if (curState == KeyState.None)
                curState = KeyState.Press;
            else if (curState == KeyState.Press)
                curState = KeyState.Hold;
            else if (curState == KeyState.Release)
                curState = KeyState.Press;
            else if (curState == KeyState.Hold)
                curState = KeyState.Hold;
        }
        else
        {
            if (curState == KeyState.Hold)
                curState = KeyState.Release;
            else if (curState == KeyState.Release)
                curState = KeyState.None;
            else if (curState == KeyState.Press)
                curState = KeyState.Release;
            else if (curState == KeyState.None)
                curState = KeyState.None;
        }

        return curState;
    }

}
