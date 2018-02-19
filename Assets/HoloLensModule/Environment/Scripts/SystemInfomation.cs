using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_UWP
using Windows.Networking.Connectivity;
using System.Net;
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif

namespace HoloLensModule.Environment
{
    public class SystemInfomation
    {

        // * デバイス名 string
        // バッテリー残量 % 残り時間 time
        // Wifi情報
        // * ネットワーク情報 ip subnetmask
        // ネットワーク情報 接続先 接続状態
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
            return SystemInfo.deviceName;
        }

        public string GetIPAddres()
        {
            string ipaddress = "";
#if UNITY_UWP
        var host=NetworkInformation.GetHostNames();
        foreach (var item in host)
        {
            if (item.Type == Windows.Networking.HostNameType.Ipv4 && item.IPInformation != null)
            {
                ipaddress = item.DisplayName;
            }
        }
#elif UNITY_EDITOR || UNITY_STANDALONE
            ipaddress = UnityEngine.Network.player.ipAddress;
#endif
            return ipaddress;
        }

        public string GetSubnetMask()
        {
            string subnetmask = "";
#if UNITY_UWP
        var host=NetworkInformation.GetHostNames();
        foreach (var item in host)
        {
            if (item.Type == Windows.Networking.HostNameType.Ipv4 && item.IPInformation != null)
            {
                byte length=item.IPInformation.PrefixLength.Value;
                BitArray bit = new BitArray(32, false);
                for (int i = 0; i < length; i++)
                {
                    bit[i] = true;
                }
                byte[] c1 = new byte[4];
                ((ICollection)bit).CopyTo(c1,0);
                subnetmask = c1[0].ToString() + "." + c1[1].ToString() + "." + c1[2].ToString() + "." + c1[3].ToString();
            }
        }
#elif UNITY_EDITOR || UNITY_STANDALONE
            var info = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            foreach (var item in info)
            {
                if (item.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                    item.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback &&
                    item.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Tunnel)
                {
                    var ips = item.GetIPProperties();
                    if (ips != null)
                    {
                        foreach (var ip in ips.UnicastAddresses)
                        {
                            if (UnityEngine.Network.player.ipAddress == ip.Address.ToString())
                            {
                                subnetmask = ip.IPv4Mask.ToString();
                            }
                        }
                    }
                }
            }
#endif
            return subnetmask;
        }

    }
}
