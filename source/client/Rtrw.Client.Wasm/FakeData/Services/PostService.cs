using Microsoft.EntityFrameworkCore;
using Rtrw.Client.Wasm.FakeData.Database;
using Rtrw.Client.Wasm.Models;

namespace Rtrw.Client.Wasm.FakeData.Services
{
    public interface IPostService
    {
        Task<Post> GetPostByIdAsync(string postId);

        Task<List<Post>> GetPostsAsync();

        Task<Comment> GetCommentByIdAsync(string commentId);
    }

    public class PostService : IPostService
    {
        private readonly IApplicationDbContextFactory<SqliteDbContext> dbContextFactory;

        public PostService(IApplicationDbContextFactory<SqliteDbContext> dbContextFactory)
        { this.dbContextFactory = dbContextFactory; }

        public async Task<Comment> GetCommentByIdAsync(string commentId)
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();
            var storageComment = await dbContext.Comments
                .AsSplitQuery()
                .Include(
                    x
                        => x.Commenter)
                .ThenInclude(
                    y
                        => y.Location)
                .Include(
                    x
                        => x.Media)
                .Include(
                    x
                        => x.Mentions)
                .Include(
                    x
                        => x.Reactions)
                .Include(
                    x
                        => x.Replies)
                .Where(
                    x
                        => x.Id == commentId)
                .FirstOrDefaultAsync();
            if (storageComment == null)
            {
                return null!;
            }
            return storageComment;
        }

        public async Task<Post> GetPostByIdAsync(string postId)
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();
            var storagePost = dbContext.Posts
                .AsSplitQuery()
                .Include(
                    x
                        => x.Author)
                .ThenInclude(
                    p
                        => p.Location)
                .Include(
                    x
                        => x.PostLocation)
                .Include(
                    x
                        => x.Media)
                .Include(
                    x
                        => x.Comments)
                .Include(
                    x
                        => x.Reactions)
                .ThenInclude(
                    reaction
                        => reaction.Reactor)
                .Where(
                    x
                        => x.Id == postId)
                .FirstOrDefault();

            return storagePost;
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();
            var storagePosts = dbContext.Posts
                .AsSplitQuery()
                .Include(
                    x
                        => x.Author)
                .Include(
                    x
                        => x.Comments)
                .OrderBy(
                    x
                        => x.CreatedAt)
                .ToList();

            return storagePosts;
        }
    }
}
