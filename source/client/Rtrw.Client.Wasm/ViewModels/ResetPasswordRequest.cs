﻿using System.ComponentModel.DataAnnotations;

namespace Rtrw.Client.Wasm.ViewModels
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
