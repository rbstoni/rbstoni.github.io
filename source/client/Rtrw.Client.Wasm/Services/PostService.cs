using Microsoft.EntityFrameworkCore;
using Rtrw.Client.Wasm.FakeData.Database;
using Rtrw.Client.Wasm.Models;
using Rtrw.Client.Wasm.FakeData;

namespace Rtrw.Client.Wasm.Services
{
    public interface IPostService
    {

        Task<Comment> GetCommentByIdAsync(string commentId);
        Task<Post> GetPostByIdAsync(string postId);
        Task<List<Post>> GetPostsAsync();
        Task SaveCommentAsync(Comment comment);
        Task<bool> SavePostAsync(Post post);
        Task<bool> SavePostAsync(IEnumerable<Post> post);

    }

    public class PostService : IPostService
    {

        private readonly ISqliteWasmDbContextFactory<SqliteWasmDbContext> dbContextFactory;
        private readonly Warga warga;

        public PostService(ISqliteWasmDbContextFactory<SqliteWasmDbContext> dbContextFactory, ICurrentUser currentUser)
        {
            this.dbContextFactory = dbContextFactory;
            warga = currentUser.Warga;
        }

        public async Task<Comment> GetCommentByIdAsync(string commentId)
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();
            var storageComment = await dbContext.Comments
                //.AsSplitQuery()
                //.Include(x => x.Commenter)
                //.ThenInclude(y => y.Geocoder)
                //.Include(x => x.Media)
                //.Include(x => x.Mentions)
                //.Include(x => x.Reactions)
                //.Include(x => x.Replies)
                .Where(x => x.Id == commentId)
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
                //.AsSplitQuery()
                //.Include(x => x.Author)
                //.ThenInclude(p => p.Geocoder)
                //.Include(x => x.PostGeocoder)
                //.Include(x => x.Media)
                //.Include(x => x.Comments)
                //.Include(x => x.Reactions)
                //.ThenInclude(reaction => reaction.Reactor)
                .Where(x => x.Id == postId)
                .FirstOrDefault();
            if (storagePost == null)
                return null!;
            return storagePost;
        }
        public async Task<List<Post>> GetPostsAsync()
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();
            var storagePosts = dbContext.Posts
                //.AsSplitQuery()
                //.Include(x => x.Author)
                //.Include(x => x.Comments)
                .OrderBy(x => x.CreatedAt)
                .ToList();
            if (storagePosts == null)
                return null!;
            return storagePosts;
        }

        public async Task SaveCommentAsync(Comment comment)
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();
            dbContext.Comments.Add(comment);
            await dbContext.SaveChangesAsync();
        }
        public async Task<bool> SavePostAsync(Post post)
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();
            try
            {
                var author = await dbContext.Warga.FirstOrDefaultAsync(x => x.Id == warga.Id);
                post.Author = author; 
                await dbContext.Posts!.AddAsync(post);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
        public async Task<bool> SavePostAsync(IEnumerable<Post> posts)
        {
            using (var dbContext = await dbContextFactory.CreateDbContextAsync())
            {
                try
                {
                    await dbContext.Posts.AddRangeAsync(posts);
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }

    }
}
