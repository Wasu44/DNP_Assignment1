using Entity;
using RepositorContracts;

namespace CLI.UI.ManageUsers;

public class ManageUsersView
{
    private readonly IUserRepository userRepository;
    private readonly CreateUserView createUserView;
    private readonly ListUsersView listUsersView;

    public ManageUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
        createUserView = new CreateUserView(userRepository);
        listUsersView = new ListUsersView(userRepository);
    }

    public async Task ShowAsync()
    {
        while (true)
        {
            Console.WriteLine("\n Manage Users ");
            Console.WriteLine("1. Create new user");
            Console.WriteLine("2. List users (with search)");
            Console.WriteLine("3. Update user");
            Console.WriteLine("4. Delete user");
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
                Console.WriteLine("Input is null or empty. Please select a valid option.");
                continue;
            }

            switch (choice)
            {
                case "1":
                    await createUserView.ShowAsync();
                    break;
                case "2":
                    await listUsersView.ShowAsync();
                    break;
                case "3":
                    await UpdateUserAsync();
                    break;
                case "4":
                    await DeleteUserAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid selection, please try again.");
                    break;
            }
        }
    }

    private async Task UpdateUserAsync()
    {
        int id;
        while (true)
        {
            Console.Write("Enter user ID to update: ");
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
            User existing = await userRepository.GetSingleAsync(id);

            Console.Write($"Enter new username (current: {existing.UserName}, leave empty to keep): ");
            string? nameInput = Console.ReadLine();
            if (nameInput is not null && !string.IsNullOrWhiteSpace(nameInput))
            {
                existing.UserName = nameInput.Trim();
            }

            Console.Write("Enter new password (leave empty to keep current): ");
            string? passInput = Console.ReadLine();
            if (passInput is not null && !string.IsNullOrWhiteSpace(passInput))
            {
                existing.Password = passInput.Trim();
            }

            await userRepository.UpdateAsync(existing);
            Console.WriteLine("User updated successfully.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private async Task DeleteUserAsync()
    {
        int id;
        while (true)
        {
            Console.Write("Enter user ID to delete: ");
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
            await userRepository.DeleteAsync(id);
            Console.WriteLine($"User with ID {id} deleted successfully.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}