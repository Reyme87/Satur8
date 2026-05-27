namespace Satur8.Persistence
{
    public class ServiceResult
    {
        public bool Success { get; private set; }
        public string? Error { get; private set; }

        public static ServiceResult Ok() => new() { Success = true };
        public static ServiceResult Fail(string e) => new() { Success = false, Error = e };
    }
}
