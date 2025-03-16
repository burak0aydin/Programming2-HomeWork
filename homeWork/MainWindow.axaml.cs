// using yönergeleri ve namespace bildirimi:
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Animation;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace homeWork;

public partial class MainWindow : Window
{
    private readonly Random _random = new();
    private Point _lastPosition;
    private TemplatedControl? _draggedControl;
    private readonly List<TemplatedControl> _draggableControls = new();
    private const int ComponentWidth = 150;
    private const int ComponentHeight = 40;

    // Modern renk paleti (Label ve TextBox'lar için renkler)
    private static readonly Color[] ColorPalette = new[]
    {
        Color.Parse("#60A5FA"), // Mavi
        Color.Parse("#34D399"), // Yeşil
        Color.Parse("#F472B6"), // Pembe
        Color.Parse("#A78BFA"), // Mor
        Color.Parse("#FBBF24")  // Sarı
    };

    public MainWindow()
    {
        InitializeComponent();
        this.Loaded += MainWindow_Loaded;
        CheckCollisionsButton.Click += CheckCollisions_Click;
    }

    private void MainWindow_Loaded(object? sender, EventArgs e)
    {
        for (int i = 0; i < 5; i++)
        {
            CreateLabel($"Label {i + 1}", i);
            CreateTextBox($"TextBox {i + 1}", i);
        }
    }

    private void CreateLabel(string name, int colorIndex)
    {
        var color = ColorPalette[colorIndex];
        var label = new Label
        {
            Name = name,
            Content = name,
            Width = ComponentWidth,
            Height = ComponentHeight,
            Background = new SolidColorBrush(color) { Opacity = 0.2 },
            Foreground = new SolidColorBrush(color),
            HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Padding = new Thickness(10),
            BorderBrush = new SolidColorBrush(color) { Opacity = 0.5 },
            BorderThickness = new Thickness(2),
            CornerRadius = new CornerRadius(8),
            FontSize = 14,
            FontWeight = FontWeight.Medium,
            [Canvas.LeftProperty] = _random.Next(0, (int)(Bounds.Width - ComponentWidth)),
            [Canvas.TopProperty] = _random.Next(0, (int)(Bounds.Height - ComponentHeight - 200))
        };

        // Animasyon efekti ekle (büyüme/küçülme geçişi)
        label.Transitions = new Transitions
        {
            new TransformOperationsTransition
            {
                Property = Visual.RenderTransformProperty,
                Duration = TimeSpan.FromSeconds(0.2)
            }
        };

        SetupDraggableControl(label);
        MainCanvas.Children.Add(label);
        _draggableControls.Add(label);
    }

    private void CreateTextBox(string name, int colorIndex)
    {
        var color = ColorPalette[colorIndex];
        var textBox = new TextBox
        {
            Name = name,
            Text = name,
            Width = ComponentWidth,
            Height = ComponentHeight,
            Background = new SolidColorBrush(Color.Parse("#1E293B")),
            Foreground = new SolidColorBrush(color),
            BorderBrush = new SolidColorBrush(color) { Opacity = 0.5 },
            BorderThickness = new Thickness(2),
            CornerRadius = new CornerRadius(8),
            FontSize = 14,
            FontWeight = FontWeight.Medium,
            Padding = new Thickness(10, 8),
            [Canvas.LeftProperty] = _random.Next(0, (int)(Bounds.Width - ComponentWidth)),
            [Canvas.TopProperty] = _random.Next(0, (int)(Bounds.Height - ComponentHeight - 200))
        };

        // Animasyon efekti ekle (büyüme/küçülme geçişi)
        textBox.Transitions = new Transitions
        {
            new TransformOperationsTransition
            {
                Property = Visual.RenderTransformProperty,
                Duration = TimeSpan.FromSeconds(0.2)
            }
        };

        SetupDraggableControl(textBox);
        MainCanvas.Children.Add(textBox);
        _draggableControls.Add(textBox);
    }

