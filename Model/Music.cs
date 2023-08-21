using System.Text.Json.Serialization;

namespace LyristAPI.Model
{
    public class Music
    {
        [JsonPropertyName("lyrics")]
        public string? Lyrics { get; set; }
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        [JsonPropertyName("artist")]
        public string? Artist { get; set; }
        [JsonPropertyName("image")]
        public string? Image { get; set; }
    }


}
