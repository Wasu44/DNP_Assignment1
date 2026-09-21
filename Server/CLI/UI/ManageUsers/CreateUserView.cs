using Entity;
using RepositorContracts;

namespace CLI.UI.ManageUsers;

public class CreateUserView
{
    private readonly IUserRepository userRepository;

    public CreateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }
    public async Task ShowAsync()
    {

        Console.WriteLine("Create new user");
        
        string userName = string.Empty;
        while (true)
        {
            Console.Write("Enter username: ");
            string? input = Console.ReadLine();

            if (input is not null && !string.IsNullOrWhiteSpace(input))
            {
                userName = input.Trim();
                break;
            }
            else
            {
                Console.WriteLine("Input is empty");
            }
        }
        
        string password = string.Empty;
        while (true)
        {
            Console.Write("Enter password: ");
            string? input = Console.ReadLine();

            if (input is not null && !string.IsNullOrWhiteSpace(input))
            {
                password = input.Trim();
                break;
            }
            else
            {
                Console.WriteLine("Input is empty");
            }
        }

        User newUser = new User
        {
            UserName = userName,
            Password = password
        };
    }
}