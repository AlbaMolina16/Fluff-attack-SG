using FluffGameApi.Entities.Base;

namespace FluffGameApi.Entities
{
    /// <summary>
    /// scores db table
    /// </summary>
    public class Score : BaseEntity
    {
        public int IdUser { get; set; }
        public int IdDifficulty { get; set; }
        public int Total { get; set; }
        public int Red { get; set; }
        public int Blue { get; set; }
        public int Yellow { get; set; }
        public int Green { get; set; }
    }
}