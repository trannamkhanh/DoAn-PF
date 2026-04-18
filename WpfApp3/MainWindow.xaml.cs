using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp3.Models;

namespace WpfApp3
{
    public partial class MainWindow : Window
    {
        private readonly LibraryDbContext _db = new();

        public MainWindow()
        {
            InitializeComponent();
            LoadAllData();
            ResetReportForm();
        }

        private void NavigateToTab(int tabIndex)
        {
            MainTabControl.SelectedIndex = tabIndex;
        }

        private void SidebarHome_Click(object sender, RoutedEventArgs e)
        {
            NavigateToTab(0);
        }

        private void SidebarBooks_Click(object sender, RoutedEventArgs e)
        {
            NavigateToTab(1);
        }

        private void SidebarMembers_Click(object sender, RoutedEventArgs e)
        {
            NavigateToTab(2);
        }

        private void SidebarLoans_Click(object sender, RoutedEventArgs e)
        {
            NavigateToTab(3);
        }

        private void SidebarReports_Click(object sender, RoutedEventArgs e)
        {
            NavigateToTab(4);
        }

        private void LoadAllData()
        {
            LoadHomeOverview();
            LoadBooks();
            LoadMembers();
            LoadLoans();
            LoadReports();
            LoadReportSummary();
            BindLoanCombos();
            ResetLoanDefaults();
        }

        private void LoadBooks()
        {
            BooksGrid.ItemsSource = _db.Books.AsNoTracking().OrderBy(b => b.Title).ToList();
        }

        private void LoadHomeOverview()
        {
            TxtHomeTotalBooks.Text = (_db.Books.Sum(b => (int?)b.Quantity) ?? 0).ToString();
            TxtHomeTotalMembers.Text = _db.Members.Count().ToString();
            TxtHomeBorrowing.Text = _db.Loans.Count(l => l.ReturnDate == null && (l.Status == null || l.Status != "Returned")).ToString();
            TxtHomeReturned.Text = _db.Loans.Count(l => l.ReturnDate != null || l.Status == "Returned").ToString();

            HomeBooksGrid.ItemsSource = _db.Books
                .AsNoTracking()
                .OrderByDescending(b => b.BookId)
                .Take(8)
                .ToList();

            HomeMembersGrid.ItemsSource = _db.Members
                .AsNoTracking()
                .OrderByDescending(m => m.MemberId)
                .Take(8)
                .ToList();

            HomeLoansGrid.ItemsSource = _db.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .AsNoTracking()
                .OrderByDescending(l => l.BorrowDate)
                .ThenByDescending(l => l.LoanId)
                .Take(10)
                .Select(l => new HomeLoanItem
                {
                    BookTitle = l.Book.Title,
                    MemberName = l.Member.FullName,
                    BorrowDate = l.BorrowDate,
                    ReturnDate = l.ReturnDate
                })
                .ToList();
        }

        private void LoadMembers()
        {
            MembersGrid.ItemsSource = _db.Members.AsNoTracking().OrderBy(m => m.FullName).ToList();
        }

        private void LoadLoans()
        {
            var loans = _db.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .AsNoTracking()
                .OrderByDescending(l => l.LoanId)
                .Select(l => new LoanGridItem
                {
                    LoanId = l.LoanId,
                    BookId = l.BookId,
                    MemberId = l.MemberId,
                    BookTitle = l.Book.Title,
                    MemberName = l.Member.FullName,
                    BorrowDate = l.BorrowDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate,
                    Status = l.Status,
                    FineAmount = l.FineAmount
                })
                .ToList();

            LoansGrid.ItemsSource = loans;
        }

        private void LoadReports()
        {
            ReportsGrid.ItemsSource = _db.Reports
                .AsNoTracking()
                .OrderByDescending(r => r.ReportDate)
                .ThenByDescending(r => r.ReportId)
                .ToList();
        }

        private void LoadReportSummary()
        {
            var summary = CalculateReportSummary();

            TxtSummaryBooks.Text = summary.TotalBooks.ToString();
            TxtSummaryMembers.Text = summary.TotalMembers.ToString();
            TxtSummaryLoans.Text = summary.TotalLoans.ToString();
            TxtSummaryOverdue.Text = summary.OverdueLoans.ToString();
            TxtSummaryFine.Text = summary.TotalFine.ToString("0.##");
        }

