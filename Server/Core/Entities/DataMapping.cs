using System;
using System.ComponentModel.DataAnnotations;

namespace Replication.RooTool.Core.Entities
{
    public class DataMapping
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
    }
}
