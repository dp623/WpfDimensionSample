namespace WpfDimensionSample.Models
{
    public sealed class DimensionDisplay
    {
        public DimensionDisplay(
            bool isInputVisible,
            bool isLineVisible,
            bool isLabelVisible)
        {
            IsInputVisible = isInputVisible;
            IsLineVisible = isLineVisible;
            IsLabelVisible = isLabelVisible;
        }

        public bool IsInputVisible { get; private set; }

        public bool IsLineVisible { get; private set; }

        public bool IsLabelVisible { get; private set; }

        public static DimensionDisplay Visible()
        {
            return new DimensionDisplay(true, true, true);
        }

        public static DimensionDisplay Hidden()
        {
            return new DimensionDisplay(false, false, false);
        }
    }
}
