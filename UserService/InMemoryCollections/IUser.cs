using UserService.DTO;

namespace UserService.InMemoryCollections
{
    public interface IUsers
    {
        void Add(UserRegistration registration);
        bool Remove(string email);
        bool Update(UserRegistration registration);
        UserRegistration? Get(string email);
    }
}