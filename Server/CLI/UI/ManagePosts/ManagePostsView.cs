using Entity;
using RepositorContracts;

namespace CLI.UI.ManagePosts;

public class ManagePostsView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;

    private readonly CreatePostView createPostView;
    private readonly ListPostsView listPostsView;
    private readonly SinglePostView singlePostView;

    public ManagePostsView(
        IPostRepository postRepository,
        ICommentRepository commentRepository,
        IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;

        createPostView = new CreatePostView(postRepository, userRepository);
        listPostsView = new ListPostsView(postRepository);
        singlePostView = new SinglePostView(postRepository, commentRepository, userRepository);
    }

    public async Task ShowAsync()
    {
        while (true)
        {
            Console.WriteLine("\n=== Manage Posts ===");
            Console.WriteLine("1. Create new post");
            Console.WriteLine("2. Overview posts [ID, Title]");
            Console.WriteLine("3. View specific post (and comments)");
            Console.WriteLine("4. Update post");
            Console.WriteLine("5. Delete post");
            Console.WriteLine("0. Back to Main Menu");
            Console.Write("Select an option: ");

            string? input = Console.ReadLine();
            string choice = string.Empty;

            if (input is not null && !string.IsNullOrWhiteSpace(input))
            {
                choice = input.Trim();
            }
            else
            {
                Console.WriteLine("Input is null or empty. Please enter an option.");
                continue;
            }

            switch (choice)
            {
                case "1":
                    await createPostView.ShowAsync();
                    break;
                case "2":
                    await listPostsView.ShowAsync();
                    break;
                case "3":
                    await singlePostView.ShowAsync();
                    break;
                case "4":
                    await UpdatePostAsync();
                    break;
                case "5":
                    await DeletePostAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid selection, please try again.");
                    break;
            }
        }
    }

    private async Task UpdatePostAsync()
    {
        int id;
        while (true)
        {
            Console.Write("Enter post ID to update: ");
            string? input = Console.ReadLine();

            if (input is not null && int.TryParse(input.Trim(), out id))
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid number format. Please enter an integer ID.");
            }
        }

        try
        {
            Post post = await postRepository.GetSingleAsync(id);

            Console.Write($"Enter new title (current: {post.Title}, leave empty to keep): ");
            string? titleInput = Console.ReadLine();
            if (titleInput is not null && !string.IsNullOrWhiteSpace(titleInput))
            {
                post.Title = titleInput.Trim();
            }

            Console.Write("Enter new body (leave empty to keep current): ");
            string? bodyInput = Console.ReadLine();
            if (bodyInput is not null && !string.IsNullOrWhiteSpace(bodyInput))
            {
                post.Body = bodyInput.Trim();
            }

            await postRepository.UpdateAsync(post);
            Console.WriteLine("Post updated successfully.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private async Task DeletePostAsync()
    {
        int id;
        while (true)
        {
            Console.Write("Enter post ID to delete: ");
            string? input = Console.ReadLine();

            if (input is not null && int.TryParse(input.Trim(), out id))
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid number format. Please enter an integer ID.");
            }
        }

        try
        {
            await postRepository.DeleteAsync(id);

            List<Comment> postComments = commentRepository.GetMany()
                .Where(c => c.PostId == id)
                .ToList();

            foreach (Comment comment in postComments)
            {
                await commentRepository.DeleteAsync(comment.Id);
            }

            Console.WriteLine($"Post with ID {id} and its comments were deleted.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}