using Entity;
using RepositorContracts;

namespace InMemoryRepositories;


public class PostInMemoryRepository : IPostRepository
{
    private readonly List<Post> posts = new();
    
    public PostInMemoryRepository()
    {
        posts.Add(new Post
        {
            Id = 1,
            Title = "Getting Started with C# and .NET 8",
            Body = "C# provides great features for building robust console apps and web backends.",
            UserId = 1
        });
        posts.Add(new Post
        {
            Id = 2,
            Title = "Why Repository Pattern Matters",
            Body = "Separating data access logic from UI makes your code testable and easy to maintain.",
            UserId = 2
        });
        posts.Add(new Post
        {
            Id = 3,
            Title = "Debugging in Rider vs VS Code",
            Body = "What are your favorite keyboard shortcuts when debugging asynchronous code?",
            UserId = 4
        });
        posts.Add(new Post
        {
            Id = 4,
            Title = "Async and Await Best Practices",
            Body = "Always avoid async void unless you are writing event handlers!",
            UserId = 1
        });
    }
    
    public Task<Post> AddAsync(Post post)
    {
        post.Id = posts.Any()
            ? posts.Max(p => p.Id) + 1
            : 1;
        posts.Add(post);
        return Task.FromResult(post);
    }

    public Task UpdateAsync(Post post)
    {
        Post? existingPost = posts.SingleOrDefault(p => p.Id == post.Id);
        if (existingPost is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{post.Id}' not found");
        }

        posts.Remove(existingPost);
        posts.Add(post);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Post? postToRemove = posts.SingleOrDefault(p => p.Id == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{id}' not found");
        }

        posts.Remove(postToRemove);
        return Task.CompletedTask;
    }

    public Task<Post> GetSingleAsync(int id)
    {
        Post? post = posts.SingleOrDefault(p => p.Id == id);
        if (post is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{id}' not found");
        }
        return Task.FromResult(post);
    }

    public IQueryable<Post> GetMany()
    {
        return posts.AsQueryable();
    }
}