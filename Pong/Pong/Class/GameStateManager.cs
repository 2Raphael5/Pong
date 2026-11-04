using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class
{
    public static class GameStateManager
    {
        public static GameState CurrentState { get; private set; }
        public static void ChangeState(GameState newState)
        {
            CurrentState = newState;
        }
        public static bool Is(GameState state)
        {
            return CurrentState == state;
        }
    }
    }
