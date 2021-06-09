using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Riddlersoft.Core
{
    /// <summary>
    /// my pathfollower code
    /// this will work with as many points as needed
    /// </summary>
    public class PathFollower
    {
        public List<Vector2> Points { get; private set; }

        /// <summary>
        /// is true when it compleates the path
        /// </summary>
        public bool Compleated { get; private set; }

        /// <summary>
        /// the total time to compleate the path
        /// </summary>
        private float _timeToCompleatePath = 0;

        /// <summary>
        /// the current time
        /// </summary>
        private float _currentTime = 0;

        /// <summary>
        /// a buffer of each lerped point
        /// </summary>
        private Vector2[] _buffers;

        /// <summary>
        /// the time each split should be
        /// </summary>
        private float _timeSplit;

        /// <summary>
        /// how may parts its made up of
        /// </summary>
        private float _parts;

        /// <summary>
        /// creates the path follower with the time to compleate
        /// </summary>
        /// <param name="time">how long to compleate the path</param>
        public PathFollower(float time)
        {
            _timeToCompleatePath = time;
            Points = new List<Vector2>();
        }

        /// <summary>
        /// adds a new point to the path
        /// </summary>
        /// <param name="point"></param>
        public void AddPoint(Vector2 point)
        {
            Points.Add(point);
        }

        /// <summary>
        /// creates the buffer call this after your last point has been added
        /// </summary>
        public void CreateBuffer()
        {
            ///we need to buffer
            _buffers = new Vector2[Points.Count - 1];
            _buffers[0]= Points[0];
            for (int i = 0; i < Points.Count - 2; i++)
            {
                Vector2 v1 = Vector2.Lerp(_buffers[i], Points[i + 1], .5f);
                Vector2 v2 = Vector2.Lerp(Points[i + 1], Points[i + 2], .5f);
                _buffers[i + 1] = Vector2.Lerp(v1, v2, .5f);
            }
            _parts = Points.Count - 1;
            _timeSplit = (_timeToCompleatePath / _parts) * 2f;
            lastPos = Points[0];
        }

        public void Reset()
        {
            _currentTime = 0;
            Compleated = false;
            lastPos = Points[0];
        }


        public Vector2 moveDir { get; private set; }

        private Vector2 lastPos = Vector2.Zero;
        public Vector2 Update(float dt)
        {

            if (Compleated)
            {
                moveDir = Vector2.Zero;
                lastPos = Points[Points.Count - 1];
                return Points[Points.Count - 1];
            }
            _currentTime += dt;

            if (_currentTime > _timeToCompleatePath)
            {
                Compleated = true;
                moveDir = Vector2.Zero;
                lastPos = Points[Points.Count - 1];
                return Points[Points.Count - 1];
            }

            float halfSplit = _timeSplit * .5f;

            int index = (int)Math.Floor(_currentTime / halfSplit);

            float per = (float)Math.Round((_currentTime - (index * halfSplit)) / (_timeSplit), 2);

            if (index + 3 < Points.Count)
            {
                Vector2 l1 = Vector2.Lerp(_buffers[index], Points[index + 1], per);
                Vector2 l2 = Vector2.Lerp(Points[index + 1], Points[index + 2], per);

                Vector2 l3 = Vector2.Lerp(l1, l2, per);
                moveDir = l3 - lastPos;
                lastPos = l3;
                return l3;
            }

            float lasthalf = _timeToCompleatePath - _timeSplit;
            //per = 0f;
            per = (float)Math.Round((_currentTime - lasthalf) / (_timeSplit), 2);
            Vector2 f1 = Vector2.Lerp(_buffers[_buffers.Length - 2], Points[Points.Count - 2], per);
            Vector2 f2 = Vector2.Lerp(Points[Points.Count - 2], Points[Points.Count - 1], per);

            Vector2 f3 = Vector2.Lerp(f1, f2, per);
            moveDir = f3 - lastPos;
            lastPos = f3;
            return f3;
        }

        public void Start(float timeToCompleate)
        {
            _timeToCompleatePath = timeToCompleate;
            _currentTime = 0;
        }
    }
}
