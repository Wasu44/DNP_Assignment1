using System.Text.Json;
using Entity;
using RepositorContracts;

namespace FileRepository;

public class UserFileRepository : IUserRepository
{
    private readonly string filePath = "users.json";

    public UserFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<User>> LoadAllAsync()
    {
        string json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }

    private async Task SaveAllAsync(List<User> users)
    {
        string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<User> AddAsync(User user)
    {
        List<User> users = await LoadAllAsync();

        int maxId = users.Count > 0 ? users.Max(u => u.Id) : 0;
        user.Id = maxId + 1;

        users.Add(user);
        await SaveAllAsync(users);

        return user;
    }

    public async Task UpdateAsync(User user)
    {
        List<User> users = await LoadAllAsync();

        User? existing = users.FirstOrDefault(u => u.Id == user.Id);
        if (existing is null)
        {
            throw new InvalidOperationException($"User with ID '{user.Id}' not found");
        }

        users.Remove(existing);
        users.Add(user);

        await SaveAllAsync(users);
    }

    public async Task DeleteAsync(int id)
    {
        List<User> users = await LoadAllAsync();

        User? toDelete = users.FirstOrDefault(u => u.Id == id);
        if (toDelete is null)
        {
            throw new InvalidOperationException($"User with ID '{id}' not found");
        }

        users.Remove(toDelete);
        await SaveAllAsync(users);
    }

    public async Task<User> GetSingleAsync(int id)
    {
        List<User> users = await LoadAllAsync();
        User? user = users.FirstOrDefault(u => u.Id == id);
        if (user is null)
        {
            throw new InvalidOperationException($"User with ID '{id}' not found");
        }

        return user;
    }

    public IQueryable<User> GetMany()
    {
        string json = File.ReadAllTextAsync(filePath).Result;
        List<User> users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        return users.AsQueryable();
    }
}