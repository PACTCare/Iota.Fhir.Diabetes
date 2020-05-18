namespace Pact.App.Core.Views.DetailPages
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;

  using Hl7.Fhir.Model;

  using Newtonsoft.Json;

  using Pact.App.Core.Models;
  using Pact.App.Core.Services;
  using Pact.App.Core.ViewModels;
  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Iota.Services;

  using RestSharp;

  using Telerik.XamarinForms.Input;

  using Xamarin.Essentials;
  using Xamarin.Forms;
  using Xamarin.Forms.Xaml;

  using Task = System.Threading.Tasks.Task;

  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class SharedDataOverview : ContentPage
  {
    public SharedDataOverview()
    {
      this.InitializeComponent();
      this.BindingContext = new SharedDataOverviewViewModel();
    }

    private void AddDoctorClicked(object sender, EventArgs e)
    {
      ((SharedDataOverviewViewModel)this.BindingContext).AddDataShareAsync();
    }

    private void Button_OnClicked(object sender, EventArgs e)
    {
      if (sender is RadButton button && button.CommandParameter is MedicalInstitution institution)
      {
        ((SharedDataOverviewViewModel)this.BindingContext).OpenShareDialog(institution);
        this.SharePopup.IsOpen = true;
      }
    }

    private async void OkClicked(object sender, EventArgs e)
    {
      this.SharePopup.IsOpen = false;
      ((SharedDataOverviewViewModel)this.BindingContext).IsBusy = true;

      if (((SharedDataOverviewViewModel)this.BindingContext).SelectedInstitute.Name != "Dr. Mary Jane")
      {
        await Task.Factory.StartNew(() => Thread.Sleep(2000));
        await SecureStorage.SetAsync("MedicalInstitutions", JsonConvert.SerializeObject(((SharedDataOverviewViewModel)this.BindingContext).MedicalInstitutions));
        ((SharedDataOverviewViewModel)this.BindingContext).IsBusy = false;
        return;
      }

      var sharables = ((SharedDataOverviewViewModel)this.BindingContext).SelectedInstitute.SharedContent.ToList();
      var search = DependencyResolver.Resolve<ISearchRepository>();
      var resourceTracker = DependencyResolver.Resolve<IResourceTracker>();

      foreach (var sharable in sharables)
      {
        if (sharable.DisplayText.Contains("Immunization") || sharable.DisplayText.Contains("Vaccination"))
        {
          var immunizations = await search.FindResourcesByTypeAsync("Immunization");
          foreach (var immunization in immunizations)
          {
            if (sharable.IsShared)
            {
              var accessEntry = await resourceTracker.GetEntryAsync(immunization.Id);
              await this.ShareDataWithApiAsync(accessEntry.ResourceRoots.First(), accessEntry.Subscription.Key.Value);
            }
            else
            {
              await this.UnshareDataWithApiAsync(immunization.Id, "Immunization");
            }
          }
        }

        if (sharable.DisplayText.Contains("Conditions"))
        {
          var conditions = await search.FindResourcesByTypeAsync("Condition");
          foreach (var condition in conditions)
          {
            if (sharable.IsShared)
            {
              var accessEntry = await resourceTracker.GetEntryAsync(condition.Id);
              await this.ShareDataWithApiAsync(accessEntry.ResourceRoots.First(), accessEntry.Subscription.Key.Value);
            }
            else
            {
              await this.UnshareDataWithApiAsync(condition.Id, "Condition");
            }
          }
        }

        if (sharable.DisplayText.Contains("Dexcom G6") || sharable.DisplayText.Contains("Daily Glucose Report"))
        {
          var observations = await search.FindResourcesByTypeAsync("Observation");
          if (observations.FirstOrDefault(g => g is Observation observation && observation.Code.Text == "Dexcom Daily Glucose Measurement") is Observation dailyReportStream)
          {
            if (sharable.IsShared)
            {
              var glucoseAccess = await resourceTracker.GetEntryAsync(dailyReportStream.Id);
              await this.ShareDataWithApiAsync(glucoseAccess.ResourceRoots.First(), glucoseAccess.Subscription.Key.Value);
            }
            else
            {
              await this.UnshareDataWithApiAsync(dailyReportStream.Id, "Observation");
            }
          }
        }

        if (sharable.DisplayText.Contains("Profile") || sharable.DisplayText.Contains("Patient"))
        {
          var patient = await search.FindResourcesByTypeAsync("Patient");
          var patientAccess = await resourceTracker.GetEntryAsync(patient.First().Id);

          if (sharable.IsShared)
          {
            await this.ShareDataWithApiAsync(patientAccess.Subscription.MessageRoot.Value, patientAccess.Subscription.Key.Value);
          }
          else
          {
            await this.UnshareDataWithApiAsync(patient.First().Id, "Patient");
          }
        }
      }

      await SecureStorage.SetAsync("MedicalInstitutions", JsonConvert.SerializeObject(((SharedDataOverviewViewModel)this.BindingContext).MedicalInstitutions));
      ((SharedDataOverviewViewModel)this.BindingContext).IsBusy = false;
    }

    private async Task ShareDataWithApiAsync(string root, string key)
    {
      var client = new RestClient("http://pactfhir.azurewebsites.net/");
      var request = new RestRequest("api/iota/import", Method.POST) {RequestFormat = DataFormat.Json};
      request.AddJsonBody(new { ChannelKey = key, Root = root });

      var response = await client.ExecuteTaskAsync(request);
    }

    private async Task UnshareDataWithApiAsync(string resourceId, string resourceType)
    {
      var client = new RestClient("http://pactfhir.azurewebsites.net/");
      var request = new RestRequest($"api/fhir/{resourceType}/{resourceId}", Method.DELETE) { RequestFormat = DataFormat.Json };

      var response = await client.ExecuteTaskAsync(request);
    }
  }
}