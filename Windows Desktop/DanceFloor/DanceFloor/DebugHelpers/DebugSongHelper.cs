//#define DEBUG_TIMING
//#define DEBUG_HIT_TIME

using Common;
using GameLayer;
using DanceFloor.Constants;
using DanceFloor.ViewModels;
using DanceFloor.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DanceFloor.DebugHelpers
{
    public class DebugSongHelper
    {
        public static void GenerateRandomSongsDB(int count = 20)
        {
            if (Directory.Exists(GameConstants.SongsDir))
                Directory.Delete(GameConstants.SongsDir, true);

            var r = new Random();
            for (int i = 0; i < count; i++)
            {
                var song = GenerateSong(r.Next(120, 300));
                song.Title = "Title";
                song.Artist = "Artist " + (i + 1).ToString();
                song.SaveToFile();
            }
        }

        public static Song GenerateSong(int seconds = 300, Difficulty difficulty = Difficulty.Easy)
        {    
            Song result = new Song();
            result.FilePath = @"Songs\song.mp3";
            result.CoverPath = @"..\..\Images\game_background.jpg";
            result.Duration = new TimeSpan(0, 0, seconds);

            var r = new Random();
            for (int i = 0; i <= 2; i++)
                if (r.Next(0, 100) < 50)
                    GenerateSequence(r, result, (Difficulty)i);

            if (result.Sequences.Keys.Count == 0)
                GenerateSequence(r, result, Difficulty.Easy);

            return result;
        }

        public static void GenerateSequence(Random randomGenerator, ISong song, Difficulty difficulty)
        {
            var r = randomGenerator;
            Sequence sequence = new Sequence();
            if (song.Sequences.ContainsKey(difficulty))
                song.Sequences.Remove(difficulty);
            song.Sequences.Add(difficulty, sequence);

            int notesCount = 3;
            switch(difficulty)
            {
                case Difficulty.Easy:
                    notesCount = 2;
                    break;
                case Difficulty.Medium:
                    notesCount = 2;
                    break;
                case Difficulty.Hard:
                    notesCount = 3;
                    break;
            }

            for (int i = 2; i < song.Duration.TotalSeconds - 2; i++)
            {
                if (difficulty == Difficulty.Medium)
                    notesCount = r.Next(2, 4);

                for (int j = 0; j < notesCount ; j++)
                {
                    SeqElemType elemType = (SeqElemType)r.Next(0, 4);
                    sequence.AddElement(new SequenceElement() { Type = elemType, IsBomb = r.Next(0, 100) < 10, Time = new TimeSpan(0, 0, 0, i, r.Next(200, 1000)) });
                }
            }
        }

        public static void AddTimeToNotes(UIElementCollection notes, SeqElemType elemType, double top)
        {
            #if DEBUG_TIMING            

            TextBlock tb = new TextBlock() { FontSize = 20, Text = String.Format("{0:N2}", top / GameUIConstants.PixelsPerSecond) };
            Canvas.SetTop(tb, top);
            switch (elemType)
            {
                case SeqElemType.LeftArrow:
                    Canvas.SetLeft(tb, GameUIConstants.LeftArrowX);
                    break;
                case SeqElemType.DownArrow:
                    Canvas.SetLeft(tb, GameUIConstants.DownArrowX);
                    break;
                case SeqElemType.UpArrow:
                    Canvas.SetLeft(tb, GameUIConstants.UpArrowX);
                    break;
                case SeqElemType.RightArrow:
                    Canvas.SetLeft(tb, GameUIConstants.RightArrowX);
                    break;
            }
            notes.Add(tb);

            #endif
        }

        public static void ShowCurrentTimeInsteadPoints(Storyboard animation, IMusicPlayerService musciPlayerService, GameView view)
        {
            #if DEBUG_TIMING

            new Thread(new ThreadStart(() =>
                {
                    while (true)
                    {
                        if (Application.Current == null)
                            return;

                        Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
                        {
                            var time = musciPlayerService.CurrentTime;
                            view.p1PointsBar.Points = "S: " + time.TotalSeconds.ToString("N2") + " | A: " + animation.GetCurrentTime().TotalSeconds.ToString("N2");
                        }));
                        Thread.Sleep(50);
                    }
                })).Start();

            #endif
        }

        public static void ShowHitTimeDifferenceInsteadPoints(GameView view, Storyboard animation)
        {          
            #if DEBUG_HIT_TIME
            var currentTime = animation.GetCurrentTime();
            var note = view.p1Notes.Children.OfType<Image>().ToList().FirstOrDefault(n => Math.Abs(((ISequenceElement)n.Tag).Time.TotalSeconds - currentTime.TotalSeconds) < 0.2);

            string hitInfo = note != null ? "YES" : "NO";

            if (note != null)
                hitInfo += " " + (((ISequenceElement)note.Tag).Time.TotalSeconds - currentTime.TotalSeconds).ToString("N2");
            view.p1PointsBar.Points = hitInfo;
            #endif
        }

        public static bool HandleKeyPressed(GameViewModel model, GameKeyEvent e)
        {
            #if DEBUG_HIT_TIME || DEBUG_TIMING
            if (e.PlayerAction == PlayerAction.Back)
                return false;
            else if (e.PlayerAction == PlayerAction.Enter)
                model.ResumeGame();
            else
                model.PauseGame();

            return true;
            #else
                return false;
            #endif
        }
    }
}
