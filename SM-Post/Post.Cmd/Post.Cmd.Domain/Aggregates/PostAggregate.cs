using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates
{
    public class PostAggregate : AggregateRoot
    {
        private bool _isActive;
        private string _author;
        private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }
        public PostAggregate()
        {
            _isActive = true;
        }

        public PostAggregate(Guid id, string author, string message)
        {
            RaiseEvent(new PostCreatedEvent
            {
                Id = id,
                Author = author,
                Message = message,
                DatePosted = DateTime.Now
            });

        }
        public void Apply(PostCreatedEvent @event)
        {
           _id = @event.Id;
           _isActive = true;
           _author = @event.Author ?? string.Empty;
        }
        public void EditMessage(string message)
        {
            if (!_isActive)
            {
                throw new InvalidOperationException("Cannot edit message on an inactive post");
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new InvalidOperationException($"The value of {nameof(message)} cannot be empty.Please provide a valid {nameof(message)} ");
            }
            RaiseEvent(new MessageUpdatedEvent
            {
                Id = _id,
                Message = message
            });
        }
        public void Apply(MessageUpdatedEvent @event)
        {
            _id = @event.Id;
        }
        public void LikePost()
        {
            if (!_isActive)
            {
                throw new InvalidOperationException("Cannot like a post that is not active");
            }
            RaiseEvent(new PostLikedEvent
            {
                Id = _id
            });
        }
        public void Apply(PostLikedEvent @event)
        {
            _id = @event.Id;
        }
        public void AddComment(string comment, string username)
        {
            if (!_isActive)
            {
                throw new InvalidOperationException("Cannot add comment to a post that is not active");
            }
            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new InvalidOperationException($"The value of {nameof(comment)} cannot be empty.Please provide a valid {nameof(comment)} ");
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidOperationException($"The value of {nameof(username)} cannot be empty.Please provide a valid {nameof(username)} ");
            }
            RaiseEvent(new CommentAddedEvent
            {
                Id = _id,
                CommentId = Guid.NewGuid(),
                Comment = comment,
                Username = username,
                CommentDate = DateTime.UtcNow
            });
        }
        public void Apply(CommentAddedEvent @event)
        {
            _id = @event.Id;
            _comments.Add(@event.CommentId, new Tuple<string, string>(@event?.Comment ?? string.Empty, @event?.Username ?? string.Empty));
        }

        public void EditComment(Guid commentId, string comment, string username)
        {
            if (!_isActive)
            {
                throw new InvalidOperationException("Cannot edit comment to a post that is not active");
            }
            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new InvalidOperationException($"The value of {nameof(comment)} cannot be empty.Please provide a valid {nameof(comment)} ");
            }
            if(!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase ))
            {
                throw new InvalidOperationException("You are not allowed to edit a comment that was made by someone else");
            }
            RaiseEvent(new CommentUpdatedEvent
            {
                Id = _id,
                CommentId = commentId,
                Comment = comment,
                Username = username,
                EditDate = DateTime.UtcNow
            });
        }
        public void Apply(CommentUpdatedEvent @event)
        {
            _id = @event.Id;
            _comments[@event.CommentId] = new Tuple<string, string>(@event?.Comment ?? string.Empty, @event?.Username ?? string.Empty);
        }
        public void RemoveComment(Guid commentId, string username)
        {
            if (!_isActive)
            {
                throw new InvalidOperationException("Cannot remove comment to a post that is not active");
            }
            if(!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase ))
            {
                throw new InvalidOperationException("You are not allowed to remove a comment that was made by someone else");
            }
            RaiseEvent(new CommentRemovedEvent
            {
                Id = _id,
                CommentId = commentId
            });
        }
        public void Apply(CommentRemovedEvent @event)
        {
            _id = @event.Id;
            _comments.Remove(@event.CommentId);
        }
        public void DeletePost(string username)
        {
            if (!_isActive)
            {
                throw new InvalidOperationException("The post has already been deleted!");
            }
            if (!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException("You are not allowed to delete a post that was made by someone else");
            }
            RaiseEvent(new PostRemovedEvent
            {
                Id = _id
            });
        }
        public void Apply(PostRemovedEvent @event)
        {
            _id = @event.Id;
            _isActive = false;
        }
        public string Author
        {
            get => _author;
            set => _author = value;
        }
    }
}