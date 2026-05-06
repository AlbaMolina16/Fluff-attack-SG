using FluffGameApi.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluffGameApi.Entities
{
    /// <summary>
    /// scores db table
    /// </summary>
    [Table("difficulties")]
    public class Difficulty : BaseEntity
    {
        public required string Name { get; set; }
    }
}