using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Areas.Admin.Models
{
    public class UserEditVm
    {
        public string? Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }

        // Используется при создании или смене пароля
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        // Блокировка пользователя (опционально)
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }

        // Роли пользователя
        public IList<string> Roles { get; set; } = new List<string>();
        public List<string> SelectedRoles { get; set; } = new();         // выбранные роли во вьюхе
        public List<SelectListItem> AllRoles { get; set; } = new();       // все роли для выпадающего списка/мультиселекта
    }
}
