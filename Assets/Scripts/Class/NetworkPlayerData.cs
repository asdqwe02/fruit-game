using System;
using UnityEngine;

namespace Class
{
    [Serializable]
    public class NetworkPlayerData
    {
        public Int64 ID;
        public float LeftHandNormalPosX;
        public float LeftHandNormalPosY;
        public float RightHandNormalPosX;
        public float RightHandNormalPosY;
        public float PlayerBodyPositionX;
        public float PlayerBodyPositionY;
        public float PlayerBodyPositionZ;
    }
}