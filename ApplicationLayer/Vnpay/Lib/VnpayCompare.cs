using System.Globalization;

namespace ApplicationLayer.Vnpay.Lib
{
    public class VnpayCompare : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpayCompare = CompareInfo.GetCompareInfo("en-US");
            return vnpayCompare.Compare(x, y, CompareOptions.Ordinal);
        }
    }
}
