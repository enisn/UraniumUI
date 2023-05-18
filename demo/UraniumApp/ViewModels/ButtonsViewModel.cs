using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace UraniumApp.ViewModels;

public class ButtonsViewModel : ReactiveObject
{
    public SingleButtonEditorViewModel FilledButtonContext { get; } = new SingleButtonEditorViewModel("FilledButton");
    public SingleButtonEditorViewModel OutlinedButtonContext { get; } = new SingleButtonEditorViewModel("OutlinedButton");
    public SingleButtonEditorViewModel FilledTonalButtonContext { get; } = new SingleButtonEditorViewModel("FilledTonalButton");
    public SingleButtonEditorViewModel ElevatedButtonContext { get; } = new SingleButtonEditorViewModel("ElevatedButton");
    public SingleButtonEditorViewModel TextButtonContext { get; } = new SingleButtonEditorViewModel("TextButton");

    public ICommand OpenDocumentationCommand { get; }

    public ButtonsViewModel()
    {
        OpenDocumentationCommand = new Command(
                       async () => await Browser.Default.OpenAsync(
                            "https://enisn-projects.io/docs/en/uranium/latest/themes/material/Buttons",
                            BrowserLaunchMode.SystemPreferred));
    }

    public class SingleButtonEditorViewModel : ReactiveObject
    {
        [Reactive] public string Text { get; set; } = "Click Me!";

        [Reactive] public bool IsEnabled { get; set; } = true;

        public string XamlSourceCode => SourceCode.ToString();

        protected XDocument SourceCode { get; }

        public string StyleClass { get; }

        public ICommand OpenDocumentationCommand { get; }
        public ICommand OpenSourceCodeCommand { get; }

        public SingleButtonEditorViewModel(string styleClass = "FilledButton")
        {
            StyleClass = styleClass;

            OpenDocumentationCommand = new Command(
                async () => await Browser.Default.OpenAsync(
                    "https://enisn-projects.io/docs/en/uranium/latest/themes/material/Buttons",
                    BrowserLaunchMode.SystemPreferred));

            SourceCode = XDocument.Parse($"""<Button Text="{Text}" />""");

            this.WhenAnyValue(x => x.Text)
                .Subscribe(_ => GenerateSourceCode());

            this.WhenAnyValue(x => x.IsEnabled)
                .Subscribe(_ => GenerateSourceCode());
        }

        protected void GenerateSourceCode()
        {
            var button = SourceCode.Descendants().First();

            button.SetAttributeValue("Text", Text);
            button.SetAttributeValue("StyleClass", StyleClass);

            if (IsEnabled)
            {
                button.Attribute("IsEnabled")?.Remove();
            }
            else
            {
                button.SetAttributeValue(nameof(IsEnabled), IsEnabled.ToString());
            }

            this.RaisePropertyChanged(nameof(XamlSourceCode));
        }
    }
}
