namespace Template.Core.Shared.Kernel.Errors;

public static class EnumerableExtensions
{
    extension<T>(IEnumerable<T> source)
    {
        public string ToFormattedString(
            Func<T, int, string> convertToFunc,
            string? separator = ",",
            string? prefix = null)
        {
            var mapped = source.Select(convertToFunc);
            var joined = string.Join(separator, mapped);
            return prefix is null ? joined : $"{prefix}{joined}";
        }
    }
}
