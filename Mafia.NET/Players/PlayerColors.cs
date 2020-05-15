using System.Drawing;

namespace Mafia.NET.Players
{
    public enum PlayerColors : uint
    {
        Red = 0xFFB4141E,
        Blue = 0xFF0042FF,
        Teal = 0xFF1CA7EA,
        Purple = 0xFF6900A1,
        Yellow = 0xFFEBE129,
        Orange = 0xFFFE8A0E,
        Green = 0xFF168000,
        LightPink = 0xFFCCA6FC,
        Violet = 0xFFA633BF,
        Grey = 0xFF525494,
        DarkGreen = 0xFF168962,
        Brown = 0xFF753F06,
        LightGreen = 0xFF96FF91,
        Black = 0xFF464646,
        Pink = 0xFFE55BB0,
        Default = 0xFFAAAAAA
    }

    public static class PlayerColorsExtensions
    {
        public static PlayerColors From(int id)
        {
            return id switch
            {
                1 => PlayerColors.Red,
                2 => PlayerColors.Blue,
                3 => PlayerColors.Teal,
                4 => PlayerColors.Purple,
                5 => PlayerColors.Yellow,
                6 => PlayerColors.Orange,
                7 => PlayerColors.Green,
                8 => PlayerColors.LightPink,
                9 => PlayerColors.Violet,
                10 => PlayerColors.Grey,
                11 => PlayerColors.DarkGreen,
                12 => PlayerColors.Brown,
                13 => PlayerColors.LightGreen,
                14 => PlayerColors.Black,
                15 => PlayerColors.Pink,
                _ => PlayerColors.Default
            };
        }

        public static Color Color(this PlayerColors color)
        {
            return System.Drawing.Color.FromArgb((int) color);
        }
    }
}