        private (int TotalBooks, int TotalMembers, int TotalLoans, int OverdueLoans, decimal TotalFine) CalculateReportSummary()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var totalBooks = _db.Books.Sum(b => (int?)b.Quantity) ?? 0;
            var totalMembers = _db.Members.Count();
            var totalLoans = _db.Loans.Count();
            var overdueLoans = _db.Loans.Count(l => l.ReturnDate == null && l.DueDate < today);
            var totalFine = _db.Loans.Sum(l => l.FineAmount ?? 0m);

            return (totalBooks, totalMembers, totalLoans, overdueLoans, totalFine);
        }

        private void BindLoanCombos()
        {
            var books = _db.Books.AsNoTracking().OrderBy(b => b.Title).ToList();
            var members = _db.Members.AsNoTracking().Where(m => m.IsActive != false).OrderBy(m => m.FullName).ToList();

            CmbLoanBook.ItemsSource = books;
            CmbLoanMember.ItemsSource = members;
        }

        private static DateOnly? ToDateOnly(DateTime? value)
        {
            return value.HasValue ? DateOnly.FromDateTime(value.Value) : null;
        }

        private static DateTime? ToDateTime(DateOnly? value)
        {
            return value.HasValue ? value.Value.ToDateTime(TimeOnly.MinValue) : null;
        }

        private static string? SelectedComboText(ComboBox comboBox)
        {
            if (comboBox.SelectedItem is ComboBoxItem item)
            {
                return item.Content?.ToString();
            }

            return comboBox.Text;
        }

        private void ResetBookForm()
        {
            TxtBookIsbn.Text = string.Empty;
            TxtBookTitle.Text = string.Empty;
            TxtBookAuthor.Text = string.Empty;
            TxtBookPublisher.Text = string.Empty;
            TxtBookPublishYear.Text = string.Empty;
            TxtBookGenre.Text = string.Empty;
            TxtBookQuantity.Text = string.Empty;
            TxtBookAvailable.Text = string.Empty;
            BooksGrid.SelectedItem = null;
        }

        private void ResetMemberForm()
        {
            TxtMemberCode.Text = string.Empty;
            TxtMemberFullName.Text = string.Empty;
            DpMemberDob.SelectedDate = null;
            CmbMemberGender.SelectedIndex = -1;
            TxtMemberPhone.Text = string.Empty;
            TxtMemberEmail.Text = string.Empty;
            TxtMemberAddress.Text = string.Empty;
            ChkMemberActive.IsChecked = true;
            MembersGrid.SelectedItem = null;
        }

        private void ResetLoanDefaults()
        {
            DpLoanBorrowDate.SelectedDate = DateTime.Today;
            DpLoanDueDate.SelectedDate = DateTime.Today.AddDays(14);
            DpLoanReturnDate.SelectedDate = null;
            CmbLoanStatus.SelectedIndex = 0;
            TxtLoanFine.Text = "0";
            LoansGrid.SelectedItem = null;
        }

        private void ResetReportForm()
        {
            TxtReportType.Text = string.Empty;
            TxtReportTitle.Text = string.Empty;
            DpReportDate.SelectedDate = DateTime.Today;
            TxtReportCreatedBy.Text = string.Empty;
            TxtReportContent.Text = string.Empty;
            TxtReportNotes.Text = string.Empty;
            TxtReportTotalBooks.Text = "0";
            TxtReportTotalMembers.Text = "0";
            TxtReportTotalLoans.Text = "0";
            TxtReportOverdueLoans.Text = "0";
            TxtReportTotalFine.Text = "0";
            ReportsGrid.SelectedItem = null;
        }

