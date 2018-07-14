using CustomAttributeExample.CustomAttributes;
using System;

namespace CustomAttributeExample.Models
{
    public class Sportsman
    {
        [IsUpdatable]
        public string FirstName { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        [IsUpdatable]
        public string Sport { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
