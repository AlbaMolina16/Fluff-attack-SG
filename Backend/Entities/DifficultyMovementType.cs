using FluffGameApi.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluffGameApi.Entities
{
    /// <summary>
    /// difficulty_movementType db table (relaciona la dificultad con el tipo de movimiento que puede simular una pelusa)
    /// </summary>
    [Table("difficulty_movementType")]
    public class DifficultyMovementTypes : BaseEntity
    {
        public required int IdDifficulty { get; set; }
        public required int IdMovementType{ get; set; }
        public decimal Probability { get; set; } = 0;
    }
}