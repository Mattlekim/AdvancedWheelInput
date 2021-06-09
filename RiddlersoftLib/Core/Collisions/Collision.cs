using Microsoft.Xna.Framework;
namespace Riddlersoft.Core.Collisions
{
   
    public enum CollisionDirection
    {
        Horizontal,
        Vertical,
    }

    public struct Collision
    {
        public Point GridLocation { get; internal set; }
        public Coordinate ObjectCoordinate { get; internal set; }
        public CollisionDirection Direction { get; internal set; }
        public object Tile { get; internal set; }
        public object Grid { get; internal set; }
    }
}
