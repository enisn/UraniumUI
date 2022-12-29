using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using UraniumUI;

namespace UraniumApp.Pages.AutoCompleteTextField;
public class GoogleAutoCompleteViewModel : UraniumBindableObject
{
    private string searchtext;

    public string Searchtext { get => searchtext; set => SetProperty(ref searchtext, value, doAfter:UpdateSuggestions); }

    private IEnumerable<string> suggestions;
    public IEnumerable<string> Suggestions { get => suggestions; set => SetProperty(ref suggestions, value); }
    
    private HttpClient httpClient = new HttpClient();
    
    private async void UpdateSuggestions(string value)
    {
        Suggestions = (await GetSuggestionsAsync(value)).ToList();
    }

    private async Task<IEnumerable<string>> GetSuggestionsAsync(string text)
    {
        try
        {
            var response = await httpClient.GetAsync($"https://suggestqueries.google.com/complete/search?q={text}&hl={CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}&client=firefox");
            if (!response.IsSuccessStatusCode)
                return null;
            var result = JsonConvert.DeserializeObject<JArray>(await response.Content.ReadAsStringAsync());

            return result[1].Select(s => ((JValue)s)?.Value?.ToString());
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            return null;
        }
    }
}
