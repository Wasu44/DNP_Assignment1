using System.Text.Json;
using Entity;
using RepositorContracts;

namespace FileRepository;

public class CommentFileRepository : ICommentRepository
{
    private readonly string filePath = "comments.json";

    public CommentFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<Comment>> LoadAllAsync()
    {
        string json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Comment>>(json) ?? new List<Comment>();
    }

    private async Task SaveAllAsync(List<Comment> comments)
    {
        string json = JsonSerializer.Serialize(comments, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        List<Comment> comments = await LoadAllAsync();

        int maxId = comments.Count > 0 ? comments.Max(c => c.Id) : 0;
        comment.Id = maxId + 1;

        comments.Add(comment);
        await SaveAllAsync(comments);

        return comment;
    }

    public async Task UpdateAsync(Comment comment)
    {
        List<Comment> comments = await LoadAllAsync();

        Comment? existing = comments.FirstOrDefault(c => c.Id == comment.Id);
        if (existing is null)
        {
            throw new InvalidOperationException($"Comment with ID '{comment.Id}' not found");
        }

        comments.Remove(existing);
        comments.Add(comment);

        await SaveAllAsync(comments);
    }

    public async Task DeleteAsync(int id)
    {
        List<Comment> comments = await LoadAllAsync();

        Comment? toDelete = comments.FirstOrDefault(c => c.Id == id);
        if (toDelete is null)
        {
            throw new InvalidOperationException($"Comment with ID '{id}' not found");
        }

        comments.Remove(toDelete);
        await SaveAllAsync(comments);
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        List<Comment> comments = await LoadAllAsync();
        Comment? comment = comments.FirstOrDefault(c => c.Id == id);
        if (comment is null)
        {
            throw new InvalidOperationException($"Comment with ID '{id}' not found");
        }

        return comment;
    }

    public IQueryable<Comment> GetMany()
    {
        string json = File.ReadAllTextAsync(filePath).Result;
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(json) ?? new List<Comment>();
        return comments.AsQueryable();
    }
}