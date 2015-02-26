using Common;
using GameLayer;
using DanceFloor.Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DanceFloor.Controls
{
    /// <summary>
    /// Interaction logic for SongsList.xaml
    /// </summary>
    public partial class SongsList : UserControl
    {
        Key? heldKey, releasedKey;

        public SongsList()
        {
            InitializeComponent();
            Loaded += SongsList_Loaded;
        }

        void SongsList_Loaded(object sender, RoutedEventArgs e)
        {
            if (ItemsSource != null && SelectedSong == null)
            {
                SelectedSong = ItemsSource.OfType<ISong>().FirstOrDefault();
            }
        }

        public void SongsList_HandleKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right)
                releasedKey = e.Key;
        }

        public void SongsList_HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Left && e.Key != Key.Right)
                return;

            if (heldKey.HasValue)
                return;

            heldKey = e.Key;
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        //animation of moving list
        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            double max_offset = GameUIConstants.SongItemWidth * (ItemsSource.OfType<ISong>().Count() - 1);
            var transform = (moveablePanel.RenderTransform as TranslateTransform);

            //animation while holding key
            if (heldKey.HasValue)
            {
                //move panel
                if (heldKey.Value == Key.Left)
                    transform.X = Math.Min(0, transform.X + GameUIConstants.SongsListMovePixelsPerFrame);
                else
                    transform.X = Math.Max(-max_offset, transform.X - GameUIConstants.SongsListMovePixelsPerFrame);

                //modify SelectedSong
                int index = (int)Math.Abs(transform.X / GameUIConstants.SongItemWidth);
                index = heldKey.Value == Key.Left ? index : Math.Min(ItemsSource.OfType<ISong>().Count() - 1, index + 1);
                SelectedSong = ItemsSource.OfType<ISong>().Skip(index).First();
            }

            //animation after released key
            if (releasedKey.HasValue)
            {
                double whatsLeft = Math.Abs(transform.X % GameUIConstants.SongItemWidth);
                if (whatsLeft < GameUIConstants.SongsListMovePixelsPerFrame)
                {
                    //align
                    if (releasedKey.Value == Key.Left)
                        transform.X = Math.Min(0, transform.X + whatsLeft);
                    else
                        transform.X = Math.Max(-max_offset, transform.X - whatsLeft);

                    //modify SelectedSong
                    int index = (int)Math.Abs(transform.X / GameUIConstants.SongItemWidth);
                    SelectedSong = ItemsSource.OfType<ISong>().Skip(index).First();

                    //stop animation
                    heldKey = null;
                    releasedKey = null;
                    CompositionTarget.Rendering -= CompositionTarget_Rendering;
                }
            }
        }




        public static void OnSetSelectedSong(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                (e.OldValue as ISong).IsSelected = false;
            if (e.NewValue != null)
                (e.NewValue as ISong).IsSelected = true;

            var songsList = (d as SongsList);
            if (!songsList.heldKey.HasValue && !songsList.releasedKey.HasValue) //if animation is not running
            {
                //move list to selected song
                int index = songsList.ItemsSource.OfType<ISong>().ToList().FindIndex(s => s.FilePath == ((ISong)e.NewValue).FilePath);
                (songsList.moveablePanel.RenderTransform as TranslateTransform).X = -GameUIConstants.SongItemWidth * index;
            }
        }

        #region SelectedSong Property
        public ISong SelectedSong
        {
            get { return (ISong)GetValue(SelectedSongProperty); }
            set { SetValue(SelectedSongProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedSong.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedSongProperty =
            DependencyProperty.Register("SelectedSong", typeof(ISong), typeof(SongsList), new PropertyMetadata(null, OnSetSelectedSong));
        #endregion

        #region ItemsSource Property
        public System.Collections.IEnumerable ItemsSource
        {
            get { return (System.Collections.IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(System.Collections.IEnumerable), typeof(SongsList), new PropertyMetadata(null));
        #endregion
    }
}
