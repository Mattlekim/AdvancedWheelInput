using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Riddlersoft.Core.Collisions
{
    public class RayCollisionDetector
    {
        /// <summary>
        /// the cell or tile size
        /// </summary>
        private int _cellSize;

        public bool Bullet = false;
        /// <summary>
        /// the amount of bounce we want after a collisoin
        /// 0 is no bound 1 is perfect bounce
        /// </summary>
        private float _amountOfBounce = 1;
        public float AmountOfBounce
        {
            get
            {
                return _amountOfBounce;
            }
            set
            {
                _amountOfBounce = MathHelper.Clamp(value, 0, 1);
            }
        }

        /// <summary>
        /// the size of the grid you want to do collision check on
        /// usual the total size of the grid
        /// </summary>
        private Rectangle _gridBounds;

        /// <summary>
        /// this function fires as soon as an object enters the cell
        /// return false to ignore collision of tile ie air
        /// return true to enable the collision
        /// OnCellEnter(CollisionDirection dir, Point gridLocation, Object sender);
        /// </summary>
        public Func<CollisionDirection, Point, object, bool> OnCellEnter;

        /// <summary>
        /// this action fires for prossesing the collision with the cell
        /// ProssesCellColision(Collision col);
        /// </summary>
        public Action<Collision> ProssesCellColision;

        /// <summary>
        /// creates a new instace of the collision detector
        /// </summary>
        /// <param name="cellSize">the size in pixels of each cell</param>
        /// <param name="gridBounds">the size of the 2d array</param>
        public RayCollisionDetector(int cellSize, Rectangle gridBounds)
        {
            _cellSize = cellSize;
            _gridBounds = gridBounds;
        }


        private float CalculateDistance(float pos, float vel)
        {
            if (vel > 0)
                return (float)Math.Ceiling(pos / _cellSize) * _cellSize - pos;

            return pos - (float)Math.Floor(pos / _cellSize) * _cellSize;
        }

        private float CalculateTravelTime(float distance, float vel)
        {
            if (vel == 0)
                return float.PositiveInfinity;

            return distance / Math.Abs(vel);
        }

        private bool ValidateHorizontalColision<T>(Point p, Vector2 vel, float travelTime, T[,] grid)
        {
            if (travelTime > 1) //if no collision
                return false;

            int dir = Math.Sign(vel.X);
            Point tile = new Point(p.X + dir, p.Y);

            if (tile.X <= _gridBounds.X || tile.X >= _gridBounds.Width)
                return true;

            if (OnCellEnter != null)
                return OnCellEnter(CollisionDirection.Horizontal, tile, grid[tile.X, tile.Y]);
            
           // if (grid[p.X + dir, p.Y]) //if we can walk here there is not a collision
                //return false;

            //   if (grid[p.X, p.Y]) //if this side block is passible we have a collision
            //  return true;
            return true;
        }

        private bool ValidateVerticalColision<T>(Point p, Vector2 vel, float travelTime, T[,] grid)
        {
            if (travelTime > 1) //if no collision
                return false;

            int dir = Math.Sign(vel.Y);
            Point tile = new Point(p.X, p.Y + dir);

            if (tile.Y <= _gridBounds.Y || tile.Y >= _gridBounds.Height)
                return true;

            if (OnCellEnter != null)
                return OnCellEnter(CollisionDirection.Vertical, tile, grid[tile.X, tile.Y]);

            //  if (grid[p.X, p.Y + dir]) //if we can walk here there is not a collision
            // return false;

            //    if (grid[p.X, p.Y]) //if this side block is passible we have a collision
            //   return true;
            return true;
        }

        int updatenumber = 0;
        /// <summary>
        /// Handle colision for one update cycle
        /// </summary>
        /// <typeparam name="T">the tile type in your 2d array</typeparam>
        /// <param name="pos">the position of the object you want to check a colision with</param>
        /// <param name="vel">the velocity of the object you want to check a colision with</param>
        /// <param name="grid">the 2d array that represents you gameing grid</param>
        /// <returns>by default the collision detector will invert the velocity after a collision and also set the position.
        /// You can then set ether the pos and vel return to your object or you can handle it on your own
        /// </returns>
        public Coordinate HandleColision<T>(Vector2 pos, Vector2 vel, T[,] grid)
        {
           
        
            //first calculate distance to next cell
            Vector2 distance = new Vector2(CalculateDistance(pos.X, vel.X),
                CalculateDistance(pos.Y, vel.Y));

            //now calculate travel time
            Vector2 travelTime = new Vector2(CalculateTravelTime(distance.X, vel.X),
                CalculateTravelTime(distance.Y, vel.Y));

            Vector2 blockTraveTime = new Vector2(_cellSize / Math.Abs(vel.X), _cellSize / Math.Abs(vel.Y));
            Point gridLocation = new Point((int)Math.Floor(pos.X / _cellSize), (int)Math.Floor(pos.Y / _cellSize));
            List<CollisionDirection> dirs = new List<CollisionDirection>();

            Vector2 deltaTime = Vector2.Zero;
            while (travelTime.X <= 1 || travelTime.Y <= 1)
            {
                updatenumber++;
                dirs.Clear();
                bool HorCol = false, VerCol = false;


                if (travelTime.X <= 1)
                {
                    HorCol = ValidateHorizontalColision(gridLocation, vel, travelTime.X, grid);
                    if (!HorCol)
                    {
                        // deltaTraveTime += blockTraveTime;
                        travelTime.X += blockTraveTime.X;
                        //    if (deltaTraveTime.Y > deltaTraveTime.X)
                        gridLocation.X += Math.Sign(vel.X);
                        deltaTime.X += blockTraveTime.X;
                    }
                    else
                        dirs.Add(CollisionDirection.Horizontal);
                }

                if (travelTime.Y <= 1)
                {
                    VerCol = ValidateVerticalColision(gridLocation, vel, travelTime.Y, grid);
                    if (!VerCol)
                    {
                        //deltaTraveTime += blockTraveTime;
                        travelTime.Y += blockTraveTime.Y;
                        //  if (deltaTraveTime.Y < deltaTraveTime.X)
                        gridLocation.Y += Math.Sign(vel.Y);
                        deltaTime.Y += blockTraveTime.Y;

                    }
                    else
                        if (dirs.Count == 0) //if we already have a contact this time
                             dirs.Add(CollisionDirection.Vertical);
                    else
                        if (travelTime.Y > travelTime.X)
                            dirs.Add(CollisionDirection.Vertical);
                    else
                        dirs.Insert(0, CollisionDirection.Vertical);

                }
                if (dirs.Count == 0)
                    continue;

          
                CollisionDirection dir = CollisionDirection.Horizontal;
                int count = 0;
                while (dirs.Count > 0)
                {
                    dir = dirs[0];
                    dirs.RemoveAt(0);

                    switch (dir)
                    {
                        case CollisionDirection.Horizontal:
                          //  Console.WriteLine($"Collisoin Horizontal > {updatenumber}");
                            //gridLocation.X += Math.Sign(vel.X);
                            if (ProssesCellColision != null) //if user has collision responce
                            {
                                Point colGrid = gridLocation;
                                colGrid.X += Math.Sign(vel.X);
                                Collision col = new Collisions.Collision()
                                {
                                    Direction = CollisionDirection.Horizontal,
                                    GridLocation = colGrid,
                                    ObjectCoordinate = new Coordinate(pos, vel),
                                    Tile = grid[colGrid.X, colGrid.Y],
                                    Grid = grid,
                                };
                                ProssesCellColision(col);

                            }
                            pos += vel * travelTime.X;
                            pos.X -= Math.Sign(vel.X) * .5f; //take away 1 pixel so that we are not in the block
                            vel.X *= -_amountOfBounce;

                            //first calculate distance to next cell
                            distance.X = CalculateDistance(pos.X, vel.X);

                            //now calculate travel time
                            travelTime.X = CalculateTravelTime(distance.X, vel.X);

                           // if (travelTime.Y <= 1) //ONLY If there was a collisoin to check
                          //  {
                                distance.Y = CalculateDistance(pos.Y, vel.Y);
                                travelTime.Y = CalculateTravelTime(distance.Y, vel.Y) + deltaTime.Y;
                          //  }
                            break;

                        case CollisionDirection.Vertical:
                            // gridLocation.Y += Math.Sign(vel.Y);
                         //   Console.WriteLine($"Collisoin Vertical > {updatenumber}");
                            if (ProssesCellColision != null) //if user has collision responce
                            {
                                Point colGrid = gridLocation;
                                colGrid.Y += Math.Sign(vel.Y);
                                Collision col = new Collisions.Collision()
                                {
                                    Direction = CollisionDirection.Vertical,
                                    GridLocation = colGrid,
                                    ObjectCoordinate = new Coordinate(pos, vel),
                                    Tile = grid[colGrid.X, colGrid.Y],
                                    Grid = grid,
                                };
                                ProssesCellColision(col);
                            }

                            pos += vel * travelTime.Y;
                            pos.Y -= Math.Sign(vel.Y) * .5f;
                            vel.Y *= -_amountOfBounce;

                            distance.Y = CalculateDistance(pos.Y, vel.Y);
                            travelTime.Y = CalculateTravelTime(distance.Y, vel.Y);
                            

                      //      if (travelTime.X <= 1) //ONLY If there was a collisoin to check
                            {
                                //first calculate distance to next cell
                                distance.X = CalculateDistance(pos.X, vel.X);

                                //now calculate travel time
                                travelTime.X = CalculateTravelTime(distance.X, vel.X) +deltaTime.X;
                            }
                            break;
                    }
                    count++;


                }
               

            }
     

            return new Collisions.Coordinate(pos, vel);
        }

        private void HandleSimpleHorizontalCol<T>(ref Vector2 pos, ref Vector2 vel, Vector2 travelTime, Point gridLocation, T[,]grid)
        {
            if (ProssesCellColision != null) //if user has collision responce
            {
                Point colGrid = gridLocation;
                colGrid.X += Math.Sign(vel.X);
                Collision col = new Collisions.Collision()
                {
                    Direction = CollisionDirection.Horizontal,
                    GridLocation = colGrid,
                    ObjectCoordinate = new Coordinate(pos, vel),
                    Tile = grid[colGrid.X, colGrid.Y],
                    Grid = grid,
                };
                ProssesCellColision(col);

            }
            pos.X += vel.X * travelTime.X;
            pos.X -= Math.Sign(vel.X) * .5f; //take away 1 pixel so that we are not in the block
            vel.X *= -_amountOfBounce;
        }

        private void HandleSimpleVerticallCol<T>(ref Vector2 pos, ref Vector2 vel, Vector2 travelTime, Point gridLocation, T[,] grid)
        {
            if (ProssesCellColision != null) //if user has collision responce
            {
                Point colGrid = gridLocation;
                colGrid.Y += Math.Sign(vel.Y);
                Collision col = new Collisions.Collision()
                {
                    Direction = CollisionDirection.Vertical,
                    GridLocation = colGrid,
                    ObjectCoordinate = new Coordinate(pos, vel),
                    Tile = grid[colGrid.X, colGrid.Y],
                    Grid = grid,
                };
                ProssesCellColision(col);
            }

            pos.Y += vel.Y * travelTime.Y;
            pos.Y -= Math.Sign(vel.Y) * .5f;
            vel.Y *= -_amountOfBounce;
        }

        public Coordinate HandleSimpleColision<T>(Vector2 pos, Vector2 vel, T[,] grid, CollisionDirection dir)
        {


            //first calculate distance to next cell
            Vector2 distance = new Vector2(CalculateDistance(pos.X, vel.X),
                CalculateDistance(pos.Y, vel.Y));

            //now calculate travel time
            Vector2 travelTime = new Vector2(CalculateTravelTime(distance.X, vel.X),
                CalculateTravelTime(distance.Y, vel.Y));

            Vector2 blockTraveTime = new Vector2(_cellSize / Math.Abs(vel.X), _cellSize / Math.Abs(vel.Y));
            Point gridLocation = new Point((int)Math.Floor(pos.X / _cellSize), (int)Math.Floor(pos.Y / _cellSize));
          
            while (travelTime.X <= 1 || travelTime.Y <= 1)
            {
                bool HorCol = false, VerCol = false;


                if (travelTime.X <= 1)
                {
                    HorCol = ValidateHorizontalColision(gridLocation, vel, travelTime.X, grid);
                    if (HorCol)
                    {
                        HandleSimpleHorizontalCol<T>(ref pos, ref vel, travelTime, gridLocation, grid);
                        travelTime.X = float.PositiveInfinity;
                    }
                    else
                        travelTime.X += blockTraveTime.X;
                }

                if (travelTime.Y <= 1)
                {
                    VerCol = ValidateVerticalColision(gridLocation, vel, travelTime.Y, grid);
                    if (VerCol)
                    {
                        HandleSimpleVerticallCol<T>(ref pos, ref vel, travelTime, gridLocation, grid);
                        travelTime.Y = float.PositiveInfinity;
                    }
                    else
                        travelTime.Y += blockTraveTime.Y;

                }

            }

            return new Collisions.Coordinate(pos, vel);
        }
    }
}
