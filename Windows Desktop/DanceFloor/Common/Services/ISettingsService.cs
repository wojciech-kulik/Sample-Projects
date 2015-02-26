using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Common
{
    public interface ISettingsService
    {
        //you need to attach it to your PreviewKeyUp event in application
        void HandleKeyUp(object sender, KeyEventArgs e);

        IDictionary<PlayerAction, string> GetControlSettings(PlayerID player);

        void SetControlSetting(PlayerID player, PlayerAction action);
    }
}
