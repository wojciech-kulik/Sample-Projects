using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DanceFloor.Constants
{
    public class GameUIConstants
    {
        public const int PixelsPerSecond = 260;
        public const int ArrowWidthHeight = 65;
        public const int MarginBetweenArrows = 50;

        public const int LeftArrowX = 0;
        public const int DownArrowX = ArrowWidthHeight + MarginBetweenArrows;
        public const int UpArrowX = DownArrowX + ArrowWidthHeight + MarginBetweenArrows;
        public const int RightArrowX = UpArrowX + ArrowWidthHeight + MarginBetweenArrows;

        public const string ResourceImagesPath = "pack://application:,,,/DanceFloor;component/Images/";
        public const string BombImage = ResourceImagesPath + "bomb.png";
        public const string P1ArrowImage = ResourceImagesPath + "active_arrow_purple.png";
        public const string P2ArrowImage = ResourceImagesPath + "active_arrow_blue.png";
        public const string DefaultGameBackground = ResourceImagesPath + "game_background.jpg";

        public const int SongItemWidth = 250;
        public const double SongsListMovePixelsPerFrame = 10;

        public static readonly LinearGradientBrush MainMenuBtnGradient =
            new LinearGradientBrush(new Color() { A = 0xFF, R = 0x58, G = 0x66, B = 0xBB }, new Color() { A = 0xFF, R = 0x2C, G = 0x2C, B = 0x93 }, 90.0);
        public static readonly LinearGradientBrush MainMenuSelectedBtnGradient =
            new LinearGradientBrush(new Color() { A = 0xFF, R = 0x80, G = 0x92, B = 0xFF }, new Color() { A = 0xFF, R = 0x49, G = 0x49, B = 0xB2 }, 90.0);

        public static readonly LinearGradientBrush GameModeSelectedBtnGradient = 
            new LinearGradientBrush(new Color() { A = 0xFF, R = 0xF5, G = 0xBF, B = 0x2F }, new Color() { A = 0xFF, R = 0xAA, G = 0x7F, B = 0x0C }, 90.0);
        public static readonly LinearGradientBrush GameModeBtnGradient =
            new LinearGradientBrush(new Color() { A = 0xFF, R = 0xD1, G = 0x7A, B = 0x22 }, new Color() { A = 0xFF, R = 0x9C, G = 0x50, B = 0x1B }, 90.0);
        public static readonly LinearGradientBrush GameModeInactiveBtnGradient =
            new LinearGradientBrush(new Color() { A = 0xFF, R = 0x3E, G = 0x3E, B = 0x3E }, new Color() { A = 0xFF, R = 0x32, G = 0x32, B = 0x32 }, 90.0);
        public static readonly SolidColorBrush GameModeInactiveBtnForeground = new SolidColorBrush(Colors.Gray);

        public static readonly SolidColorBrush PopupBtnBackground = new SolidColorBrush(new Color() { A = 0xFF, R = 0x97, G = 0x97, B = 0x97 });
        public static readonly SolidColorBrush PopupSelectedBtnBackground = new SolidColorBrush(new Color() { A = 0xFF, R = 0xD3, G = 0xA3, B = 0x5D });

        public static readonly LinearGradientBrush RecordCreatedBtnGradient =
            new LinearGradientBrush(new Color() { A = 0xFF, R = 0x71, G = 0xFD, B = 0x2F }, new Color() { A = 0xFF, R = 0x4E, G = 0xA4, B = 0x25 }, 90.0);

        public const int CountDownDuration = 3;
    }
}
