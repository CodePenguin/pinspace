using System.Windows.Input;

namespace Pinspaces.Controls
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand ChangeColor = new(
            "Change Color",
            "ChangeColor",
            typeof(CustomCommands)
            );

        public static readonly RoutedUICommand New = new(
            "New",
            "New",
            typeof(CustomCommands)
            );

        public static readonly RoutedUICommand Remove = new(
            "Remove",
            "Remove",
            typeof(CustomCommands)
            );

        public static readonly RoutedUICommand Rename = new(
            "Rename",
            "Rename",
            typeof(CustomCommands)
            );
    }
}
