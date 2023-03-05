using System;
using System.Collections.Generic;
using UnityEngine;

namespace DiGro {

    public enum PalletColor {
        Good,
        Bad,
        Select,

        FirstPlayerPawn,
        SeconPlayerdPawn,

        FirstPlayerTile,
        SecondPlayerTile,

        Empty,

        Camera
    }

    [CreateAssetMenu(fileName = "pallete.asset", menuName = "Custom/Pallete", order = 51)]
    public class Pallete : ScriptableObject {

        public string id = "pallete";
        public bool update;

        public Color Good;
        public Color Bad;
        public Color Select;

        public Color FirstPlayerPawn;
        public Color SeconPlayerdPawn;

        public Color FirstPlayerTile;
        public Color SecondPlayerTile;

        public Color Camera;


        public Color GetColor(PalletColor type) {
            switch (type) {
                case PalletColor.Good: return Good;
                case PalletColor.Bad: return Bad;
                case PalletColor.Select: return Select;

                case PalletColor.FirstPlayerPawn: return FirstPlayerPawn;
                case PalletColor.SeconPlayerdPawn: return SeconPlayerdPawn;

                case PalletColor.FirstPlayerTile: return FirstPlayerTile;
                case PalletColor.SecondPlayerTile: return SecondPlayerTile;

                case PalletColor.Camera: return Camera;

                case PalletColor.Empty: return new Color(0, 0, 0, 0);
                default: return new Color(1, 0, 0);
            }
        }

        private Color GetOptionalColor(OptionalColor optColor) {
            if (optColor.use)
                return optColor.color;
            return GetColor(optColor.dubler);
        }


        [Serializable]
        public class OptionalColor {
            public bool use = false;
            public Color color;
            public PalletColor dubler = PalletColor.Empty;
        }

    }
}