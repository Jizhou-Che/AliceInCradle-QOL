using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace AutomaticReels;

public enum ReelTriggerMode
{
    Always,
    HoldKey,
    Never,
}

public enum ReelGamepadButton
{
    None,
    LeftTrigger,
    RightTrigger,
    LeftShoulder,
    RightShoulder,
    LeftStick,
    RightStick,
    South,
    East,
    West,
    North,
    Select,
    Start,
}

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    internal static ConfigEntry<ReelTriggerMode> TriggerMode;
    internal static ConfigEntry<KeyCode> TriggerKey;
    internal static ConfigEntry<ReelGamepadButton> TriggerGamepadKey;

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} successfully loaded.");

        TriggerMode = Config.Bind("General", "TriggerMode", ReelTriggerMode.HoldKey,
            "Always optimize the reel selection, optimize only while a trigger key is held, or never optimize.");
        TriggerKey = Config.Bind("General", "TriggerKey", KeyCode.RightShift,
            "The keyboard key held to optimize the reel selection when TriggerMode is HoldKey. Set to None to disable the keyboard trigger.");
        TriggerGamepadKey = Config.Bind("General", "TriggerGamepadKey", ReelGamepadButton.LeftTrigger,
            "The gamepad button held to optimize the reel selection when TriggerMode is HoldKey. LeftTrigger is LT. Reel resolution only responds to a few buttons, so LT does not activate confirm or the menu. Set to None to disable the gamepad trigger.");

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    internal static bool ShouldOptimize()
    {
        return TriggerMode.Value switch
        {
            ReelTriggerMode.Always => true,
            ReelTriggerMode.HoldKey => IsTriggerHeld(TriggerKey.Value) || IsGamepadTriggerHeld(TriggerGamepadKey.Value),
            _ => false,
        };
    }

    private static bool IsTriggerHeld(KeyCode key)
    {
        return key != KeyCode.None && Input.GetKey(key);
    }

    private static bool IsGamepadTriggerHeld(ReelGamepadButton button)
    {
        if (button == ReelGamepadButton.None)
            return false;
        if (TryGetLegacyKey(button, out var key) && Input.GetKey(key))
            return true;

        // LT is an axis, so legacy KeyCode cannot see it. Steam Input also leaves the
        // Deck pad out of Input.GetKey. Read the logical control from connected gamepads.
        foreach (var gamepad in Gamepad.all)
        {
            if (IsPressed(ButtonOn(gamepad, button)))
                return true;
        }

        return false;
    }

    private static bool TryGetLegacyKey(ReelGamepadButton button, out KeyCode key)
    {
        key = button switch
        {
            ReelGamepadButton.South => KeyCode.JoystickButton0,
            ReelGamepadButton.East => KeyCode.JoystickButton1,
            ReelGamepadButton.West => KeyCode.JoystickButton2,
            ReelGamepadButton.North => KeyCode.JoystickButton3,
            ReelGamepadButton.LeftShoulder => KeyCode.JoystickButton4,
            ReelGamepadButton.RightShoulder => KeyCode.JoystickButton5,
            ReelGamepadButton.Select => KeyCode.JoystickButton6,
            ReelGamepadButton.Start => KeyCode.JoystickButton7,
            ReelGamepadButton.LeftStick => KeyCode.JoystickButton8,
            ReelGamepadButton.RightStick => KeyCode.JoystickButton9,
            _ => KeyCode.None,
        };
        return key != KeyCode.None;
    }

    private static ButtonControl ButtonOn(Gamepad gamepad, ReelGamepadButton button)
    {
        return button switch
        {
            ReelGamepadButton.LeftTrigger => gamepad.leftTrigger,
            ReelGamepadButton.RightTrigger => gamepad.rightTrigger,
            ReelGamepadButton.LeftShoulder => gamepad.leftShoulder,
            ReelGamepadButton.RightShoulder => gamepad.rightShoulder,
            ReelGamepadButton.LeftStick => gamepad.leftStickButton,
            ReelGamepadButton.RightStick => gamepad.rightStickButton,
            ReelGamepadButton.South => gamepad.buttonSouth,
            ReelGamepadButton.East => gamepad.buttonEast,
            ReelGamepadButton.West => gamepad.buttonWest,
            ReelGamepadButton.North => gamepad.buttonNorth,
            ReelGamepadButton.Select => gamepad.selectButton,
            ReelGamepadButton.Start => gamepad.startButton,
            _ => null,
        };
    }

    private static bool IsPressed(ButtonControl control)
    {
        return control != null && control.isPressed;
    }
}
