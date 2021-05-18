using System.Drawing;
using System.Windows.Forms;

namespace Pinspace
{
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

    public class DraggablePanel : Panel
    {
        private bool isDragging;
        private bool isResizing;
        private Cursor mouseCursor;
        private PanelEdge mouseDownEdge;
        private Size startingOffset;
        private Size startingSize;

        public DraggablePanel()
        {
            mouseCursor = Cursors.Default;
            MouseDown += MouseDownHandler;
            MouseMove += MouseMoveHandler;
            MouseUp += MouseUpHandler;
        }

        protected void HandleDraggablePanelEvents(Control control)
        {
            control.MouseDown += MouseDownHandler;
            control.MouseMove += MouseMoveHandler;
            control.MouseUp += MouseUpHandler;
        }

        protected void MouseDownHandler(object sender, MouseEventArgs e)
        {
            var mousePos = ControlPointToClientPoint(sender, e.Location);
            startingOffset = new Size(mousePos);
            startingSize = Size;
            mouseDownEdge = PanelEdgeAtPosition(mousePos);
            isDragging = mouseDownEdge == PanelEdge.None;
            isResizing = mouseDownEdge != PanelEdge.None;
            Cursor.Current = mouseCursor;
        }

        protected void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            var mousePos = ControlPointToClientPoint(sender, e.Location);
            var offset = mousePos - startingOffset;
            if (isDragging)
            {
                // Change position while dragging
                Left += offset.X;
                Top += offset.Y;
            }
            else if (isResizing)
            {
                // Manipulate position and size while dragging
                switch (mouseDownEdge)
                {
                    case PanelEdge.Bottom:
                        Height = offset.Y + startingSize.Height;
                        break;

                    case PanelEdge.BottomLeft:
                        Width -= offset.X;
                        Left += offset.X;
                        Height = offset.Y + startingSize.Height;
                        break;

                    case PanelEdge.BottomRight:
                        Width = offset.X + startingSize.Width;
                        Height = offset.Y + startingSize.Height;
                        break;

                    case PanelEdge.Left:
                        Width -= offset.X;
                        Left += offset.X;
                        break;

                    case PanelEdge.Right:
                        Width = offset.X + startingSize.Width;
                        break;

                    case PanelEdge.Top:
                        Height -= offset.Y;
                        Top += offset.Y;
                        break;

                    case PanelEdge.TopLeft:
                        Width -= offset.X;
                        Left += offset.X;
                        Height -= offset.Y;
                        Top += offset.Y;
                        break;

                    case PanelEdge.TopRight:
                        Width = offset.X + startingSize.Width;
                        Height -= offset.Y;
                        Top += offset.Y;
                        break;
                }
            }
            else
            {
                // Change mouse cursor based on edges
                var mouseMoveEdge = PanelEdgeAtPosition(mousePos);
                switch (mouseMoveEdge)
                {
                    case PanelEdge.Top:
                    case PanelEdge.Bottom:
                        mouseCursor = Cursors.SizeNS;
                        break;

                    case PanelEdge.Left:
                    case PanelEdge.Right:
                        mouseCursor = Cursors.SizeWE;
                        break;

                    case PanelEdge.TopLeft:
                    case PanelEdge.BottomRight:
                        mouseCursor = Cursors.SizeNWSE;
                        break;

                    case PanelEdge.TopRight:
                    case PanelEdge.BottomLeft:
                        mouseCursor = Cursors.SizeNESW;
                        break;

                    default:
                        mouseCursor = Cursors.Default;
                        break;
                }
            }
            Cursor.Current = mouseCursor;
        }

        protected void MouseUpHandler(object sender, MouseEventArgs e)
        {
            isDragging = false;
            isResizing = false;
            mouseDownEdge = PanelEdge.None;
        }

        private Point ControlPointToClientPoint(object sender, Point controlPoint)
        {
            if (sender == this)
            {
                return controlPoint;
            }
            return PointToClient((sender as Control).PointToScreen(controlPoint));
        }

        private PanelEdge PanelEdgeAtPosition(Point p)
        {
            const int edgeThreshold = 6;
            var rightEdge = p.X >= Width - edgeThreshold && p.X <= Width;
            var leftEdge = p.X >= 0 && p.X <= edgeThreshold;
            var topEdge = p.Y >= 0 && p.Y <= edgeThreshold;
            var bottomEdge = p.Y >= Height - edgeThreshold && p.Y <= Height;

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
