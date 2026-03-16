namespace Sportopoliten.BLL.DTO.User
{
    public class LoginUserDTO
    {
        public string LoginOrEmail { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
