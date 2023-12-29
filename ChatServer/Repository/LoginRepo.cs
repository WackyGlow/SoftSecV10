using Shared;

namespace ChatServer.Repository;

public class LoginRepo
{
    private static LoginRepo _instance;
    private static readonly object _lock = new object();

    private Dictionary<int, User> _users;

    private LoginRepo()
    {
        _users = new Dictionary<int, User>();
    }

    public static LoginRepo Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new LoginRepo();
                }
                return _instance;
            }
        }
    }

    public void AddOrUpdateUser(User user)
    {
        _users[user.Id] = user;
    }

    public User GetUser(int userId)
    {
        _users.TryGetValue(userId, out User user);
        return user;
    }

    public List<User> GetUsers()
    {
        return _users.Values.ToList();
    }

    public bool RemoveUser(int userId)
    {
        return _users.Remove(userId);
    }

}