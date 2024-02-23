using DotNurse.Injector.Attributes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Windows.Input;
using UraniumUI.Dialogs;

namespace UraniumApp.ViewModels;

[RegisterAs(typeof(AutoFormViewPageViewModel))]
public class AutoFormViewPageViewModel : ReactiveObject
{
    protected IDialogService _dialogService;
    public AutoFormViewPageViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
        ShowDialogCommand = new Command(async () =>
        {
            var result = await _dialogService.DisplayFormViewAsync("Auto Form View", this);
            if (result != null)
            {
                await _dialogService.DisplayViewAsync("Auto Form View Result", new Label { Text = JsonSerializer.Serialize(result) });
            }
        });
    }

    [EmailAddress]
    [Required]
    [MinLength(5)]
    [Reactive] public string Email { get; set; }
    [Reactive] public string FullName { get; set; }
    [Reactive] public Gender Gender { get; set; }
    [Reactive] public DateTime? BirthDate { get; set; }
    [Reactive] public TimeSpan? MeetingTime { get; set; }
    [Reactive] public int? NumberOfSeats { get; set; }

    [Required]
    [Reactive] public bool IsTermsAndConditionsAccepted { get; set; }

    public ICommand ShowDialogCommand { get; }
}