        private void BooksGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BooksGrid.SelectedItem is not Book selected)
            {
                return;
            }

            TxtBookIsbn.Text = selected.Isbn;
            TxtBookTitle.Text = selected.Title;
            TxtBookAuthor.Text = selected.Author;
            TxtBookPublisher.Text = selected.Publisher ?? string.Empty;
            TxtBookPublishYear.Text = selected.PublishYear?.ToString() ?? string.Empty;
            TxtBookGenre.Text = selected.Genre ?? string.Empty;
            TxtBookQuantity.Text = selected.Quantity.ToString();
            TxtBookAvailable.Text = selected.AvailableQuantity.ToString();
        }

        private void MembersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MembersGrid.SelectedItem is not Member selected)
            {
                return;
            }

            TxtMemberCode.Text = selected.MemberCode;
            TxtMemberFullName.Text = selected.FullName;
            DpMemberDob.SelectedDate = ToDateTime(selected.DateOfBirth);
            CmbMemberGender.Text = selected.Gender ?? string.Empty;
            TxtMemberPhone.Text = selected.Phone ?? string.Empty;
            TxtMemberEmail.Text = selected.Email ?? string.Empty;
            TxtMemberAddress.Text = selected.Address ?? string.Empty;
            ChkMemberActive.IsChecked = selected.IsActive ?? true;
        }

        private void LoansGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LoansGrid.SelectedItem is not LoanGridItem selected)
            {
                return;
            }

            CmbLoanBook.SelectedValue = selected.BookId;
            CmbLoanMember.SelectedValue = selected.MemberId;
            DpLoanBorrowDate.SelectedDate = ToDateTime(selected.BorrowDate);
            DpLoanDueDate.SelectedDate = ToDateTime(selected.DueDate);
            DpLoanReturnDate.SelectedDate = ToDateTime(selected.ReturnDate);
            CmbLoanStatus.Text = selected.Status ?? "Borrowing";
            TxtLoanFine.Text = (selected.FineAmount ?? 0).ToString("0.##");
        }

        private void ReportsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReportsGrid.SelectedItem is not Report selected)
            {
                return;
            }

            TxtReportType.Text = selected.ReportType;
            TxtReportTitle.Text = selected.ReportTitle;
            DpReportDate.SelectedDate = ToDateTime(selected.ReportDate);
            TxtReportCreatedBy.Text = selected.CreatedBy ?? string.Empty;
            TxtReportContent.Text = selected.Content ?? string.Empty;
            TxtReportNotes.Text = selected.Notes ?? string.Empty;
            TxtReportTotalBooks.Text = (selected.TotalBooks ?? 0).ToString();
            TxtReportTotalMembers.Text = (selected.TotalMembers ?? 0).ToString();
            TxtReportTotalLoans.Text = (selected.TotalLoans ?? 0).ToString();
            TxtReportOverdueLoans.Text = (selected.OverdueLoans ?? 0).ToString();
            TxtReportTotalFine.Text = (selected.TotalFine ?? 0).ToString("0.##");
        }

        private static int? ParseNullableInt(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            return int.TryParse(input, out var value) ? value : null;
        }

        private static decimal? ParseNullableDecimal(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            return decimal.TryParse(input, out var value) ? value : null;
        }

        private Report? BuildReportFromForm()
        {
            var report = new Report
            {
                ReportType = TxtReportType.Text.Trim(),
                ReportTitle = TxtReportTitle.Text.Trim(),
                ReportDate = ToDateOnly(DpReportDate.SelectedDate ?? DateTime.Today),
                CreatedBy = string.IsNullOrWhiteSpace(TxtReportCreatedBy.Text) ? null : TxtReportCreatedBy.Text.Trim(),
                Content = string.IsNullOrWhiteSpace(TxtReportContent.Text) ? null : TxtReportContent.Text.Trim(),
                Notes = string.IsNullOrWhiteSpace(TxtReportNotes.Text) ? null : TxtReportNotes.Text.Trim(),
                TotalBooks = ParseNullableInt(TxtReportTotalBooks.Text),
                TotalMembers = ParseNullableInt(TxtReportTotalMembers.Text),
                TotalLoans = ParseNullableInt(TxtReportTotalLoans.Text),
                OverdueLoans = ParseNullableInt(TxtReportOverdueLoans.Text),
                TotalFine = ParseNullableDecimal(TxtReportTotalFine.Text)
            };

            if (string.IsNullOrWhiteSpace(report.ReportType) || string.IsNullOrWhiteSpace(report.ReportTitle))
            {
                MessageBox.Show("Loại báo cáo và tiêu đề là bắt buộc.");
                return null;
            }

            if (report.TotalBooks is null || report.TotalMembers is null || report.TotalLoans is null || report.OverdueLoans is null || report.TotalFine is null)
            {
                MessageBox.Show("Các trường số trong báo cáo không hợp lệ.");
                return null;
            }

            return report;
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TxtBookQuantity.Text, out var quantity) || quantity < 0)
            {
                MessageBox.Show("Số lượng không hợp lệ.");
                return;
            }

            if (!int.TryParse(TxtBookAvailable.Text, out var available) || available < 0 || available > quantity)
            {
                MessageBox.Show("Số lượng còn lại không hợp lệ.");
                return;
            }

            int? publishYear = null;
            if (!string.IsNullOrWhiteSpace(TxtBookPublishYear.Text))
            {
                if (!int.TryParse(TxtBookPublishYear.Text, out var year))
                {
                    MessageBox.Show("Năm xuất bản không hợp lệ.");
                    return;
                }
                publishYear = year;
            }

            var book = new Book
            {
                Isbn = TxtBookIsbn.Text.Trim(),
                Title = TxtBookTitle.Text.Trim(),
                Author = TxtBookAuthor.Text.Trim(),
                Publisher = string.IsNullOrWhiteSpace(TxtBookPublisher.Text) ? null : TxtBookPublisher.Text.Trim(),
                PublishYear = publishYear,
                Genre = string.IsNullOrWhiteSpace(TxtBookGenre.Text) ? null : TxtBookGenre.Text.Trim(),
                Quantity = quantity,
                AvailableQuantity = available
            };

            if (string.IsNullOrWhiteSpace(book.Isbn) || string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Author))
            {
                MessageBox.Show("ISBN, tiêu đề và tác giả là bắt buộc.");
                return;
            }

            _db.Books.Add(book);
            SaveAndRefresh();
            ResetBookForm();
        }

        private void UpdateBook_Click(object sender, RoutedEventArgs e)
        {
            if (BooksGrid.SelectedItem is not Book selected)
            {
                MessageBox.Show("Chọn sách cần cập nhật.");
                return;
            }

            var dbBook = _db.Books.Find(selected.BookId);
            if (dbBook is null)
            {
                return;
            }

            if (!int.TryParse(TxtBookQuantity.Text, out var quantity) || quantity < 0)
            {
                MessageBox.Show("Số lượng không hợp lệ.");
                return;
            }

            if (!int.TryParse(TxtBookAvailable.Text, out var available) || available < 0 || available > quantity)
            {
                MessageBox.Show("Số lượng còn lại không hợp lệ.");
                return;
            }

            int? publishYear = null;
            if (!string.IsNullOrWhiteSpace(TxtBookPublishYear.Text))
            {
                if (!int.TryParse(TxtBookPublishYear.Text, out var year))
                {
                    MessageBox.Show("Năm xuất bản không hợp lệ.");
                    return;
                }
                publishYear = year;
            }

            dbBook.Isbn = TxtBookIsbn.Text.Trim();
            dbBook.Title = TxtBookTitle.Text.Trim();
            dbBook.Author = TxtBookAuthor.Text.Trim();
            dbBook.Publisher = string.IsNullOrWhiteSpace(TxtBookPublisher.Text) ? null : TxtBookPublisher.Text.Trim();
            dbBook.PublishYear = publishYear;
            dbBook.Genre = string.IsNullOrWhiteSpace(TxtBookGenre.Text) ? null : TxtBookGenre.Text.Trim();
            dbBook.Quantity = quantity;
            dbBook.AvailableQuantity = available;

            SaveAndRefresh();
        }

        private void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if (BooksGrid.SelectedItem is not Book selected)
            {
                MessageBox.Show("Chọn sách cần xóa.");
                return;
            }

            var hasLoan = _db.Loans.Any(l => l.BookId == selected.BookId);
            if (hasLoan)
            {
                MessageBox.Show("Không thể xóa sách đã có lịch sử mượn.");
                return;
            }

            var dbBook = _db.Books.Find(selected.BookId);
            if (dbBook is null)
            {
                return;
            }

            _db.Books.Remove(dbBook);
            SaveAndRefresh();
            ResetBookForm();
        }

        private void RefreshBooks_Click(object sender, RoutedEventArgs e)
        {
            LoadBooks();
            BindLoanCombos();
            ResetBookForm();
        }

        private void AddMember_Click(object sender, RoutedEventArgs e)
        {
            var member = new Member
            {
                MemberCode = TxtMemberCode.Text.Trim(),
                FullName = TxtMemberFullName.Text.Trim(),
                DateOfBirth = ToDateOnly(DpMemberDob.SelectedDate),
                Gender = string.IsNullOrWhiteSpace(CmbMemberGender.Text) ? null : CmbMemberGender.Text,
                Phone = string.IsNullOrWhiteSpace(TxtMemberPhone.Text) ? null : TxtMemberPhone.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(TxtMemberEmail.Text) ? null : TxtMemberEmail.Text.Trim(),
                Address = string.IsNullOrWhiteSpace(TxtMemberAddress.Text) ? null : TxtMemberAddress.Text.Trim(),
                JoinDate = DateOnly.FromDateTime(DateTime.Today),
                IsActive = ChkMemberActive.IsChecked ?? true
            };

            if (string.IsNullOrWhiteSpace(member.MemberCode) || string.IsNullOrWhiteSpace(member.FullName))
            {
                MessageBox.Show("Mã thành viên và họ tên là bắt buộc.");
                return;
            }

            _db.Members.Add(member);
            SaveAndRefresh();
            ResetMemberForm();
        }

        private void UpdateMember_Click(object sender, RoutedEventArgs e)
        {
            if (MembersGrid.SelectedItem is not Member selected)
            {
                MessageBox.Show("Chọn thành viên cần cập nhật.");
                return;
            }

            var dbMember = _db.Members.Find(selected.MemberId);
            if (dbMember is null)
            {
                return;
            }

            dbMember.MemberCode = TxtMemberCode.Text.Trim();
            dbMember.FullName = TxtMemberFullName.Text.Trim();
            dbMember.DateOfBirth = ToDateOnly(DpMemberDob.SelectedDate);
            dbMember.Gender = string.IsNullOrWhiteSpace(CmbMemberGender.Text) ? null : CmbMemberGender.Text;
            dbMember.Phone = string.IsNullOrWhiteSpace(TxtMemberPhone.Text) ? null : TxtMemberPhone.Text.Trim();
            dbMember.Email = string.IsNullOrWhiteSpace(TxtMemberEmail.Text) ? null : TxtMemberEmail.Text.Trim();
            dbMember.Address = string.IsNullOrWhiteSpace(TxtMemberAddress.Text) ? null : TxtMemberAddress.Text.Trim();
            dbMember.IsActive = ChkMemberActive.IsChecked ?? true;

            SaveAndRefresh();
        }

        private void DeleteMember_Click(object sender, RoutedEventArgs e)
        {
            if (MembersGrid.SelectedItem is not Member selected)
            {
                MessageBox.Show("Chọn thành viên cần xóa.");
                return;
            }

            var hasLoan = _db.Loans.Any(l => l.MemberId == selected.MemberId);
            if (hasLoan)
            {
                MessageBox.Show("Không thể xóa thành viên đã có lịch sử mượn.");
                return;
            }

            var dbMember = _db.Members.Find(selected.MemberId);
            if (dbMember is null)
            {
                return;
            }

            _db.Members.Remove(dbMember);
            SaveAndRefresh();
            ResetMemberForm();
        }

        private void RefreshMembers_Click(object sender, RoutedEventArgs e)
        {
            LoadMembers();
            BindLoanCombos();
            ResetMemberForm();
        }

        private void AddLoan_Click(object sender, RoutedEventArgs e)
        {
            if (CmbLoanBook.SelectedValue is not int bookId || CmbLoanMember.SelectedValue is not int memberId)
            {
                MessageBox.Show("Chọn sách và thành viên.");
                return;
            }

            var book = _db.Books.Find(bookId);
            if (book is null)
            {
                return;
            }

            if (book.AvailableQuantity <= 0)
            {
                MessageBox.Show("Sách đã hết.");
                return;
            }

            if (DpLoanDueDate.SelectedDate is null)
            {
                MessageBox.Show("Vui lòng chọn hạn trả.");
                return;
            }

            if (!decimal.TryParse(TxtLoanFine.Text, out var fine))
            {
                MessageBox.Show("Tiền phạt không hợp lệ.");
                return;
            }

            var loan = new Loan
            {
                BookId = bookId,
                MemberId = memberId,
                BorrowDate = ToDateOnly(DpLoanBorrowDate.SelectedDate ?? DateTime.Today),
                DueDate = DateOnly.FromDateTime(DpLoanDueDate.SelectedDate.Value),
                ReturnDate = ToDateOnly(DpLoanReturnDate.SelectedDate),
                Status = string.IsNullOrWhiteSpace(SelectedComboText(CmbLoanStatus)) ? "Borrowing" : SelectedComboText(CmbLoanStatus),
                FineAmount = fine
            };

            if (loan.Status == "Borrowing" && loan.ReturnDate is not null)
            {
                loan.Status = "Returned";
            }

            if (loan.Status != "Returned")
            {
                book.AvailableQuantity -= 1;
            }

            _db.Loans.Add(loan);
            SaveAndRefresh();
            ResetLoanDefaults();
        }

        private void UpdateLoan_Click(object sender, RoutedEventArgs e)
        {
            if (LoansGrid.SelectedItem is not LoanGridItem selected)
            {
                MessageBox.Show("Chọn phiếu mượn cần cập nhật.");
                return;
            }

            var loan = _db.Loans.Find(selected.LoanId);
            if (loan is null)
            {
                return;
            }

            if (CmbLoanBook.SelectedValue is not int bookId || CmbLoanMember.SelectedValue is not int memberId)
            {
                MessageBox.Show("Chọn sách và thành viên.");
                return;
            }

            if (DpLoanDueDate.SelectedDate is null)
            {
                MessageBox.Show("Vui lòng chọn hạn trả.");
                return;
            }

            if (!decimal.TryParse(TxtLoanFine.Text, out var fine))
            {
                MessageBox.Show("Tiền phạt không hợp lệ.");
                return;
            }

            var newStatus = string.IsNullOrWhiteSpace(SelectedComboText(CmbLoanStatus)) ? "Borrowing" : SelectedComboText(CmbLoanStatus)!;
            var oldReturned = string.Equals(loan.Status, "Returned", StringComparison.OrdinalIgnoreCase);
            var newReturned = string.Equals(newStatus, "Returned", StringComparison.OrdinalIgnoreCase) || DpLoanReturnDate.SelectedDate is not null;

            if (loan.BookId != bookId)
            {
                var oldBook = _db.Books.Find(loan.BookId);
                var newBook = _db.Books.Find(bookId);

                if (newBook is null)
                {
                    return;
                }

                if (!oldReturned)
                {
                    oldBook!.AvailableQuantity += 1;
                    if (newBook.AvailableQuantity <= 0)
                    {
                        MessageBox.Show("Sách mới đã hết.");
                        return;
                    }
                    newBook.AvailableQuantity -= 1;
                }

                loan.BookId = bookId;
            }
            else if (oldReturned != newReturned)
            {
                var sameBook = _db.Books.Find(loan.BookId);
                if (sameBook is not null)
                {
                    if (oldReturned && !newReturned)
                    {
                        if (sameBook.AvailableQuantity <= 0)
                        {
                            MessageBox.Show("Sách đã hết.");
                            return;
                        }
                        sameBook.AvailableQuantity -= 1;
                    }
                    else if (!oldReturned && newReturned)
                    {
                        sameBook.AvailableQuantity += 1;
                    }
                }
            }

            loan.MemberId = memberId;
            loan.BorrowDate = ToDateOnly(DpLoanBorrowDate.SelectedDate ?? DateTime.Today);
            loan.DueDate = DateOnly.FromDateTime(DpLoanDueDate.SelectedDate.Value);
            loan.ReturnDate = ToDateOnly(DpLoanReturnDate.SelectedDate);
            loan.Status = newStatus;
            loan.FineAmount = fine;

            SaveAndRefresh();
        }

        private void MarkReturned_Click(object sender, RoutedEventArgs e)
        {
            if (LoansGrid.SelectedItem is not LoanGridItem selected)
            {
                MessageBox.Show("Chọn phiếu mượn.");
                return;
            }

            var loan = _db.Loans.Find(selected.LoanId);
            if (loan is null)
            {
                return;
            }

            if (!string.Equals(loan.Status, "Returned", StringComparison.OrdinalIgnoreCase))
            {
                var book = _db.Books.Find(loan.BookId);
                if (book is not null)
                {
                    book.AvailableQuantity += 1;
                }
            }

            loan.ReturnDate = ToDateOnly(DpLoanReturnDate.SelectedDate ?? DateTime.Today);
            loan.Status = "Returned";

            SaveAndRefresh();
            ResetLoanDefaults();
        }

        private void DeleteLoan_Click(object sender, RoutedEventArgs e)
        {
            if (LoansGrid.SelectedItem is not LoanGridItem selected)
            {
                MessageBox.Show("Chọn phiếu mượn cần xóa.");
                return;
            }

            var loan = _db.Loans.Find(selected.LoanId);
            if (loan is null)
            {
                return;
            }

            if (!string.Equals(loan.Status, "Returned", StringComparison.OrdinalIgnoreCase))
            {
                var book = _db.Books.Find(loan.BookId);
                if (book is not null)
                {
                    book.AvailableQuantity += 1;
                }
            }

            _db.Loans.Remove(loan);
            SaveAndRefresh();
            ResetLoanDefaults();
        }

        private void RefreshLoans_Click(object sender, RoutedEventArgs e)
        {
            LoadLoans();
            BindLoanCombos();
            ResetLoanDefaults();
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            var summary = CalculateReportSummary();
            var today = DateTime.Today;

            TxtReportType.Text = "Tổng hợp";
            TxtReportTitle.Text = $"Báo cáo tổng hợp ngày {today:dd/MM/yyyy}";
            DpReportDate.SelectedDate = today;
            TxtReportCreatedBy.Text = "Admin";
            TxtReportTotalBooks.Text = summary.TotalBooks.ToString();
            TxtReportTotalMembers.Text = summary.TotalMembers.ToString();
            TxtReportTotalLoans.Text = summary.TotalLoans.ToString();
            TxtReportOverdueLoans.Text = summary.OverdueLoans.ToString();
            TxtReportTotalFine.Text = summary.TotalFine.ToString("0.##");
            TxtReportContent.Text = $"Tổng hợp hệ thống tại ngày {today:dd/MM/yyyy}.";
            TxtReportNotes.Text = "Tạo nhanh từ dữ liệu hiện tại.";
        }

        private void AddReport_Click(object sender, RoutedEventArgs e)
        {
            var report = BuildReportFromForm();
            if (report is null)
            {
                return;
            }

            _db.Reports.Add(report);
            SaveAndRefresh();
            ResetReportForm();
        }

        private void UpdateReport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsGrid.SelectedItem is not Report selected)
            {
                MessageBox.Show("Chọn báo cáo cần cập nhật.");
                return;
            }

            var dbReport = _db.Reports.Find(selected.ReportId);
            if (dbReport is null)
            {
                return;
            }

            var report = BuildReportFromForm();
            if (report is null)
            {
                return;
            }

            dbReport.ReportType = report.ReportType;
            dbReport.ReportTitle = report.ReportTitle;
            dbReport.ReportDate = report.ReportDate;
            dbReport.CreatedBy = report.CreatedBy;
            dbReport.Content = report.Content;
            dbReport.Notes = report.Notes;
            dbReport.TotalBooks = report.TotalBooks;
            dbReport.TotalMembers = report.TotalMembers;
            dbReport.TotalLoans = report.TotalLoans;
            dbReport.OverdueLoans = report.OverdueLoans;
            dbReport.TotalFine = report.TotalFine;

            SaveAndRefresh();
        }

        private void DeleteReport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsGrid.SelectedItem is not Report selected)
            {
                MessageBox.Show("Chọn báo cáo cần xóa.");
                return;
            }

            var dbReport = _db.Reports.Find(selected.ReportId);
            if (dbReport is null)
            {
                return;
            }

            _db.Reports.Remove(dbReport);
            SaveAndRefresh();
            ResetReportForm();
        }

        private void RefreshReports_Click(object sender, RoutedEventArgs e)
        {
            LoadReports();
            LoadReportSummary();
            ResetReportForm();
        }

        private void SaveAndRefresh()
        {
            try
            {
                _db.SaveChanges();
                LoadAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu dữ liệu: {ex.Message}");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _db.Dispose();
            base.OnClosed(e);
        }

        private sealed class LoanGridItem
        {
            public int LoanId { get; set; }
            public int BookId { get; set; }
            public int MemberId { get; set; }
            public string BookTitle { get; set; } = string.Empty;
            public string MemberName { get; set; } = string.Empty;
            public DateOnly? BorrowDate { get; set; }
            public DateOnly DueDate { get; set; }
            public DateOnly? ReturnDate { get; set; }
            public string? Status { get; set; }
            public decimal? FineAmount { get; set; }
        }

        private sealed class HomeLoanItem
        {
            public string BookTitle { get; set; } = string.Empty;
            public string MemberName { get; set; } = string.Empty;
            public DateOnly? BorrowDate { get; set; }
            public DateOnly? ReturnDate { get; set; }
        }
    }
}