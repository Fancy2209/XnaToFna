﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoMod.InlineRT;
using SDL2;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using XnaToFna.ProxyForms;

namespace XnaToFna {
    public static class KeyboardEvents {

        public static HashSet<Keys> LastDown = new HashSet<Keys>();
        public static HashSet<Keys> Down = new HashSet<Keys>();

        public static void KeyDown(Keys key)
            // From what I can tell, lParam is being used to differentiate between left and right ctrl, alt and shift. 
            => PInvokeHelper.CallHooks(Messages.WM_KEYDOWN, (IntPtr) key, IntPtr.Zero);

        public static void KeyUp(Keys key)
            => PInvokeHelper.CallHooks(Messages.WM_KEYUP, (IntPtr) key, IntPtr.Zero);

        public static void CharEntered(char c)
            => PInvokeHelper.CallHooks(Messages.WM_CHAR, (IntPtr) c, IntPtr.Zero);

        // Unclear how / where this should be invoked
        public static void SetContext(bool wParam)
            => PInvokeHelper.CallHooks(Messages.WM_IME_SETCONTEXT, (IntPtr) (wParam ? 1 : 0), IntPtr.Zero);

        public static void Update() {
            Keys[] keys = Keyboard.GetState().GetPressedKeys();
            Down.Clear();
            for (int i = 0; i < keys.Length; i++) {
                Keys key = keys[i];
                if (!LastDown.Contains(key))
                    KeyDown(key);
                Down.Add(key);
            }
            foreach (Keys key in LastDown) {
                if (!Down.Contains(key))
                    KeyUp(key);
            }
            LastDown.Clear();
            LastDown.UnionWith(Down);
        }

    }

    public static partial class PInvokeHooks {

        public static IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC) {
            // Not required for FNA
            return IntPtr.Zero;
        }

        public static IntPtr ImmGetContext(IntPtr hWnd) {
            // Not required for FNA
            return IntPtr.Zero;
        }

        public static bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC) {
            return true;
        }

        public static short GetAsyncKeyState(int vKey) {
            return (short) (Keyboard.GetState().IsKeyDown((Keys) vKey) ? 0x80 : 0x00);
        }

    }
}