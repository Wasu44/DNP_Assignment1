using System.Text.Json;
using Entity;
using RepositorContracts;

namespace FileRepository;

public class PostFileRepository : IPostRepository
{
    private readonly string filePath = "posts.json";

    public PostFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<Post>> LoadAllAsync()
    {
        string json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Post>>(json) ?? new List<Post>();
    }

    private async Task SaveAllAsync(List<Post> posts)
    {
        string json = JsonSerializer.Serialize(posts, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<Post> AddAsync(Post post)
    {
        List<Post> posts = await LoadAllAsync();

        int maxId = posts.Count > 0 ? posts.Max(p => p.Id) : 0;
        post.Id = maxId + 1;

        posts.Add(post);
        await SaveAllAsync(posts);

        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        List<Post> posts = await LoadAllAsync();

        Post? existing = posts.FirstOrDefault(p => p.Id == post.Id);
        if (existing is null)
        {
            throw new InvalidOperationException($"Post with ID '{post.Id}' not found");
        }

        posts.Remove(existing);
        posts.Add(post);

        await SaveAllAsync(posts);
    }

    public async Task DeleteAsync(int id)
    {
        List<Post> posts = await LoadAllAsync();

        Post? toDelete = posts.FirstOrDefault(p => p.Id == id);
        if (toDelete is null)
        {
            throw new InvalidOperationException($"Post with ID '{id}' not found");
        }

        posts.Remove(toDelete);
        await SaveAllAsync(posts);
    }

    public async Task<Post> GetSingleAsync(int id)
    {
        List<Post> posts = await LoadAllAsync();
        Post? post = posts.FirstOrDefault(p => p.Id == id);
        if (post is null)
        {
            throw new InvalidOperationException($"Post with ID '{id}' not found");
        }

        return post;
    }

    public IQueryable<Post> GetMany()
    {
        string json = File.ReadAllTextAsync(filePath).Result;
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(json) ?? new List<Post>();
        return posts.AsQueryable();
    }
}