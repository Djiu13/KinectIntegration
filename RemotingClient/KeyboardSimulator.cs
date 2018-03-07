using System;
using System.Runtime.InteropServices;
using WindowsInput;

namespace Input
{


    public static class KeyboardSimulator
    {
        public const int KEYEVENTF_KEYDOWN = 0x0001;
        public const int KEYEVENTF_KEYUP = 0x0002;
        public const int KEYEVENTF_UNICODE = 0x0008;
        public const int INPUT_KEYBOARD = 1;


        public static short myGetScancode(ushort key)
        {
            switch (key)
            {
                case (ushort)VirtualKeyCode.UP: return 0x48;
                case (ushort)VirtualKeyCode.DOWN: return 0x50;
                case (ushort)VirtualKeyCode.LEFT: return 0x4b;
                case (ushort)VirtualKeyCode.RIGHT: return 0x4d;
                case (ushort)VirtualKeyCode.TAB: return 0x0f;
                case (ushort)VirtualKeyCode.LCONTROL: return 0x1d;
                case (ushort)VirtualKeyCode.F5: return 0x3f;
                case (ushort)VirtualKeyCode.ESCAPE: return 0x01;
                case (ushort)VirtualKeyCode.VK_0: return 0x0B; //0
                case (ushort)VirtualKeyCode.VK_1: return 0x02; //1
                case (ushort)VirtualKeyCode.VK_2: return 0x03; //2
                case (ushort)VirtualKeyCode.VK_3: return 0x04; //3
                case (ushort)VirtualKeyCode.VK_4: return 0x05; //4
                case (ushort)VirtualKeyCode.VK_5: return 0x06; //5
                case (ushort)VirtualKeyCode.VK_6: return 0x07; //6
                case (ushort)VirtualKeyCode.VK_7: return 0x08; //7
                case (ushort)VirtualKeyCode.VK_8: return 0x09; //8
                case (ushort)VirtualKeyCode.VK_9: return 0x0A; //9
                case (ushort)VirtualKeyCode.VK_Q: return 0x1e; //A
                case (ushort)VirtualKeyCode.VK_B: return 0x30; //B
                case (ushort)VirtualKeyCode.VK_C: return 0x2E; //C
                case (ushort)VirtualKeyCode.VK_D: return 0x20; //D
                case (ushort)VirtualKeyCode.VK_E: return 0x12; //E
                case (ushort)VirtualKeyCode.VK_F: return 0x21; //F
                case (ushort)VirtualKeyCode.VK_G: return 0x22; //G
                case (ushort)VirtualKeyCode.VK_H: return 0x23; //H
                case (ushort)VirtualKeyCode.VK_I: return 0x17; //I
                case (ushort)VirtualKeyCode.VK_J: return 0x24; //J
                case (ushort)VirtualKeyCode.VK_K: return 0x25; //K
                case (ushort)VirtualKeyCode.VK_L: return 0x26; //L
                case (ushort)VirtualKeyCode.VK_M: return 0x27; //M
                case (ushort)VirtualKeyCode.VK_N: return 0x31; //N
                case (ushort)VirtualKeyCode.VK_O: return 0x18; //O
                case (ushort)VirtualKeyCode.VK_P: return 0x19; //P
                case (ushort)VirtualKeyCode.VK_A: return 0x10; //Q
                case (ushort)VirtualKeyCode.VK_R: return 0x13; //R
                case (ushort)VirtualKeyCode.VK_S: return 0x1F; //S
                case (ushort)VirtualKeyCode.VK_T: return 0x14; //T
                case (ushort)VirtualKeyCode.VK_U: return 0x16; //U
                case (ushort)VirtualKeyCode.VK_V: return 0x2F; //V
                case (ushort)VirtualKeyCode.VK_Z: return 0x11; //W
                case (ushort)VirtualKeyCode.VK_X: return 0x2D; //X
                case (ushort)VirtualKeyCode.VK_Y: return 0x15; //Y
                case (ushort)VirtualKeyCode.VK_W: return 0x2C; //Z
                default: return 0;
            }
        }

        /// <summary>
        ///     Envoie un signal d'un peripherique d'entree
        /// </summary>
        /// <param name="nInputs"> Nombre d'entrées</param>
        /// <param name="pInputs"> Liste des commandes</param>
        /// <param name="cbSize"> Taille de la structure (28 par defaut)</param>
        /// <returns> Code d'eerreur </returns>
        [DllImport("user32.dll")]
        public static extern UInt32 SendInput(UInt32 nInputs, ref Input pInputs, int cbSize);

        //Structure des peripheriques d'entree
        [StructLayout(LayoutKind.Explicit)]
        public struct Input
        {
            [FieldOffset(0)]
            public Int32 type;
            [FieldOffset(4)]
            public MouseInput mi;
            [FieldOffset(4)]
            public tagKEYBDINPUT ki;
            [FieldOffset(4)]
            public tagHARDWAREINPUT hi;
        }
        /// <summary>
        ///  Represente une commande clavier
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct tagKEYBDINPUT
        {
            public Int16 wVk;
            public Int16 wScan;
            public Int32 dwFlags;
            public Int32 time;
            public IntPtr dwExtraInfo;
        }

        //--------------------------------- Inutilisés ------------------------------//
        [StructLayout(LayoutKind.Sequential)]
        public struct MouseInput
        {
            public Int32 dx;
            public Int32 dy;
            public Int32 Mousedata;
            public Int32 dwFlag;
            public Int32 time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct tagHARDWAREINPUT
        {
            public Int32 uMsg;
            public Int16 wParamL;
            public Int16 wParamH;
        }


    }

}