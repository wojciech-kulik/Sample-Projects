using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Common
{
    public class ControlHelper
    {
        public static PlayerAction KeyToPlayerAction(Key key)
        {
            switch(key)
            {
                case Key.Up:
                case Key.W:
                    return PlayerAction.Up;
                case Key.Down:
                case Key.S:
                    return PlayerAction.Down;
                case Key.Left:
                case Key.A:
                    return PlayerAction.Left;
                case Key.Right:
                case Key.D:
                    return PlayerAction.Right;
                case Key.Escape:
                    return PlayerAction.Back;
                case Key.Enter:
                    return PlayerAction.Enter;
                default:
                    throw new InvalidOperationException("Unsupported key");
            }
        }

        public static PlayerID KeyToPlayerID(Key key)
        {
            switch (key)
            {
                case Key.Enter:
                case Key.Escape:
                case Key.Up:
                case Key.Down:
                case Key.Left:
                case Key.Right:
                    return PlayerID.Player1;

                case Key.W:
                case Key.A:
                case Key.S:
                case Key.D:
                    return PlayerID.Player2;

                default:
                    throw new InvalidOperationException("Unsupported key");
            }
        }
    }
}
