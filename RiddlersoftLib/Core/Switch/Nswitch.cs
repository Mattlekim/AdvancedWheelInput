using System;
using System.Runtime.CompilerServices;

namespace Riddlersoft.Core.Switch
{
    public static class NSwitch
    {
        public const string SavePath = "save:/";

        // Called somewhere early in your startup code. Ex: Game constructor.

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Init();

        public static bool Mounted = false;

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool Mount();

        public static bool MountRom()
        {
            if (!Mounted)
            {
                Mounted = Mount();
                return Mounted;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void CommitSaveData();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Unmount();

        public static void UnmountRom()
        {
            if (Mounted)
            {
                Mounted = false;
                Unmount();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string GetNickName();

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string SetVibration(int index, 
            float leftMotorAmplitudeLow, float leftMotorAmplitudeHigh, float leftMotorLow, float leftMotorHigh,
                float rightMotorAmplitudeLow, float rightMotorAmplitudeHigh, float rightMotorLow, float rightMotorHigh);


        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsNSAAccount();

        //  [MethodImpl(MethodImplOptions.InternalCall)]
        //  public static extern Action NpadStyleSetUpdateEvent();
        //  }
    }

}