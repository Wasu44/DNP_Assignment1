using Entity;
using RepositorContracts;

namespace CLI.UI.ManagePosts;

public class CreatePostView
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public CreatePostView(IPostRepository postRepository, IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        Console.WriteLine("\nCreate New Post");

        int userId;
        while (true)
        {
            Console.Write("Enter author User ID: ");
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
                    Console.WriteLine($"Validation Error: User with ID {userId} does not exist.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID. Please enter a valid number.");
            }
        }

        string title = string.Empty;
        while (true)
        {
            Console.Write("Enter title: ");
            string? input = Console.ReadLine();

            if (input is not null && !string.IsNullOrWhiteSpace(input))
            {
                title = input.Trim();
                break;
            }
            else
            {
                Console.WriteLine("Title cannot be empty. Please try again.");
            }
        }

        string body = string.Empty;
        while (true)
        {
            Console.Write("Enter body: ");
            string? input = Console.ReadLine();

            if (input is not null && !string.IsNullOrWhiteSpace(input))
            {
                body = input.Trim();
                break;
            }
            else
            {
                Console.WriteLine("Body cannot be empty. Please try again.");
            }
        }

        Post post = new Post
        {
            Title = title,
            Body = body,
            UserId = userId
        };

        Post created = await postRepository.AddAsync(post);
        Console.WriteLine($"Post created successfully with ID: {created.Id}");
    }
}