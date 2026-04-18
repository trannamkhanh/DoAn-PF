using System;
using System.Collections.Generic;

namespace WpfApp3.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Role { get; set; }

    public bool? IsActive { get; set; }
}
