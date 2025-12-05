using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FeedingApp.Models;
using FeedingApp.Services;

namespace FeedingApp.Views;

public partial class EditAnimalPage : ContentPage, IQueryAttributable
{
    private readonly IDatabaseService _db;
    private Animal _animal = new();

    public EditAnimalPage(IDatabaseService db)
    {
        InitializeComponent();

        _db = db;
        BindingContext = _animal; // kezdetben egy üres Animal
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        // Shell route paraméter: ?animalId=123
        if (query.TryGetValue("animalId", out var idValue))
        {
            var idString = Uri.UnescapeDataString(idValue?.ToString() ?? string.Empty);
            if (int.TryParse(idString, out var id) && id != 0)
            {
                _ = LoadExistingAnimalAsync(id);
                return;
            }
        }

        // új állat felvétele
        _animal = new Animal();
        BindingContext = _animal;
    }

    private async Task LoadExistingAnimalAsync(int id)
    {
        var existing = await _db.GetAnimalAsync(id);
        if (existing != null)
        {
            _animal = existing;
            BindingContext = _animal; // újra beállítjuk, hogy a mezők frissüljenek
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
