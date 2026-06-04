using FluffGameApi.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluffGameApi.Entities
{
    /// <summary>
    /// users db table
    /// </summary>
    [Table("users")]
    public class User : BaseEntity
    {
        /// <summary>
        /// Username of the user, must be unique
        /// </summary>
        public required string Username { get; set; }
        /// <summary>
        /// First name of the user, can be null if not provided
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Last name of the user, can be null if not provided
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Age of the user
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// Handedness of the user, can be "diestro", "zurdo"
        /// </summary>
        public required string Handedness { get; set; }

        /// <summary>
        /// Password hash of the user, must be generated using a secure hashing algorithm and should not be stored in plain text
        /// </summary>
        public required string PasswordHash { get; set; }
        /// <summary>
        /// Date of user creation, should be set to the current date and time when a new user is created
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}