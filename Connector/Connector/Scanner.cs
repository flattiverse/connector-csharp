using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// A scanner information.
    /// </summary>
    public class Scanner
    {
        /// <summary>
        /// The spanned width in degrees.
        /// </summary>
        public readonly float SpannedWidth;

        /// <summary>
        /// The direction the scanner is looking at.
        /// </summary>
        public readonly float Direction;

        internal Scanner(float spannedWidth, float direction)
        {
            SpannedWidth = spannedWidth;
            Direction = direction;
        }
    }
}
