using FluffGameApi.Entities.Base;

namespace FluffGameApi.Entities
{
    /// <summary>
    /// users db table
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// Username of the user, must be unique
        /// </summary>
        public required string Username { get; set; }
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