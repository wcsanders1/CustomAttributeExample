using CustomAttributeExample.CustomAttributes;
using System;

namespace CustomAttributeExample.Models
{
    public class Customer
    {
        public long Id { get; set; }
        
        [IsUpdatable]
        public string FirstName { get; set; }

        [IsUpdatable]
        public string LastName { get; set; }

        [IsUpdatable]
        public string Address { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool Deleted { get; set; }
    }
}
