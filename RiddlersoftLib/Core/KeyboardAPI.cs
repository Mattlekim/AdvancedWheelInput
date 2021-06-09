using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
namespace Riddlersoft.Core
{
    public class TimedKey
    {
        /// <summary>
        /// the maximume key lifetime in seconds
        /// </summary>
        public const float MaxKeyLife = .2f;
        /// <summary>
        /// the lifetime of this key
        /// </summary>
        public float Life;

        /// <summary>
        /// the key we are tracking
        /// </summary>
        public Keys Key;

        public TimedKey(Keys key)
        {
            Life = 0; //set life to zero
            Key = key;
        }
    }
    /// <summary>
    /// A gloable keybaord API that is accesable from anywhere in the program
    /// This will allow us to share the keyboard input without having to have several keyboard.getstate functions
    /// </summary>
    public class KeyboardAPI: GameComponent
    {
        private static KeyboardState kb, lkb;

        public static KeyboardState CurrentKeyBoard { get { return kb; } }

        private static List<TimedKey> TimedKeys = new List<TimedKey>();

        //A list of all keychars been pressed in this update sycle
        private static Queue<char> _keyBuffer = new Queue<char>();

        /// <summary>
        /// Checks to see if there is a key in the buffer
        /// </summary>
        public static bool IsKeyAvalible {  get { if (_keyBuffer.Count > 0)
                                                    return true;
                                                else
                                                    return false; } }

        private static Dictionary<Keys, char> _xnaKeyLookupTable = new Dictionary<Keys, char>();

        public static bool IsCharKeyboardEnabled { get; private set; } = false;
        public static void EnableCharKeyboard()
        {
            if (IsCharKeyboardEnabled)
                return;

            IsCharKeyboardEnabled = true;
            _xnaKeyLookupTable.Add(Keys.A, 'a');
            _xnaKeyLookupTable.Add(Keys.B, 'b');
            _xnaKeyLookupTable.Add(Keys.C, 'c');
            _xnaKeyLookupTable.Add(Keys.D, 'd');
            _xnaKeyLookupTable.Add(Keys.E, 'e');
            _xnaKeyLookupTable.Add(Keys.F, 'f');
            _xnaKeyLookupTable.Add(Keys.G, 'g');
            _xnaKeyLookupTable.Add(Keys.H, 'h');
            _xnaKeyLookupTable.Add(Keys.I, 'i');
            _xnaKeyLookupTable.Add(Keys.J, 'j');
            _xnaKeyLookupTable.Add(Keys.K, 'k');
            _xnaKeyLookupTable.Add(Keys.L, 'l');
            _xnaKeyLookupTable.Add(Keys.M, 'm');
            _xnaKeyLookupTable.Add(Keys.N, 'n');
            _xnaKeyLookupTable.Add(Keys.O, 'o');
            _xnaKeyLookupTable.Add(Keys.P, 'p');
            _xnaKeyLookupTable.Add(Keys.Q, 'q');
            _xnaKeyLookupTable.Add(Keys.R, 'r');
            _xnaKeyLookupTable.Add(Keys.S, 's');
            _xnaKeyLookupTable.Add(Keys.T, 't');
            _xnaKeyLookupTable.Add(Keys.U, 'u');
            _xnaKeyLookupTable.Add(Keys.V, 'v');
            _xnaKeyLookupTable.Add(Keys.W, 'w');
            _xnaKeyLookupTable.Add(Keys.X, 'x');
            _xnaKeyLookupTable.Add(Keys.Y, 'y');
            _xnaKeyLookupTable.Add(Keys.Z, 'z');
            _xnaKeyLookupTable.Add(Keys.OemPlus, '=');

            _xnaKeyLookupTable.Add(Keys.D0, '0');
            _xnaKeyLookupTable.Add(Keys.D1, '1');
            _xnaKeyLookupTable.Add(Keys.D2, '2');
            _xnaKeyLookupTable.Add(Keys.D3, '3');
            _xnaKeyLookupTable.Add(Keys.D4, '4');
            _xnaKeyLookupTable.Add(Keys.D5, '5');
            _xnaKeyLookupTable.Add(Keys.D6, '6');
            _xnaKeyLookupTable.Add(Keys.D7, '7');
            _xnaKeyLookupTable.Add(Keys.D8, '8');
            _xnaKeyLookupTable.Add(Keys.D9, '9');
            _xnaKeyLookupTable.Add(Keys.OemMinus, '-');

            _xnaKeyLookupTable.Add(Keys.Space, ' ');
            _xnaKeyLookupTable.Add(Keys.OemPeriod, '.');
            _xnaKeyLookupTable.Add(Keys.OemComma, ',');
            _xnaKeyLookupTable.Add(Keys.Enter, '\n');



            _xnaKeyLookupTable.Add(Keys.Back, Convert.ToChar(8)); //the hash char will be used to denote that more needs to be done than a simple convertion
            
            _xnaKeyLookupTable.Add(Keys.End, '#'); //the hash char will be used to denote that more needs to be done than a simple convertion
            _xnaKeyLookupTable.Add(Keys.Home, '#'); //the hash char will be used to denote that more needs to be done than a simple convertion
            _xnaKeyLookupTable.Add(Keys.Escape, '#'); //the hash char will be used to denote that more needs to be done than a simple convertion

        }

