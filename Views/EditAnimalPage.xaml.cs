using FeedingApp.Models;
using FeedingApp.Services;

namespace FeedingApp.Views;

[QueryProperty(nameof(AnimalId), "animalId")]
public partial class EditAnimalPage : ContentPage
{
    private readonly DatabaseService _db;
    private Animal _animal = new();

    // Shell route paraméter: ?animalId=123
    public int AnimalId { get; set; }

    public EditAnimalPage()
    {
        InitializeComponent();

        _db = new DatabaseService();
        BindingContext = _animal; // kezdetben egy üres Animal
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (AnimalId != 0)
        {
            // meglévő állat szerkesztése
            var existing = await _db.GetAnimalAsync(AnimalId);
            if (existing != null)
            {
                _animal = existing;
                BindingContext = _animal; // újra beállítjuk, hogy a mezők frissüljenek
            }
        }
        else
        {
            // új állat felvétele
            _animal = new Animal();
            BindingContext = _animal;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // egyszerű validáció: legyen név
        if (string.IsNullOrWhiteSpace(_animal.Name))
        {
            await DisplayAlert("Hiba", "A név megadása kötelező.", "OK");
            return;
        }

        await _db.SaveAnimalAsync(_animal);
        await Shell.Current.GoToAsync(".."); // vissza az előző oldalra (AnimalsPage)
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
