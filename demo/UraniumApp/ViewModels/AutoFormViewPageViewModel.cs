using DotNurse.Injector.Attributes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel.DataAnnotations;

namespace UraniumApp.ViewModels;

[RegisterAs(typeof(AutoFormViewPageViewModel))]
public class AutoFormViewPageViewModel : ReactiveObject
{
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
}
