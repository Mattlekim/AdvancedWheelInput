using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riddlersoft.Core.Networking
{
    public struct NEX_Category
    {
        public int Code;
        public SortOrder SortBy;
    }

    /// <summary>
    /// a helpler class to convert a level name to a corisponding category
    /// </summary>
    public static class CategoryHelper
    {
        /// <summary>
        /// the list of levels and there corisponding category codes
        /// </summary>
        private static Dictionary<string, NEX_Category> _categoryCode = new Dictionary<string, NEX_Category>();

        /// <summary>
        /// add a new category to the game
        /// </summary>
        /// <param name="levelName">the level name</param>
        /// <param name="code">the category code</param>
        /// <returns></returns>
        public static bool AddCategory(string levelName, int code, SortOrder sortBy)
        {
            if (_categoryCode.ContainsKey(levelName))
                return false;

            _categoryCode.Add(levelName, new Networking.NEX_Category() { Code = code, SortBy = sortBy });

            return true;
        }

        /// <summary>
        /// returns the category code of the given level
        /// </summary>
        /// <param name="levelName">the level we want the category for</param>
        /// <returns>returns the correct category code for the nex rankings</returns>
        public static int GetGategoryNexCode(string levelName)
        {
            if (!_categoryCode.ContainsKey(levelName))
                return -1;
            return _categoryCode[levelName].Code;
        }

        public static NEX_Category? GetCategory(string levelName)
        {
            if (!_categoryCode.ContainsKey(levelName))
                return null;
            return _categoryCode[levelName];
        }

        
    }
}
