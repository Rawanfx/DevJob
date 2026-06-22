namespace DevJob.Application.DTOs.MockInterview
{
    public class TranscribeResponse
    {

        public bool Success { get; init; }
        public string Text { get; init; } = string.Empty;
        public string Language { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
    }
}
