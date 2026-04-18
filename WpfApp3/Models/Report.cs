using System;
using System.Collections.Generic;

namespace WpfApp3.Models;

public partial class Report
{
    public int ReportId { get; set; }

    public string ReportType { get; set; } = null!;

    public string ReportTitle { get; set; } = null!;

    public DateOnly? ReportDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? Content { get; set; }

    public int? TotalBooks { get; set; }

    public int? TotalMembers { get; set; }

    public int? TotalLoans { get; set; }

    public int? OverdueLoans { get; set; }

    public decimal? TotalFine { get; set; }

    public string? Notes { get; set; }
}
