using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using ICSharpCode.SharpZipLib.Zip.Compression;
using UnityEngine;

namespace Class
{
    [Serializable]
    public class NetworkPlayer : MonoBehaviour
    {
        [SerializeField] private List<Blade> _blades;

        public void UpdateBladePosition(Vector2 leftHandScreenPos, Vector2 rightHandSreenPos)
        {
            _blades[0].SetBladePos(leftHandScreenPos);
            _blades[1].SetBladePos(rightHandSreenPos);
        }
    }
}