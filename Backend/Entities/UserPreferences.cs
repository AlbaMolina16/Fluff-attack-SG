using FluffGameApi.Entities.Base;

namespace FluffGameApi.Entities
{
    /// <summary>
    /// user_preferences db table, stores the user's preferred difficulty level for the game
    /// </summary>
    public class UserPreferences : BaseEntity
    {
        public int IdUser { get; set; }
        public int IdDifficulty { get; set; }
    }
}