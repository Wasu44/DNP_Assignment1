using Entity;
using RepositorContracts;

namespace CLI.UI.ManagePosts;

public class SinglePostView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;

    public SinglePostView(
        IPostRepository postRepository,
        ICommentRepository commentRepository,
        IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        int postId;
        while (true)
        {
            Console.Write("\nEnter post ID to view: ");
            string? input = Console.ReadLine();

            if (input is not null && int.TryParse(input.Trim(), out postId))
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid ID. Please enter an integer.");
            }
        }

        try
        {
            Post post = await postRepository.GetSingleAsync(postId);

            string authorName = $"User #{post.UserId}";
            try
            {
                User author = await userRepository.GetSingleAsync(post.UserId);
                authorName = author.UserName;
            }
            catch (InvalidOperationException) { }

            Console.WriteLine($"Title:   {post.Title}");
            Console.WriteLine($"Author:  {authorName} (ID: {post.UserId})");
            Console.WriteLine($"Post ID: {post.Id}");
            Console.WriteLine("\n " + post.Body);

            List<Comment> comments = commentRepository.GetMany()
                .Where(c => c.PostId == post.Id)
                .ToList();

            Console.WriteLine($"Comments ({comments.Count}):");
            if (!comments.Any())
            {
                Console.WriteLine("  (No comments yet)");
            }
            else
            {
                foreach (Comment comment in comments)
                {
                    Console.WriteLine($"  [{comment.Id}] (Author ID: {comment.UserId}): {comment.Body}");
                }
            }

            Console.WriteLine("\nOptions: [1] Add Comment | [0] Back");
            Console.Write("Choice: ");
            string? optionInput = Console.ReadLine();

            if (optionInput is not null && optionInput.Trim() == "1")
            {
                await AddCommentAsync(post.Id);
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private async Task AddCommentAsync(int postId)
    {
        int userId;
        while (true)
        {
            Console.Write("Enter your User ID: ");
            string? input = Console.ReadLine();

            if (input is not null && int.TryParse(input.Trim(), out userId))
            {
                try
                {
                    await userRepository.GetSingleAsync(userId);
                    break;
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine($"User with ID {userId} does not exist. Enter a valid user ID.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format. Try again.");
            }
        }

        string body = string.Empty;
        while (true)
        {
            Console.Write("Enter comment body: ");
            string? input = Console.ReadLine();

            if (input is not null && !string.IsNullOrWhiteSpace(input))
            {
                body = input.Trim();
                break;
            }
            else
            {
                Console.WriteLine("Comment body cannot be empty. Try again.");
            }
        }

        Comment comment = new Comment
        {
            PostId = postId,
            UserId = userId,
            Body = body
        };

        Comment created = await commentRepository.AddAsync(comment);
        Console.WriteLine($"Comment added successfully with ID: {created.Id}");
    }
}