using CareerTrail.Domain.Common;

namespace CareerTrail.Domain.Entities;

public class JobApplication
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string CompanyName { get; private set; } = string.Empty;

    public string JobTitle { get; private set; } = string.Empty;

    public string? JobPostingUrl { get; private set; }

    public ApplicationStatus Status { get; private set; }

    public DateTime AppliedDateUtc { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    private JobApplication()
    {
    }

    public JobApplication(
        Guid userId,
        string companyName,
        string jobTitle,
        string? jobPostingUrl,
        DateTime appliedDateUtc)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(companyName))
        {
            throw new ArgumentException("Company name is required.", nameof(companyName));
        }

        if (string.IsNullOrWhiteSpace(jobTitle))
        {
            throw new ArgumentException("Job title is required.", nameof(jobTitle));
        }

        Id = Guid.NewGuid();
        UserId = userId;
        CompanyName = companyName.Trim();
        JobTitle = jobTitle.Trim();
        JobPostingUrl = string.IsNullOrWhiteSpace(jobPostingUrl)
            ? null
            : jobPostingUrl.Trim();
        Status = ApplicationStatus.Applied;
        AppliedDateUtc = appliedDateUtc;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string companyName,
        string jobTitle,
        string? jobPostingUrl)
    {
        if (string.IsNullOrWhiteSpace(companyName))
        {
            throw new ArgumentException("Company name is required.", nameof(companyName));
        }

        if (string.IsNullOrWhiteSpace(jobTitle))
        {
            throw new ArgumentException("Job title is required.", nameof(jobTitle));
        }

        CompanyName = companyName.Trim();
        JobTitle = jobTitle.Trim();
        JobPostingUrl = string.IsNullOrWhiteSpace(jobPostingUrl)
            ? null
            : jobPostingUrl.Trim();

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ChangeStatus(ApplicationStatus newStatus)
    {
        if (!IsValidTransition(Status, newStatus))
        {
            throw new InvalidOperationException(
                $"Cannot change application status from '{Status}' to '{newStatus}'.");
        }

        Status = newStatus;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static bool IsValidTransition(
        ApplicationStatus currentStatus,
        ApplicationStatus newStatus)
    {
        return currentStatus switch
        {
            ApplicationStatus.Saved => newStatus is ApplicationStatus.Applied
                or ApplicationStatus.Withdrawn,

            ApplicationStatus.Applied => newStatus is ApplicationStatus.Screening
                or ApplicationStatus.Interview
                or ApplicationStatus.Rejected
                or ApplicationStatus.Withdrawn,

            ApplicationStatus.Screening => newStatus is ApplicationStatus.Interview
                or ApplicationStatus.Rejected
                or ApplicationStatus.Withdrawn,

            ApplicationStatus.Interview => newStatus is ApplicationStatus.Offer
                or ApplicationStatus.Rejected
                or ApplicationStatus.Withdrawn,

            ApplicationStatus.Offer => newStatus is ApplicationStatus.Rejected
                or ApplicationStatus.Withdrawn,

            ApplicationStatus.Rejected => false,

            ApplicationStatus.Withdrawn => false,

            _ => false
        };
    }
}