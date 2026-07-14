using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AvaVibeTweak.Adorners
{
    public class HighlightAdorner : Control
    {
        private Control? _adornedElement;
        private AdornerLayer? _adornerLayer;

        public HighlightAdorner()
        {
            IsHitTestVisible = false;
            ClipToBounds = false;
        }

        public void Attach(Control target)
        {
            if (_adornedElement == target) return;
            Detach();
            
            _adornedElement = target;
            _adornerLayer = AdornerLayer.GetAdornerLayer(target);
            
            if (_adornerLayer != null)
            {
                AdornerLayer.SetAdornedElement(this, target);
                _adornerLayer.Children.Add(this);
            }
        }

        public void Detach()
        {
            if (_adornerLayer != null && _adornerLayer.Children.Contains(this))
            {
                _adornerLayer.Children.Remove(this);
            }
            _adornedElement = null;
            _adornerLayer = null;
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            if (_adornedElement == null) return;

            var bounds = new Rect(new Point(0, 0), Bounds.Size);
            var pen = new Pen(Brushes.Magenta, 2);
            context.DrawRectangle(null, pen, bounds);
        }
    }
}
