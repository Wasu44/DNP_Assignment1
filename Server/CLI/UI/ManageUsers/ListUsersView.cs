using Entity;
using RepositorContracts;

namespace CLI.UI.ManageUsers;

public class ListUsersView
{
    private readonly IUserRepository userRepository;

    public ListUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public Task ShowAsync()
    {
        Console.WriteLine("List Users");
        Console.Write("Enter username filter (leave empty to view all): ");
        string? input = Console.ReadLine();

        IQueryable<User> query = userRepository.GetMany();

        if (input is not null && !string.IsNullOrWhiteSpace(input))
        {
            string filter = input.Trim();
            query = query.Where(u => u.UserName.Contains(filter, StringComparison.OrdinalIgnoreCase));
        }

        List<User> users = query.ToList();

        if (!users.Any())
        {
            Console.WriteLine("No users found.");
        }
        

        return Task.CompletedTask;
    }
}