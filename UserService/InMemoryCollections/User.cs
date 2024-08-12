using UserService.DTO;

namespace UserService.InMemoryCollections
{
    public class Users : IUsers
    {
        private Dictionary<string, UserRegistration> users;
        public Users()
        {
            users = new Dictionary<string, UserRegistration>();
        }
        public void Add(UserRegistration registration)
        {
            if (!users.ContainsKey(registration.Email))
            {
                users.Add(registration.Email, registration);
            }
            else
            {
                users[registration.Email] = registration;
            }
        }
        public UserRegistration? Get(string email)
        {
            if (users.TryGetValue(email, out var registration))
            {
                return registration;
            }
            return null;
        }
        public bool Remove(string email)
        {
            if (users.ContainsKey(email))
            {
                return users.Remove(email);
            }
            return false;
        }
        public bool Update(UserRegistration registration)
        {
            if (users.ContainsKey(registration.Email))
            {
                users[registration.Email] = registration;
                return true;
            }
            return false;
        }
    }
}
