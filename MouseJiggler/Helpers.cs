#region header

// MouseJiggler - Helpers.cs
// 
// Created by: Alistair J R Young (avatar) at 2021/01/20 7:40 PM.
// Updates by: Dimitris Panokostas (midwan)
// Updates by: Dennis

#endregion

#region using

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

#endregion

namespace ArkaneSystems.MouseJiggler;

internal static class Helpers
{
    #region Console management

    /// <summary>
    ///     Constant value signifying a request to attach to the console of the parent process.
    /// </summary>
    internal const uint AttachParentProcess = uint.MaxValue;

    #endregion Console management


    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [StructLayout(LayoutKind.Sequential)]
    internal struct INPUT
    {
        public uint type;
        public MOUSEINPUT mi;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    internal static class INPUT_TYPE
    {
        public const uint INPUT_MOUSE = 0;
    }

    internal static class MOUSE_EVENT_FLAGS
    {
        public const uint MOUSEEVENTF_MOVE = 0x0001;
    }

    internal static void Jiggle(int delta)
    {
        var input = new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
            mi = new MOUSEINPUT
            {
                dx = delta,
                dy = delta,
                mouseData = 0,
                dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE,
                time = 0,
                dwExtraInfo = IntPtr.Zero
            }
        };

        var inputs = new INPUT[] { input };
        var result = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<INPUT>());

        if (result == 0)
        {
            var errorCode = Marshal.GetLastWin32Error();
            Debugger.Log(1, "Jiggle", $"SendInput failed: error code 0x{errorCode:X8}\n");
        }
    }
}