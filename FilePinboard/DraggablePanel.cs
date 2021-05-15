using System.Drawing;
using System.Windows.Forms;

namespace FilePinboard
{
    public enum WindowEdge
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
        private WindowEdge mouseDownEdge;
        private Size startingOffset;
        private Size startingSize;

        public DraggablePanel()
        {
            mouseCursor = Cursors.Default;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            startingOffset = new Size(e.Location);
            startingSize = Size;
            mouseDownEdge = WindowEdgeAtPosition(e.Location);
            isDragging = mouseDownEdge == WindowEdge.None;
            isResizing = mouseDownEdge != WindowEdge.None;
            Cursor.Current = mouseCursor;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var offset = e.Location - startingOffset;
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
                    case WindowEdge.Bottom:
                        Height = offset.Y + startingSize.Height;
                        break;

                    case WindowEdge.BottomLeft:
                        Width -= offset.X;
                        Left += offset.X;
                        Height = offset.Y + startingSize.Height;
                        break;

                    case WindowEdge.BottomRight:
                        Width = offset.X + startingSize.Width;
                        Height = offset.Y + startingSize.Height;
                        break;

                    case WindowEdge.Left:
                        Width -= offset.X;
                        Left += offset.X;
                        break;

                    case WindowEdge.Right:
                        Width = offset.X + startingSize.Width;
                        break;

                    case WindowEdge.Top:
                        Height -= offset.Y;
                        Top += offset.Y;
                        break;

                    case WindowEdge.TopLeft:
                        Width -= offset.X;
                        Left += offset.X;
                        Height -= offset.Y;
                        Top += offset.Y;
                        break;

                    case WindowEdge.TopRight:
                        Width = offset.X + startingSize.Width;
                        Height -= offset.Y;
                        Top += offset.Y;
                        break;
                }
            }
            else
            {
                // Change mouse cursor based on edges
                var mouseMoveEdge = WindowEdgeAtPosition(e.Location);
                switch (mouseMoveEdge)
                {
                    case WindowEdge.Top:
                    case WindowEdge.Bottom:
                        mouseCursor = Cursors.SizeNS;
                        break;

                    case WindowEdge.Left:
                    case WindowEdge.Right:
                        mouseCursor = Cursors.SizeWE;
                        break;

                    case WindowEdge.TopLeft:
                    case WindowEdge.BottomRight:
                        mouseCursor = Cursors.SizeNWSE;
                        break;

                    case WindowEdge.TopRight:
                    case WindowEdge.BottomLeft:
                        mouseCursor = Cursors.SizeNESW;
                        break;

                    default:
                        mouseCursor = Cursors.Default;
                        break;
                }
            }
            Cursor.Current = mouseCursor;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isDragging = false;
            isResizing = false;
            mouseDownEdge = WindowEdge.None;
        }

        private WindowEdge WindowEdgeAtPosition(Point p)
        {
            const int edgeThreshold = 10;
            var rightEdge = p.X >= Width - edgeThreshold && p.X <= Width;
            var leftEdge = p.X >= 0 && p.X <= edgeThreshold;
            var topEdge = p.Y >= 0 && p.Y <= edgeThreshold;
            var bottomEdge = p.Y >= Height - edgeThreshold && p.Y <= Height;

            if (topEdge && rightEdge)
            {
                return WindowEdge.TopRight;
            }
            else if (topEdge && leftEdge)
            {
                return WindowEdge.TopLeft;
            }
            else if (bottomEdge && rightEdge)
            {
                return WindowEdge.BottomRight;
            }
            else if (bottomEdge && leftEdge)
            {
                return WindowEdge.BottomLeft;
            }
            else if (leftEdge)
            {
                return WindowEdge.Left;
            }
            else if (rightEdge)
            {
                return WindowEdge.Right;
            }
            else if (topEdge)
            {
                return WindowEdge.Top;
            }
            else if (bottomEdge)
            {
                return WindowEdge.Bottom;
            }
            return WindowEdge.None;
        }
    }
}
