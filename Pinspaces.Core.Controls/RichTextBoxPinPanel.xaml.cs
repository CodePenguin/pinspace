using Pinspaces.Core.Interfaces;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Pinspaces.Core.Controls
{
    [PinType(DisplayName = "Rich Text Box", PinType = typeof(RichTextBoxPin))]
    public partial class RichTextBoxPinPanel : PinUserControl<RichTextBoxPin>, IDisposable
    {
        private const string contentDataKey = "content.rtf";
        private readonly IDelayedAction delayedUpdateContentAction;
        private readonly IPinDataRepository pinDataRepository;
        private bool disposedValue;

        public RichTextBoxPinPanel(IDelayedActionFactory delayedActionFactory, IPinDataRepository pinDataRepository)
        {
            this.pinDataRepository = pinDataRepository;
            delayedUpdateContentAction = delayedActionFactory.Debounce(UpdateContent, 5000);

            InitializeComponent();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
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

        protected override void LoadPin()
        {
            LoadContent();
        }

        private void LoadContent()
        {
            if (!pinDataRepository.Retrieve(PinspaceId, Pin.Id, contentDataKey, out var data))
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
                pinDataRepository.Store(PinspaceId, Pin.Id, contentDataKey, stream.ToArray());
            }
        }
    }
}
