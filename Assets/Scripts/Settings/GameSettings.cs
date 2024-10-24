// using System;
// using UnityEngine;

// namespace AmazingTrack
// {
//     [Serializable]
//     public class GameSettings
//     {
//         public float BallInitialSpeed = 5f;
//         public GameMode GameMode = GameMode.Normal;
//         [Range(1, 10)] public int Level = 1;
//         public bool RandomCrystals;
        
//         public Color BackgroundColor1 = Color.red;
//         public Color BackgroundColor2 = Color.blue;
//         public float BackgroundChangeDuration = 5.0f;
        
//         public float CameraLerpRate = 5.0f;
//         public LayerMask CrystalMask; 
//     }
// }

using System;
using UnityEngine;

namespace AmazingTrack
{
    [Serializable]
    public class GameSettings
    {
        public float BallInitialSpeed = 5f;
        public GameMode GameMode =GameMode.Normal;
        [Range(1, 10)] public int Level = 1;
        public bool RandomCrystals;
        
        // Gradient colors for background skybox
        public Color BackgroundColorTop1 = Color.red;
        public Color BackgroundColorBottom1 = Color.blue;
        public Color BackgroundColorTop2 = Color.blue;
        public Color BackgroundColorBottom2 = Color.red;
        public float BackgroundChangeDuration = 5.0f; // Time between color transitions

        public float CameraLerpRate = 5.0f;
        public LayerMask CrystalMask;
        
        // Skybox material reference for dynamic changes
        public Material SkyboxMaterial;
    }
}
