using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Shell;

namespace ClientsManager.Tombstoning
{
    public class TombstoningContainer
    {
        public static bool HasValue(TombstoningVariables key)
        {
            if (PhoneApplicationService.Current.State != null)
            {
                return PhoneApplicationService.Current.State.ContainsKey(Enum.GetName(typeof(TombstoningVariables), key));
            }
            return false;
        }  
        public static object GetValue(TombstoningVariables key)
        {
            return PhoneApplicationService.Current.State[Enum.GetName(typeof(TombstoningVariables), key)];
        }
        public static void SetValue(TombstoningVariables key, object value)
        {
            PhoneApplicationService.Current.State[Enum.GetName(typeof(TombstoningVariables), key)] = value;
        }
        public static void RemoveKey(TombstoningVariables key)
        {
            if (HasValue(key))
            {
                PhoneApplicationService.Current.State.Remove(Enum.GetName(typeof(TombstoningVariables), key));
            }
        }
    }
}
