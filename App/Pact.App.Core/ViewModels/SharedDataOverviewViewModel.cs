namespace Pact.App.Core.ViewModels
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;

  using Newtonsoft.Json;

  using Pact.App.Core.Models;

  using Xamarin.Essentials;

  using ZXing.Mobile;

  public class SharedDataOverviewViewModel : BaseViewModel
  {
    public SharedDataOverviewViewModel()
    {
      var institutions = SecureStorage.GetAsync("MedicalInstitutions").Result;
      if (string.IsNullOrEmpty(institutions))
      {
        this.MedicalInstitutions = new ObservableCollection<MedicalInstitution>(
          new List<MedicalInstitution>
            {
              new MedicalInstitution
                {
                  Name = "Dr. Mary Jane",
                  Endpoint = "http://pactfhir.azurewebsites.net/api",
                  SharedContent = new List<Sharable>
                                    {
                                      new Sharable { DisplayText = "Profile", IsShared = false },
                                      new Sharable { DisplayText = "Vaccinations", IsShared = false },
                                      new Sharable { DisplayText = "Conditions", IsShared = false },
                                      new Sharable { DisplayText = "Daily Glucose Report", IsShared = false }
                                    }
                }
            });
      }
      else
      {
        this.MedicalInstitutions =
          new ObservableCollection<MedicalInstitution>(JsonConvert.DeserializeObject<List<MedicalInstitution>>(institutions));
      }
    }

    public ObservableCollection<MedicalInstitution> MedicalInstitutions { get; set; }

    public MedicalInstitution SelectedInstitute { get; set; }
    public ObservableCollection<Sharable> Shareables { get; set; }

    public async void AddDataShareAsync()
    {
      var scanner = new MobileBarcodeScanner();
      var result = await scanner.Scan();

      scanner.Cancel();

      if (result == null || string.IsNullOrEmpty(result.Text))
      {
        return;
      }

      var medicalInstitution = JsonConvert.DeserializeObject<MedicalInstitution>(result.Text);
      if (this.MedicalInstitutions.All(m => m.Name != medicalInstitution.Name))
      {
        this.MedicalInstitutions.Add(medicalInstitution);
        await SecureStorage.SetAsync("MedicalInstitutions", JsonConvert.SerializeObject(this.MedicalInstitutions));
      }
    }

    public void OpenShareDialog(MedicalInstitution medicalInstitution)
    {
      this.SelectedInstitute = medicalInstitution;
    }
  }
}