using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories
{
    public class CommentRepository: ICommentRepository
    {
        private readonly DatabaseContextFactory _ContextFactory;
        
        public CommentRepository(DatabaseContextFactory databaseContextFactory)
        {
            _ContextFactory = databaseContextFactory;
        }
        public async Task CreateAsync(CommentEntity comment)
        {
            using DatabaseContext context = _ContextFactory.CreateDbContext();
            context.Comments.Add(comment);
            _ =await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CommentEntity comment)
        {
            using DatabaseContext context = _ContextFactory.CreateDbContext();
            var existingComment = await context.Comments.FindAsync(comment.CommentId);
            if (existingComment == null)
            {
                throw new KeyNotFoundException($"Comment with ID {comment.CommentId} not found");
            }
            context.Comments.Update(comment);
            _ =await context.SaveChangesAsync();
        }

        public async Task<CommentEntity?> GetByIdAsync(Guid commentId)
        {
            using DatabaseContext context = _ContextFactory.CreateDbContext();
            return await context.Comments.FindAsync(commentId);
        }

        public async Task DeleteAsync(Guid commentId)
        {
            using DatabaseContext context = _ContextFactory.CreateDbContext();
            var comment = await context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return;
            }
            context.Comments.Remove(comment);
            _ =await context.SaveChangesAsync();
        }
    }
}