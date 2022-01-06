using APICore.Models.RequestDTO;
using APICore.Models.ResponseDTO;

namespace APICore.Repositories.Interfaces
{
    public interface IUserRepository
    {
        AuthResult Registration(UserAddDTO userDTO);
        AuthResult UserLogin(UserLoginDTO loginDTO);
    }
}