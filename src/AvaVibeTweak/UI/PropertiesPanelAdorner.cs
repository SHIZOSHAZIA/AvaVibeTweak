using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using AvaVibeTweak.Patching;

namespace AvaVibeTweak.UI
{
    public class PropertiesPanelAdorner : Control
    {
        private Control? _target;
        private AdornerLayer? _adornerLayer;
        private readonly Border _container;
        private readonly StackPanel _panel;

        public PropertiesPanelAdorner()
        {
            IsHitTestVisible = true;
            ClipToBounds = false;
            
            _panel = new StackPanel { Spacing = 5 };
            _container = new Border
            {
                Background = Brushes.WhiteSmoke,
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10),
                CornerRadius = new CornerRadius(4),
                Child = _panel,
                Width = 200
            };
            
            LogicalChildren.Add(_container);
            VisualChildren.Add(_container);
        }

        public void Attach(Control target)
        {
            Detach();
            _target = target;
            _adornerLayer = AdornerLayer.GetAdornerLayer(target);
            
            if (_adornerLayer != null)
            {
                BuildUI();
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
            _target = null;
            _adornerLayer = null;
        }

        private void BuildUI()
        {
            _panel.Children.Clear();
            if (_target == null) return;
            
            _panel.Children.Add(new TextBlock { Text = $"Edit: {_target.Name ?? _target.GetType().Name}", FontWeight = FontWeight.Bold });

            AddPropertyEditor("Margin", _target.Margin.ToString(), val => {
                try { _target.Margin = Thickness.Parse(val); PatchGenerator.RecordChange(_target, "Margin", val); } catch {}
            });
            
            AddPropertyEditor("Padding", _target.GetValue(Decorator.PaddingProperty).ToString() ?? "0", val => {
                try { _target.SetValue(Decorator.PaddingProperty, Thickness.Parse(val)); PatchGenerator.RecordChange(_target, "Padding", val); } catch {}
            });

            if (_target is TextBlock tb)
            {
                AddPropertyEditor("FontSize", tb.FontSize.ToString(), val => {
                    try { if (double.TryParse(val, out var size)) { tb.FontSize = size; PatchGenerator.RecordChange(_target, "FontSize", size); } } catch {}
                });
            }
            
            AddPropertyEditor("Width", _target.Width.ToString(), val => {
                try { if (double.TryParse(val, out var w)) { _target.Width = w; PatchGenerator.RecordChange(_target, "Width", w); } } catch {}
            });
            AddPropertyEditor("Height", _target.Height.ToString(), val => {
                try { if (double.TryParse(val, out var h)) { _target.Height = h; PatchGenerator.RecordChange(_target, "Height", h); } } catch {}
            });
        }

        private void AddPropertyEditor(string label, string currentValue, Action<string> onChange)
        {
            var sp = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 5 };
            sp.Children.Add(new TextBlock { Text = label, Width = 60, VerticalAlignment = VerticalAlignment.Center });
            
            var tb = new TextBox { Text = currentValue, Width = 100 };
            tb.TextChanged += (s, e) => onChange(tb.Text ?? "");
            
            sp.Children.Add(tb);
            _panel.Children.Add(sp);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _container.Measure(Size.Infinity);
            return _container.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_target != null)
            {
                _container.Arrange(new Rect(finalSize.Width + 10, 0, _container.DesiredSize.Width, _container.DesiredSize.Height));
            }
            return finalSize;
        }
    }
}
