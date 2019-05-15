using Microsoft.AspNetCore.Identity;
using AdvDictionaryServer.Models;
using Moq;

namespace AdvDictionaryServerTests
{
    class UserManagerMock : UserManager<User>
    {
        public UserManagerMock() : base (
            new Mock<IUserStore<User>>()
        )
        {
            
        }
    }
}