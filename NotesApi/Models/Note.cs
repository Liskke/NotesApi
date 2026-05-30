using System.Text.Json.Serialization;

namespace NotesApi.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Content { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }
    }
}