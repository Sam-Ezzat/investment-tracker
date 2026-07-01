// Investment Tracker - Custom JavaScript

$(document).ready(function () {
    // Initialize DataTables on all tables with class 'datatable'
    $('.datatable').DataTable({
        responsive: true,
        language: {
            search: "Search:",
            lengthMenu: "Show _MENU_ entries",
            info: "Showing _START_ to _END_ of _TOTAL_ entries",
            infoEmpty: "Showing 0 to 0 of 0 entries",
            infoFiltered: "(filtered from _MAX_ total entries)",
            paginate: {
                first: "First",
                last: "Last",
                next: "Next",
                previous: "Previous"
            }
        },
        pageLength: 25,
        order: [[0, 'desc']]
    });

    // Auto-hide alerts after 5 seconds
    setTimeout(function () {
        $('.alert').fadeOut('slow');
    }, 5000);

    // RTL Toggle
    $('#toggleRTL').click(function (e) {
        e.preventDefault();
        var currentDir = $('html').attr('dir');
        if (currentDir === 'rtl') {
            $('html').attr('dir', 'ltr');
            $('html').attr('lang', 'en');
            localStorage.setItem('direction', 'ltr');
        } else {
            $('html').attr('dir', 'rtl');
            $('html').attr('lang', 'ar');
            localStorage.setItem('direction', 'rtl');
        }
        location.reload();
    });

    // Restore RTL preference
    var savedDirection = localStorage.getItem('direction');
    if (savedDirection) {
        $('html').attr('dir', savedDirection);
        $('html').attr('lang', savedDirection === 'rtl' ? 'ar' : 'en');
    }

    // Format currency display
    $('.currency').each(function () {
        var value = parseFloat($(this).text());
        if (!isNaN(value)) {
            $(this).text(value.toLocaleString('en-US', {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            }));
        }
    });

    // Confirm delete actions
    $('form[action*="Delete"]').submit(function (e) {
        if (!confirm('Are you sure you want to delete this item?')) {
            e.preventDefault();
            return false;
        }
    });

    // Active menu highlighting
    var path = window.location.pathname;
    $('.nav-sidebar a').each(function () {
        var href = $(this).attr('href');
        if (path.indexOf(href) === 0 && href !== '/') {
            $(this).addClass('active');
            $(this).closest('.nav-item').addClass('menu-open');
        }
    });

    // Real-time investment calculation
    $('#calculateInvestment').click(function (e) {
        e.preventDefault();
        
        var data = {
            principalAmount: parseFloat($('#PrincipalAmount').val()) || 0,
            startDate: $('#StartDate').val(),
            durationMonths: parseInt($('#DurationMonths').val()) || 0,
            interestType: parseInt($('#InterestType').val()) || 0,
            interestRate: parseFloat($('#InterestRate').val()) || 0
        };

        $.ajax({
            url: '/InvestmentCycle/Calculate',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (result) {
                $('#EndDate').val(result.endDate);
                $('#MonthlyInterest').text(result.monthlyInterest.toFixed(2));
                $('#TotalExpectedProfit').text(result.totalExpectedProfit.toFixed(2));
                $('#FinalAmount').text(result.finalAmount.toFixed(2));
                $('#calculationResults').show();
            },
            error: function () {
                alert('Error calculating investment values.');
            }
        });
    });

    // Disable submit button on form submission to prevent double-click
    $('form').submit(function () {
        $(this).find('button[type="submit"]').prop('disabled', true);
        return true;
    });
});

// Helper function to format currency
function formatCurrency(value) {
    return value.toLocaleString('en-US', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}

// Helper function to format date
function formatDate(dateString) {
    var date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    });
}
