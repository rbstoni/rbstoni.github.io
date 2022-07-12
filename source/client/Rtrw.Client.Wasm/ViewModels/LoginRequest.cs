using System.ComponentModel.DataAnnotations;

namespace Rtrw.Client.Wasm.ViewModels
{
    public class LoginRequest
    {
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
