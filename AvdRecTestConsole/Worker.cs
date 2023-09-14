using Crud;
using System;

namespace GrpcService.Models
{
    public class Worker
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public long Birthday { get; set; }
        public Sex Sex { get; set; } = Sex.Default;
    }
}
