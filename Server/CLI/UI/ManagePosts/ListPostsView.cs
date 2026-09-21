using Entity;
using RepositorContracts;

namespace CLI.UI.ManagePosts;

public class ListPostsView
{
    private readonly IPostRepository postRepository;

    public ListPostsView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public Task ShowAsync()
    {
        Console.WriteLine("\n Posts Overview ");
        Console.Write("Filter by ID (leave empty to view all): ");
        string? input = Console.ReadLine();

        IQueryable<Post> query = postRepository.GetMany();

        if (input is not null && int.TryParse(input.Trim(), out int authorId))
        {
            query = query.Where(p => p.UserId == authorId);
        }

        List<Post> posts = query.ToList();

        if (!posts.Any())
        {
            Console.WriteLine("No posts found.");
            return Task.CompletedTask;
        }

        Console.WriteLine("\n Title");
        foreach (Post post in posts)
        {
            Console.WriteLine($"{post.Id}\t| {post.Title}");
        }

        return Task.CompletedTask;
    }
}