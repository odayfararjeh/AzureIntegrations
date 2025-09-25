namespace CosmosHelper.Helper
{
    public static class ExtensionHelper
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(IEnumerable<T> enumerator, int size)
        {
            var length = enumerator.Count();
            var pos = 0;
            do
            {
                yield return enumerator.Skip(pos).Take(size);
                pos = pos + size;
            } while (pos < length);
        }
    }
}
