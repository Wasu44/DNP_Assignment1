using Entity;
using RepositorContracts;

namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{
    private readonly List<Comment> comments = new();
    
    public CommentInMemoryRepository()
    {
        comments.Add(new Comment
        {
            Id = 1,
            PostId = 1,
            UserId = 2,
            Body = "Great summary, thanks for sharing!"
        });
        comments.Add(new Comment
        {
            Id = 2,
            PostId = 1,
            UserId = 3,
            Body = "Totally agree, C# 8+ features make syntax so much cleaner."
        });
        comments.Add(new Comment
        {
            Id = 3,
            PostId = 2,
            UserId = 4,
            Body = "It also makes swapping in-memory mocks for real SQL DB seamless."
        });
        comments.Add(new Comment
        {
            Id = 4,
            PostId = 3,
            UserId = 1,
            Body = "Rider's smart step into is unmatched."
        });
        comments.Add(new Comment
        {
            Id = 5,
            PostId = 4,
            UserId = 3,
            Body = "Very good point about Task vs void."
        });
    }

    public Task<Comment> AddAsync(Comment comment)
    {
        comment.Id = comments.Any()
            ? comments.Max(c => c.Id) + 1
            : 1;
        comments.Add(comment);
        return Task.FromResult(comment);
    }

    public Task UpdateAsync(Comment comment)
    {
        Comment? existingComment = comments.SingleOrDefault(c => c.Id == comment.Id);
        if (existingComment is null)
        {
            throw new InvalidOperationException($"Comment with ID '{comment.Id}' not found");
        }

        comments.Remove(existingComment);
        comments.Add(comment);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Comment? commentToRemove = comments.SingleOrDefault(c => c.Id == id);
        if (commentToRemove is null)
        {
            throw new InvalidOperationException($"Comment with ID '{id}' not found");
        }

        comments.Remove(commentToRemove);
        return Task.CompletedTask;
    }

    public Task<Comment> GetSingleAsync(int id)
    {
        Comment? comment = comments.SingleOrDefault(c => c.Id == id);
        if (comment is null)
        {
            throw new InvalidOperationException($"Comment with ID '{id}' not found");
        }

        return Task.FromResult(comment);
    }

    public IQueryable<Comment> GetMany()
    {
        return comments.AsQueryable();
    }
}