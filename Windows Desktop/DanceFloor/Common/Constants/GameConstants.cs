using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class GameConstants
    {
        //Hit Points
        public const int BestHitPoints = 20;
        public const int MediumHitPoints = 10;
        public const int WorstHitPoints = 5;

        //Hit Times
        public const double BestHitTime = 0.05;
        public const double MediumHitTime = 0.13;
        public const double WorstHitTime = 0.2;

        //Life Points
        public const int HitLifePoints = 1;
        public const int BombLifePoints = 20;
        public const int MissLifePoints = 8;
        public const int WrongMomentOrActionLifePoints = 4;

        public const int FullLife = 100;

        public const string SongsDir = "Songs\\";
        public const string SongObjectFileName = "data.sm";
        public const string SongFileName = "song";
        public const string CoverFileName = "cover";
        public const string BackgroundFileName = "background";
    }
}
