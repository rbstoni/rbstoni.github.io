namespace Rtrw.Client.Wasm.ViewModels
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
