using System;
using System.Collections.Generic;

namespace WpfApp3.Models;

public partial class Member
{
    public int MemberId { get; set; }

    public string MemberCode { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public DateOnly? JoinDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
