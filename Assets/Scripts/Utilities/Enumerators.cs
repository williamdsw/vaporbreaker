
namespace Utilities
{
    public class Enumerators
    {
        /// <summary>
        /// Available Game States
        /// </summary>
        public enum GameStates
        {
            /// <summary>
            /// Level is completed
            /// </summary>
            LEVEL_COMPLETE, 
            
            /// <summary>
            /// Current Gameplay
            /// </summary>
            GAMEPLAY, 
            
            /// <summary>
            /// Game is paused
            /// </summary>
            PAUSE, 
            
            /// <summary>
            /// Is saving or loading progress
            /// </summary>
            SAVE_LOAD, 
            
            /// <summary>
            /// Is transitioning between fade in / fade out
            /// </summary>
            TRANSITION
        }

        /// <summary>
        /// Available block directions
        /// </summary>
        public enum Directions
        {
            Down, Left, Right, Up, None
        }
    }
}
