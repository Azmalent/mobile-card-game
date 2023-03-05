using System;

namespace SpiralJumper.Model
{
    public enum PlatformEffect
    {
        None = 0,
        Red = 1,
        Jump = 2,
    }

    public enum PlatformType
    {
        Simple85 = 0,
        Simple50 = 2,
        Simple35 = 3,
        SaveRing = 1,
        Red50Left = 4,
        Red50Right = 5,
        Red50LeftRight = 6,
        Red35Left = 7,
        Red35Right = 8,
        Red35LeftRight = 9,
        Jump50Left = 10,
    }
}