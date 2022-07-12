using System.ComponentModel.DataAnnotations;

namespace Rtrw.Client.Wasm.ViewModels
{
    public class ForgotPasswordRequest
    {
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
    }
}
