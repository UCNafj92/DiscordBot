using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBirthdayApp.Model
{
    /// <summary>
    /// Represents a Discord user with an associated birthday.
    /// Equality and hashing are based on the unique <see cref="UserId"/>.
    /// </summary>
    public class Member
    {
        /// <summary>
        /// Gets or sets the unique Discord user ID.
        /// </summary>
        public ulong UserId { get; set; }

        /// <summary>
        /// Gets or sets the user's birthday in format "DD-MM".
        /// </summary>
        public string Birthday { get; set; }


        public override bool Equals(object obj)
        {
            if (obj is Member other)
            {
                return this.UserId == other.UserId;
            }
            return false;
        }


        public override int GetHashCode()
        {
            // Use UserId for hashing since it's unique
            return UserId.GetHashCode();
        }


        public static bool operator ==(Member left, Member right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }


        public static bool operator !=(Member left, Member right)
        {
            return !(left == right);
        }
    }
}
