using Caliburn.Micro;
using Common;
using DanceFloor.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DanceFloor.Converters
{
    public class DifficultyToBtnBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var player = (IPlayer)value;
            string d = (parameter as string);

            Difficulty difficulty = Difficulty.Easy;
            if (d == "easy")
                difficulty = Difficulty.Easy;
            else if (d == "medium")
                difficulty = Difficulty.Medium;
            else
                difficulty = Difficulty.Hard;

            if (player.Difficulty != difficulty)
                return GameUIConstants.GameModeBtnGradient;
            else
                return GameUIConstants.GameModeSelectedBtnGradient;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
