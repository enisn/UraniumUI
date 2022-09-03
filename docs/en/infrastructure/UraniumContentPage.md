# UraniumContentPage

This is the main page for the Uranium Content system. It has some infrastructure for building user interface with Uranium UI components.

## Setting-up

- Create a Content Page (XAML) and replace ContentPage with UraniumContentPage.

    ```xml
    <uranium:UraniumContentPage x:Class="App1.MainPage"
                xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                xmlns:uranium="clr-namespace:UraniumUI.Pages;assembly=UraniumUI"
                xmlns:local="clr-namespace:App1">

        <!-- Content here -->
    </uranium:UraniumContentPage>
    ```

- Make sure C# class also inherits from `UraniumContentPage`

    ```csharp
    public partial class MainPage : UraniumContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
    }
    ```

## Attachments

Attachments is a feature of [UraniumContentPage](UraniumContentPage.md) that allows you to add attachments to your content page. Attachments must implement `IPageAttachment` interface. That interface also implements `IView`. So, attachments are View that implements `IPageAttachments`. Different UI packs can include different attachments. 

Attachments aren't same layer with page content and they will automatically rendered over the page. You should place them in a position or use LayoutOptions to align them. When multiple attachments are added to the page, they will be rendered in the order they are added. So, the attachment at the end will be rendered at the front.

```xml
    <uranium:UraniumContentPage x:Class="App1.MainPage"
                xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                xmlns:uranium="clr-namespace:UraniumUI.Pages;assembly=UraniumUI"
                xmlns:local="clr-namespace:App1">

        <!-- Content here -->

        <uranium:UraniumContentPage.Attachments>
            <!-- Attachments here -->
        </uranium:UraniumContentPage.Attachments>
    </uranium:UraniumContentPage>
```

### IPageAttachment
`IPageAttachment` is an interface and contains 2 members that you should implement. 

- `OnAttached(UraniumContentPage attachedPage)`: A mothod that is executed right after attachment is added to the page. You can use this method to initialize your attachment.

- `AttachmentPosition`: A property you should return either of `Front` or `Behind` enum value. This property determines the position of the attachment. 
  - `Front`: The attachment will be rendered at the front.
  - `Behind`: The attachment will be rendered at the back of the entire page. In the **behind** case, your attachment will not be appeared, you should make something with the main page content like making it transparent or slide it away according to your logic.

### Creating an attachment

You can either create an attachment with XAML or C# code. 

- Creating a simple Floating Action Button attachment
```csharp
public class FAB : ImageButton, IPageAttachment
{
    public FAB()
    {
        this.Source = new Uri("arrow.png");
        this.Width = 42;
        this.Height = 42;
        this.CornerRadius = 21;
        this.BackgroundColor = Colors.Blue;

        this.Click += (s, e) =>{ Console.WriteLine("FAB clicked"); };
    }

     public void OnAttached(UraniumContentPage page)
     {
        // Place it right bottom of the page.
        this.TranslationX = this.PageWidth - this.Width - 20;
        this.TranslationY = this.PageHeight - this.Height - 20;
     }
}
```

- Using it in XAML

```xml
    <uranium:UraniumContentPage x:Class="App1.MainPage"
                xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                xmlns:uranium="clr-namespace:UraniumUI.Pages;assembly=UraniumUI"
                xmlns:local="clr-namespace:App1">

        <!-- Content here -->

        <uranium:UraniumContentPage.Attachments>
            <local:FAB />
        </uranium:UraniumContentPage.Attachments>
    </uranium:UraniumContentPage>
```