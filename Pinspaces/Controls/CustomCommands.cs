using System.Windows.Input;

namespace Pinspaces.Controls
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand ChangeColor = new(
            "Change Color",
            "changeColor",
            typeof(CustomCommands)
            );

        public static readonly RoutedUICommand New = new(
            "New",
            "new",
            typeof(CustomCommands)
            );

        public static readonly RoutedUICommand NextPinspace = new(
            "Next Pinspace",
            "nextPinspace",
            typeof(CustomCommands)
            );

        public static readonly RoutedUICommand PreviousPinspace = new(
            "Previous Pinspace",
            "previousPinspace",
            typeof(CustomCommands)
            );

        public static readonly RoutedUICommand Remove = new(
            "Remove",
            "remove",
            typeof(CustomCommands)
            );

        public static readonly RoutedUICommand Rename = new(
            "Rename",
            "rename",
            typeof(CustomCommands)
            );
    }
}
