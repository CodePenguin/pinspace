using System.Windows.Input;

namespace Pinspaces.Configuration
{
    public class ApplicationAction
    {
        public string Command { get; set; }
        public string Keys { get; set; }

        public bool TryConvertKeys(out Key key, out ModifierKeys modifierKeys)
        {
            key = Key.None;
            modifierKeys = ModifierKeys.None;
            var values = Keys.Split('+');
            if (values.Length == 0)
            {
                return false;
            }
            var keyValue = values[^1];
            var modifierValues = values[..^1];
            var keyConverter = new KeyConverter();
            key = (Key)keyConverter.ConvertFromString(keyValue);
            if (modifierValues.Length > 0)
            {
                var modifierKeysConverter = new ModifierKeysConverter();
                modifierKeys = (ModifierKeys)modifierKeysConverter.ConvertFromString(string.Join('+', modifierValues));
            }
            return true;
        }
    }
}
