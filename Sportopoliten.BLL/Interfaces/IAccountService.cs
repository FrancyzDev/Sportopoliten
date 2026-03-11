namespace Sportopoliten.BLL.Interfaces
{
    public interface IUserService<TDto> where TDto : class
    {
        Task Create(TDto dto);
        Task<TDto> Login(TDto dto);

        Task<bool> IsUnique(string login);

    }
}
