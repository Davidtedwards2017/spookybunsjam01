using InControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InControlManager))]
public class InputController : Singleton<InputController>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static InputDevice GetInputDevice(int deviceIndex)
    {
        if (InputManager.Devices.Count <= deviceIndex) return null;
        return InputManager.Devices[deviceIndex];
    }
          
    public bool GetJumpInput()
    {
        return GetJumpInput(0) || Input.GetKeyDown(KeyCode.UpArrow);
    }

    public bool GetJumpInput(int deviceIndex)
    {
        return GetJumpInput(GetInputDevice(deviceIndex));
    }

    public bool GetJumpInput(InputDevice device)
    {
        if(device == null)
        {
            return false;
        }

        return device.Action1;
    }


    public float GetInputX()
    {
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            return -1;
        }
        else if(Input.GetKey(KeyCode.RightArrow))
        {
            return 1;
        }

        return GetInputX(0);
    }

    public float GetInputX(int deviceIndex)
    {
        return GetInputX(GetInputDevice(deviceIndex));
    }

    public float GetInputX(InputDevice device)
    {
        if (device == null)
        {
            return 0;
        }

        return Mathf.Clamp(device.LeftStick.X /*+ device.RightStick.X*/, -1, 1);
    }

}
