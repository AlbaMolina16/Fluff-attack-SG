using FluffGameApi.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluffGameApi.Entities
{
    /// <summary>
    /// scores db table
    /// </summary>
    [Table("scores")]
    public class Score : BaseEntity
    {
        public int IdUser { get; set; }
        public int IdDifficulty { get; set; }
        public int TotalPoints { get; set; }
        public int RedPoints { get; set; }
        public int BluePoints { get; set; }
        public int YellowPoints { get; set; }
        public int GreenPoints { get; set; }
    }
}