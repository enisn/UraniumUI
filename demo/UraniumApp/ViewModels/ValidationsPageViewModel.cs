using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UraniumUI;

namespace UraniumApp.ViewModels;
public class ValidationsPageViewModel : UraniumBindableObject
{
    private string email = string.Empty;
    private string fullName = string.Empty;
    private Gender? gender;
    private DateTime? birthDate;
    private TimeSpan? meetingTime;
    private int? numberOfSeats;
    private bool isTermsAndConditionsAccepted;

    public ValidationsPageViewModel()
    {
        SubmitCommand = new Command(() =>
        {
            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Colors.Red,
                TextColor = Colors.Green,
                ActionButtonTextColor = Colors.Yellow,
                CornerRadius = new CornerRadius(10),
                CharacterSpacing = 0.5
            };
            Snackbar.Make($"Thank you {FullName}. You successfully registered!", duration: TimeSpan.FromSeconds(6) , visualOptions: snackbarOptions)
            .Show();
            ClearCommand.Execute(null);
        });

        ClearCommand = new Command(() =>
        {
            Email = string.Empty;
            FullName = string.Empty;
            Gender = default;
            BirthDate = default;
            MeetingTime = default;
            NumberOfSeats = default;
            IsTermsAndConditionsAccepted = default;
        });

        FillCommand = new Command(() =>
        {
            Email = "a@b.c";
            FullName = "Full Name";
            Gender = ViewModels.Gender.Male;
            BirthDate = DateTime.UtcNow.AddYears(-26);
            MeetingTime = new TimeSpan(10,00,00);
            NumberOfSeats = 2;
            IsTermsAndConditionsAccepted = true;
        });
    }

    public ICommand SubmitCommand { get; set; }

    public ICommand ClearCommand { get; set; }

    public ICommand FillCommand { get; set; }

    public string Email { get => email; set => SetProperty(ref email, value); }
    public string FullName { get => fullName; set => SetProperty(ref fullName, value); }
    public Gender? Gender { get => gender; set => SetProperty(ref gender, value); }
    public DateTime? BirthDate { get => birthDate; set => SetProperty(ref birthDate, value); }
    public TimeSpan? MeetingTime { get => meetingTime; set => SetProperty(ref meetingTime, value); }
    public int? NumberOfSeats { get => numberOfSeats; set => SetProperty(ref numberOfSeats, value); }
    public bool IsTermsAndConditionsAccepted { get => isTermsAndConditionsAccepted; set => SetProperty(ref isTermsAndConditionsAccepted, value); }
}
public enum Gender
{
    Other,
    Male,
    Female
}
