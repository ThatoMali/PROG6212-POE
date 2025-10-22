// Client-side validation for claim form
function validateClaimForm() {
    const hours = parseFloat($('#hoursWorked').val());
    const rate = parseFloat($('#hourlyRate').val());

    if (hours <= 0) {
        alert('Hours worked must be greater than 0.');
        return false;
    }

    if (rate <= 0) {
        alert('Hourly rate must be greater than 0.');
        return false;
    }

    return true;
}

// Attach validation to form submit
$(document).ready(function () {
    $('#claimForm').on('submit', function () {
        return validateClaimForm();
    });
});