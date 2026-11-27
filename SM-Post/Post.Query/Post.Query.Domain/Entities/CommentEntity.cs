using System.ComponentModel.DataAnnotations.Schema;

namespace Post.Query.Domain.Entities
{
    [Table("Comments")]
    public class CommentEntity
    {
        public Guid CommentId { get; set; }

        public string? Username { get; set; }
        public string? Comment { get; set; }
        public DateTime CommentDate { get; set; }
        public bool IsEdited { get; set; }
        public Guid PostId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual PostEntity Post { get; set; }

    }
}