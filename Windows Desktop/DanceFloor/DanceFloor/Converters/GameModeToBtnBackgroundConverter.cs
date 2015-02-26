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
    public class GameModeToBtnBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var game = (IGame)value;
            string p = (parameter as string);

            if ((p == "single" && !game.IsMultiplayer) || (p == "multi" && game.IsMultiplayer))
                return GameUIConstants.GameModeSelectedBtnGradient;
            else
                return GameUIConstants.GameModeInactiveBtnGradient;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
