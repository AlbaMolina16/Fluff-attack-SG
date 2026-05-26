using FluffGameApi.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluffGameApi.Entities
{
    /// <summary>
    /// movement_type db table
    /// </summary>
    [Table("movement_type")]
    public class MovementTypes : BaseEntity
    {
        public required string Name { get; set; }
    }
}