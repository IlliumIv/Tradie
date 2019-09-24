using System.Collections.Generic;
using SharpDX;

namespace Tradie
{
    public class ItemDisplay
    {
        public IEnumerable<Item> Items { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int TextSize { get; set; }
        public Color TextColor { get; set; }
        public Color BackgroundColor { get; set; }
        public int BackgroundTransparency { get; set; }
        public int ImageSize { get; set; }
        public int Spacing { get; set; }
        public bool LeftAlignment { get; set; }
        public bool Ascending { get; set; }

        public ItemDisplay()
        {

        }

        public ItemDisplay(IEnumerable<Item> items, float x, float y, int textSize, int imageSize, int spacing,
            Color textColor, Color backgroundColor, int backgroundTransparency, bool leftAlignment, bool ascending)
        {
            Items = items;
            X = x;
            Y = y;
            TextColor = textColor;
            TextSize = textSize;
            BackgroundColor = backgroundColor;
            BackgroundTransparency = backgroundTransparency;
            ImageSize = imageSize;
            Spacing = spacing;
            LeftAlignment = leftAlignment;
            Ascending = ascending;
        }
    }
}
