using FluffGameApi.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace FluffGameApi.Entities
{
    /// <summary>
    /// user_preferences db table, stores the user's preferred difficulty level for the game
    /// </summary>
    [Table("user_preferences")]
    public class UserPreferences : BaseEntity
    {
        public required int IdUser { get; set; }
        public required User User { get; set; }
        public required int IdDifficulty { get; set; }
        public required Difficulty Difficulty { get; set; }
    }
}