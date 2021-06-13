using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pinspaces
{
    public class DraggableController
    {
        private const int edgeThreshold = 6;
        private const double minimumSize = edgeThreshold * 2;
        private readonly ContentControl baseControl;
        private readonly List<UIElement> dragElements = new();
        private bool isDragging;
        private bool isResizing;
        private Cursor mouseCursor;
        private PanelEdge mouseDownEdge;
        private Point startingOffset;
        private Size startingSize;

        public DraggableController(ContentControl baseControl)
        {
            this.baseControl = baseControl;
            AttachMouseEvents(baseControl);
        }

        public enum PanelEdge
        {
            None,
            Top,
            Bottom,
            Left,
            Right,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        public void AttachMouseEvents(UIElement el)
        {
            dragElements.Add(el);
            el.MouseDown += MouseDownHandler;
            el.MouseMove += MouseMoveHandler;
            el.MouseUp += MouseUpHandler;
        }

        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || !dragElements.Contains(e.OriginalSource as UIElement))
            {
                return;
            }
            var mousePos = e.GetPosition(baseControl);
            startingOffset = mousePos;
            startingSize = new Size(baseControl.Width, baseControl.Height);
            mouseDownEdge = PanelEdgeAtPosition(mousePos);
            isDragging = mouseDownEdge == PanelEdge.None;
            isResizing = mouseDownEdge != PanelEdge.None;
            baseControl.Cursor = mouseCursor;

            var el = (UIElement)sender;
            el.CaptureMouse();
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(baseControl);
            var offset = mousePos - startingOffset;
            if (isDragging)
            {
                // Change position while dragging
                Canvas.SetLeft(baseControl, Canvas.GetLeft(baseControl) + offset.X);
                Canvas.SetTop(baseControl, Canvas.GetTop(baseControl) + offset.Y);
            }
            else if (isResizing)
            {
                var newHeight = baseControl.Height;
                var newWidth = baseControl.Width;
                var newLeft = Canvas.GetLeft(baseControl);
                var newTop = Canvas.GetTop(baseControl);

                // Manipulate position and size while dragging
                switch (mouseDownEdge)
                {
                    case PanelEdge.Bottom:
                        newHeight = offset.Y + startingSize.Height;
                        break;

                    case PanelEdge.BottomLeft:
                        newWidth -= offset.X;
                        newLeft += offset.X;
                        newHeight = offset.Y + startingSize.Height;
                        break;

                    case PanelEdge.BottomRight:
                        newWidth = offset.X + startingSize.Width;
                        newHeight = offset.Y + startingSize.Height;
                        break;

                    case PanelEdge.Left:
                        newWidth -= offset.X;
                        newLeft += offset.X;
                        break;

                    case PanelEdge.Right:
                        newWidth = offset.X + startingSize.Width;
                        break;

                    case PanelEdge.Top:
                        newHeight -= offset.Y;
                        newTop += offset.Y;
                        break;

                    case PanelEdge.TopLeft:
                        newWidth -= offset.X;
                        newLeft += offset.X;
                        newHeight -= offset.Y;
                        newTop += offset.Y;
                        break;

                    case PanelEdge.TopRight:
                        newWidth = offset.X + startingSize.Width;
                        newHeight -= offset.Y;
                        newTop += offset.Y;
                        break;
                }
                baseControl.Height = Math.Max(minimumSize, newHeight);
                baseControl.Width = Math.Max(minimumSize, newWidth);
                Canvas.SetLeft(baseControl, newLeft);
                Canvas.SetTop(baseControl, newTop);
            }
            else
            {
                // Change mouse cursor based on edges
                mouseCursor = PanelEdgeAtPosition(mousePos) switch
                {
                    PanelEdge.Top or PanelEdge.Bottom => Cursors.SizeNS,
                    PanelEdge.Left or PanelEdge.Right => Cursors.SizeWE,
                    PanelEdge.TopLeft or PanelEdge.BottomRight => Cursors.SizeNWSE,
                    PanelEdge.TopRight or PanelEdge.BottomLeft => Cursors.SizeNESW,
                    _ => Cursors.Arrow,
                };
            }
            baseControl.Cursor = mouseCursor;
        }

        private void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            var el = (UIElement)sender;
            el.ReleaseMouseCapture();

            isDragging = false;
            isResizing = false;
            mouseDownEdge = PanelEdge.None;
        }

        private PanelEdge PanelEdgeAtPosition(Point p)
        {
            var rightEdge = p.X >= baseControl.Width - edgeThreshold && p.X <= baseControl.Width;
            var leftEdge = p.X >= 0 && p.X <= edgeThreshold;
            var topEdge = p.Y >= 0 && p.Y <= edgeThreshold;
            var bottomEdge = p.Y >= baseControl.Height - edgeThreshold && p.Y <= baseControl.Height;

            if (topEdge && rightEdge)
            {
                return PanelEdge.TopRight;
            }
            else if (topEdge && leftEdge)
            {
                return PanelEdge.TopLeft;
            }
            else if (bottomEdge && rightEdge)
            {
                return PanelEdge.BottomRight;
            }
            else if (bottomEdge && leftEdge)
            {
                return PanelEdge.BottomLeft;
            }
            else if (leftEdge)
            {
                return PanelEdge.Left;
            }
            else if (rightEdge)
            {
                return PanelEdge.Right;
            }
            else if (topEdge)
            {
                return PanelEdge.Top;
            }
            else if (bottomEdge)
            {
                return PanelEdge.Bottom;
            }
            return PanelEdge.None;
        }
    }
}
