using Pinspaces.Core.Data;
using Pinspaces.Core.Interfaces;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Pinspaces.Core.Controls
{
    [PinType(DisplayName = "Rich Text Box", PinType = typeof(RichTextBoxPin))]
    public partial class RichTextBoxPinPanel : UserControl, IPinControl, IDisposable
    {
        private const string contentDataKey = "content.rtf";
        private readonly IDelayedAction delayedUpdateContentAction;
        private readonly IPinDataRepository pinDataRepository;
        private bool disposedValue;
        private RichTextBoxPin pin;
        private Guid pinspaceId;

        public RichTextBoxPinPanel(IDelayedActionFactory delayedActionFactory, IPinDataRepository pinDataRepository)
        {
            this.pinDataRepository = pinDataRepository;
            delayedUpdateContentAction = delayedActionFactory.Debounce(UpdateContent, 5000);

            InitializeComponent();
            DataContext = pin;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Control ContentControl => this;

        public void AddContextMenuItems(ContextMenu contextMenu)
        {
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void LoadPin(Guid pinspaceId, Pin pin)
        {
            DataContext = pin;
            this.pinspaceId = pinspaceId;
            this.pin = pin as RichTextBoxPin;
            LoadContent();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    delayedUpdateContentAction?.Stop();
                }
                disposedValue = true;
            }
        }

        private void LoadContent()
        {
            if (!pinDataRepository.Retrieve(pinspaceId, pin.Id, contentDataKey, out var data))
            {
                richTextBox.Document.Blocks.Clear();
                return;
            }
            var stream = new MemoryStream(data);
            var range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            range.Load(stream, DataFormats.Rtf);
        }

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            delayedUpdateContentAction.Execute();
        }

        private void UpdateContent()
        {
            var range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            using (var stream = new MemoryStream())
            {
                range.Save(stream, DataFormats.Rtf);
                pinDataRepository.Store(pinspaceId, pin.Id, contentDataKey, stream.ToArray());
            }
            PropertyChanged?.Invoke(pin, new PropertyChangedEventArgs(nameof(pin.Content)));
        }
    }
}
