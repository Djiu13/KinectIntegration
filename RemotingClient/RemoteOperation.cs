using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Windows.Forms;
using Input;
using RemotingClient.Formes;
using RemotingInterfaces;
using WindowsInput;

namespace RemotingClient
{
    public class RemoteOperation : MarshalByRefObject, IRemoteOperation
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, uint dwExtraInf);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(ref Point lpPoint);

        #region mouse
        private const UInt32 MOUSEEVENTF_MOVE = 0x0001;
        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;
        private const UInt32 MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const UInt32 MOUSEEVENTF_RIGHTUP = 0x0010;
        private const UInt32 MOUSEEVENTF_ABSOLUTE = 0x8000;
        #endregion
        public static MainForm mainForm = null;

        #region movecursor
        private static uint x = 900;
        #endregion

        // Indique que l'objet aura une durée de vie illimitée
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void message(string mess)
        {
            mainForm.WriteLine(mess);
        }

        public void moveCursorToPosition(float p, float p_2)
        {
            Cursor.Position = new Point(Convert.ToInt32(p), Convert.ToInt32(p_2));
        }

        public void press_key_down(VirtualKeyCode key)
        {
            KeyboardSimulator.Input inDown = new KeyboardSimulator.Input();
            inDown.type = KeyboardSimulator.INPUT_KEYBOARD;
            inDown.ki.wVk = (Int16)key;
            inDown.ki.wScan = KeyboardSimulator.myGetScancode((ushort)key);
            if (key == VirtualKeyCode.LEFT || key == VirtualKeyCode.DOWN || key == VirtualKeyCode.RIGHT
                || key == VirtualKeyCode.UP)
                inDown.ki.dwFlags = KeyboardSimulator.KEYEVENTF_KEYDOWN;
            else
                inDown.ki.dwFlags = KeyboardSimulator.KEYEVENTF_UNICODE;
            KeyboardSimulator.SendInput(1, ref inDown, Marshal.SizeOf(inDown));
        }

        public void press_key_up(VirtualKeyCode key)
        {
            KeyboardSimulator.Input inUp = new KeyboardSimulator.Input();
            inUp.type = KeyboardSimulator.INPUT_KEYBOARD;
            inUp.ki.wVk = (Int16)key;
            inUp.ki.wScan = KeyboardSimulator.myGetScancode((ushort)key);
            if (key == VirtualKeyCode.LEFT || key == VirtualKeyCode.DOWN || key == VirtualKeyCode.RIGHT
                || key == VirtualKeyCode.UP)
                inUp.ki.dwFlags = KeyboardSimulator.KEYEVENTF_KEYUP;
            else
                inUp.ki.dwFlags = KeyboardSimulator.KEYEVENTF_KEYUP | KeyboardSimulator.KEYEVENTF_UNICODE;
            KeyboardSimulator.SendInput(1, ref inUp, Marshal.SizeOf(inUp));
        }

        public void mouseLeftDown()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);//make left button down

        }

        public void mouseLeftUp()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);//make left button down
        }

        public void mouseRightDown()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
        }

        public void mouseRightUp()
        {
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }

        public void doubleClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);//make left button down
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);//make left button down
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public void moveCursorToLeft()
        {
            x = x - 30;
            mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, x, (uint)(SystemInformation.PrimaryMonitorSize.Height / 2), 0, 0);
        }

        public void moveCursorToCenter()
        {
            mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, (uint)(SystemInformation.PrimaryMonitorSize.Width / 2), (uint)(SystemInformation.PrimaryMonitorSize.Height / 2), 0, 0);
        }

        public void moveCursorToRight()
        {
            x = x + 30;
            mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, x, (uint)(SystemInformation.PrimaryMonitorSize.Height / 2), 0, 0);
        }

        public void execProgramm(string p)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = p;
                Process.Start(startInfo);
            }
            catch
            {
                Console.WriteLine("Programme non trouvé");
            }
        }

        public void play(string mess)
        {
            SpeechSynthesizer synthetizer = new SpeechSynthesizer();
            synthetizer.SetOutputToDefaultAudioDevice();
            synthetizer.Volume = 100;
            synthetizer.Rate = 0;
            synthetizer.SpeakAsync(mess);
        }
    }
}