    private void SetupDraggableControl(TemplatedControl control)
    {
        control.PointerPressed += Control_PointerPressed;
        control.PointerMoved += Control_PointerMoved;
        control.PointerReleased += Control_PointerReleased;

        // Hover efekti (fareyle üzerine gelince büyüt ve öne getir)
        control.PointerEntered += (s, e) => {
            if (s is TemplatedControl c)
            {
                c.RenderTransform = new ScaleTransform(1.05, 1.05);
                c.ZIndex = 1;
            }
        };

        control.PointerExited += (s, e) => {
            if (s is TemplatedControl c)
            {
                c.RenderTransform = new ScaleTransform(1, 1);
                if (c != _draggedControl)
                    c.ZIndex = 0;
            }
        };
    }

    private void Control_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is TemplatedControl control)
        {
            _draggedControl = control;
            _lastPosition = e.GetPosition(MainCanvas);
            control.ZIndex = 2;
            control.Opacity = 0.8;
        }
    }

    private void Control_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_draggedControl != null && e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
        {
            var currentPosition = e.GetPosition(MainCanvas);
            var deltaX = currentPosition.X - _lastPosition.X;
            var deltaY = currentPosition.Y - _lastPosition.Y;

            var newLeft = Canvas.GetLeft(_draggedControl) + deltaX;
            var newTop = Canvas.GetTop(_draggedControl) + deltaY;

            newLeft = Math.Max(0, Math.Min(newLeft, MainCanvas.Bounds.Width - _draggedControl.Bounds.Width));
            newTop = Math.Max(0, Math.Min(newTop, MainCanvas.Bounds.Height - _draggedControl.Bounds.Height));

            Canvas.SetLeft(_draggedControl, newLeft);
            Canvas.SetTop(_draggedControl, newTop);

            _lastPosition = currentPosition;
            CheckCollisionsForControl(_draggedControl);
        }
    }

    private void Control_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_draggedControl != null)
        {
            _draggedControl.ZIndex = 0;
            _draggedControl.Opacity = 1;
            _draggedControl = null;
        }
    }

    private void CheckCollisions_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CollisionListBox.Items.Clear();
        CheckAllCollisions();
    }

    private void CheckCollisionsForControl(TemplatedControl control)
    {
        foreach (var c in _draggableControls)
        {
            var color = ColorPalette[_draggableControls.IndexOf(c) % ColorPalette.Length];
            c.BorderBrush = new SolidColorBrush(color) { Opacity = 0.5 };
        }

        foreach (var other in _draggableControls)
        {
            if (control != other && IsColliding(control, other))
            {
                control.BorderBrush = new SolidColorBrush(Colors.Red);
                other.BorderBrush = new SolidColorBrush(Colors.Red);
            }
        }
    }

    private void CheckAllCollisions()
    {
        foreach (var control in _draggableControls)
        {
            var color = ColorPalette[_draggableControls.IndexOf(control) % ColorPalette.Length];
            control.BorderBrush = new SolidColorBrush(color) { Opacity = 0.5 };
        }

        for (int i = 0; i < _draggableControls.Count; i++)
        {
            for (int j = i + 1; j < _draggableControls.Count; j++)
            {
                var control1 = _draggableControls[i];
                var control2 = _draggableControls[j];

                if (IsColliding(control1, control2))
                {
                    control1.BorderBrush = new SolidColorBrush(Colors.Red);
                    control2.BorderBrush = new SolidColorBrush(Colors.Red);
                    CollisionListBox.Items.Add($"⚠️ {control1.Name} ve {control2.Name} çakışıyor");
                }
            }
        }
    }

    private bool IsColliding(Control control1, Control control2)
    {
        var rect1 = new Rect(
            Canvas.GetLeft(control1),
            Canvas.GetTop(control1),
            control1.Bounds.Width,
            control1.Bounds.Height
        );

        var rect2 = new Rect(
            Canvas.GetLeft(control2),
            Canvas.GetTop(control2),
            control2.Bounds.Width,
            control2.Bounds.Height
        );

        return rect1.Intersects(rect2);
    }
}