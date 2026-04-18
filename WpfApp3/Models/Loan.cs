using System;
using System.Collections.Generic;

namespace WpfApp3.Models;

public partial class Loan
{
    public int LoanId { get; set; }

    public int BookId { get; set; }

    public int MemberId { get; set; }

    public DateOnly? BorrowDate { get; set; }

    public DateOnly DueDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public string? Status { get; set; }

    public decimal? FineAmount { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Member Member { get; set; } = null!;
}
