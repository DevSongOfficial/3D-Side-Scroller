using System.Collections;
using System.Collections.Generic;

public static class Animation
{
    public enum State {               None = 0, 
                       /*For Player*/ BT_Move, Jump, Swing, 
                       /*For Zombie*/ BT_ZombieMove1 }

    public static class Parameter
    {
        public static readonly string MoveSpeed = "MoveSpeed";

    }
}