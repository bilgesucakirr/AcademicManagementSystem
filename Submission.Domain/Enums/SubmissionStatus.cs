namespace Submission.Domain.Enums;

public enum SubmissionStatus
{
    Draft,
    Submitted,
    UnderReview,
    MinorRevisionRequired, 
    MajorRevisionRequired, 
    CameraReadyRequested,  
    CameraReadySubmitted,  
    Accepted,
    Rejected,
    Withdrawn
}