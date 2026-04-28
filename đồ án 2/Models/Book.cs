using System;
using System.Collections.Generic;

namespace WpfApp3.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string Isbn { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string? Publisher { get; set; }

    public int? PublishYear { get; set; }

    public string? Genre { get; set; }

    public int Quantity { get; set; }

    public int AvailableQuantity { get; set; }

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
