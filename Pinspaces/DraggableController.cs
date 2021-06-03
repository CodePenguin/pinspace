using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pinspaces
{
    public class DraggableController
    {
        private readonly ContentControl baseControl;
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
            el.MouseDown += MouseDownHandler;
            el.MouseMove += MouseMoveHandler;
            el.MouseUp += MouseUpHandler;
        }

        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
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
                // Manipulate position and size while dragging
                switch (mouseDownEdge)
                {
                    case PanelEdge.Bottom:
                        baseControl.Height = offset.Y + startingSize.Height;
                        break;

                    case PanelEdge.BottomLeft:
                        baseControl.Width -= offset.X;
                        Canvas.SetLeft(baseControl, Canvas.GetLeft(baseControl) + offset.X);
                        baseControl.Height = offset.Y + startingSize.Height;
                        break;

                    case PanelEdge.BottomRight:
                        baseControl.Width = offset.X + startingSize.Width;
                        baseControl.Height = offset.Y + startingSize.Height;
                        break;

                    case PanelEdge.Left:
                        baseControl.Width -= offset.X;
                        Canvas.SetLeft(baseControl, Canvas.GetLeft(baseControl) + offset.X);
                        break;

                    case PanelEdge.Right:
                        baseControl.Width = offset.X + startingSize.Width;
                        break;

                    case PanelEdge.Top:
                        baseControl.Height -= offset.Y;
                        Canvas.SetTop(baseControl, Canvas.GetTop(baseControl) + offset.Y);
                        break;

                    case PanelEdge.TopLeft:
                        baseControl.Width -= offset.X;
                        Canvas.SetLeft(baseControl, Canvas.GetLeft(baseControl) + offset.X);
                        baseControl.Height -= offset.Y;
                        Canvas.SetTop(baseControl, Canvas.GetTop(baseControl) + offset.Y);
                        break;

                    case PanelEdge.TopRight:
                        baseControl.Width = offset.X + startingSize.Width;
                        baseControl.Height -= offset.Y;
                        Canvas.SetTop(baseControl, Canvas.GetTop(baseControl) + offset.Y);
                        break;
                }
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
            const int edgeThreshold = 6;
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
