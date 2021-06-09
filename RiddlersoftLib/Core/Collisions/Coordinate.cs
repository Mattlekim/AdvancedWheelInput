using Microsoft.Xna.Framework;

namespace Riddlersoft.Core.Collisions
{
    /// <summary>
    /// this structur contains both a position and velocity for an 2d game object
    /// </summary>
    public struct Coordinate
    {
        /// <summary>
        /// the position of the object
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// the velocity of the object
        /// </summary>
        public Vector2 Velocity;

        /// <summary>
        /// creates a new coordinate instance
        /// </summary>
        /// <param name="position">the start position of the object</param>
        /// <param name="velocity">the start velocity of the object</param>
        public Coordinate(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
        }
    }
}
