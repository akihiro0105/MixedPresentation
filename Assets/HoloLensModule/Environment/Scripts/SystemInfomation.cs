using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR || UNITY_UWP
#if !UNITY_2017_2_OR_NEWER

#else

#endif
#endif

namespace HoloLensModule.Environment
{
    public class SystemInfomation
    {

        // デバイス名 string
        // バッテリー残量 % 残り時間 time
        // Wifi情報
        // ネットワーク情報 接続状態 接続先 ip subnetmask
        // cpu稼働率 %
        // メモリ使用率 % MB
        // 温度 c
        // bluetooth情報
        // マスター音量 %
        // ディスプレイ輝度 %

        public SystemInfomation()
        {

        }

        public string GetDevicename()
        {
            return "name";
        }

        public string GetIPAddres()
        {
            return "";
        }

        public string GetSubnetMask()
        {
            return "";
        }

    }
}
