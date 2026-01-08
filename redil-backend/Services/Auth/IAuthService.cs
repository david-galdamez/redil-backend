namespace redil_backend.Services.Auth
{
    public interface IAuthService<T, Tr, Tl>
    {
        Task<T> Register(Tr authRegisterDto);
        Task<T?> Login(Tl authLoginDto);
        void LogOut();
    }
}