        public static char ReadKey()
        {
            return _keyBuffer.Dequeue();
        }

        private static bool _supressXna = false;

        public KeyboardAPI(Game game) : base(game)
        {
        }

        /// <summary>
        /// this fuction will disable all xna keyboard keys when run
        /// the purpues is when needing text input for a text box
        /// </summary>
        public static void DisableXnaInputThisUpdate()
        {
            _supressXna = true;
        }

        private static void XnaKeyToChar()
        {
            Keys[] ckey = kb.GetPressedKeys(); //get all current key pressed
           
            Keys[] okey = lkb.GetPressedKeys(); //get old keypress
            _keyBuffer.Clear(); //clear the buffer from old updates
            for (int i=0; i < ckey.Length; i++) 
                if (!okey.Contains(ckey[i])) //if a key press exist this updat but not last we have a new keypress
                {
                    if (_xnaKeyLookupTable.ContainsKey(ckey[i]))
                        if (kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift))
                            _keyBuffer.Enqueue(Char.ToUpper(_xnaKeyLookupTable[ckey[i]]));
                    else
                            _keyBuffer.Enqueue(_xnaKeyLookupTable[ckey[i]]);
                }
        }

        /// <summary>
        /// gets the keyboards states. 
        /// THIS MUST ONLY BE RUN ONCE PER UPDATE CYCLE
        /// </summary>
        public static void Update(float dt)
        {
            _supressXna = false;
            lkb = kb; //get last kb state
            kb = Keyboard.GetState(); //get current state of kb

            if (IsCharKeyboardEnabled)
                XnaKeyToChar();
            
            for (int i = TimedKeys.Count - 1; i >= 0; i--) //check all timed keys
            {
                TimedKeys[i].Life += dt; //add on delta time
                if (TimedKeys[i].Life >= TimedKey.MaxKeyLife) //check if key is to old
                    TimedKeys.RemoveAt(i); //remove if old
            }

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }
        /// <summary>
        /// check if the given key is drpessed
        /// </summary>
        /// <param name="key">the key to check</param>
        /// <returns></returns>
        public static bool IsKeyDown(Keys key)
        {
            if (_supressXna)
                return false;
            return kb.IsKeyDown(key);
        }

        /// <summary>
        /// check if the given key is drpessed
        /// </summary>
        /// <param name="key">the key to check</param>
        /// <returns></returns>
        public static bool isKeyUp(Keys key)
        {
            if (_supressXna)
                return false;
            return kb.IsKeyUp(key);
        }

        /// <summary>
        /// Check if a key has been pressed
        /// will only return true on the initalze state change
        /// </summary>
        /// <param name="key">the key to check </param>
        /// <returns></returns>
        public static bool IsKeyPressed(Keys key)
        {
            if (_supressXna)
                return false;

            if (kb.IsKeyDown(key) && lkb.IsKeyUp(key))
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Check if a key has been pressed
        /// will only return true on the initalze state change
        /// </summary>
        /// <param name="key">the key to check </param>
        /// <returns></returns>
        public static bool isKeyDoubblePressed(Keys key)
        {
            if (_supressXna)
                return false;

            if (kb.IsKeyDown(key) && lkb.IsKeyUp(key))
            {
                for (int i = 0; i < TimedKeys.Count; i++) //loop through all timed keys
                    if (TimedKeys[i].Key == key) //look for if the key has alread been pressed in
                    {
                        //maximume time frame
                        return true; //if it has we have a double press
                    }

                TimedKeys.Add(new TimedKey(key)); //if not add the key to the timed keys
                return false;
            }
            else
                return false;
        }
    }
}
