using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServarrAuthAPI.Database.Models
{
    [Table("Spotify")]
    public class SpotifyEntity
    {
        public int Id { get; set; }

        public Guid State { get; set; }

        public string Target { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}