using Hl7.Fhir.Model;
using Pact.App.Core.Models;

namespace Pact.App.Core.Views.DetailPages
{
  using Telerik.XamarinForms.Input;

  using Xamarin.Forms;
  using Xamarin.Forms.Xaml;

  /// <summary>
  /// The patient resource detail page.
  /// </summary>
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class PatientDetailPage : ContentPage
  {
    public PatientDetailPage(Patient patient)
    {
      this.InitializeComponent();
      this.PatientData.Source = new PatientDetail(patient);
      this.PatientData.RegisterEditor(nameof(PatientDetail.Firstname), EditorType.TextEditor);
      this.PatientData.RegisterEditor(nameof(PatientDetail.Lastname), EditorType.TextEditor);
      this.PatientData.RegisterEditor(nameof(PatientDetail.Postcode), EditorType.TextEditor);
      this.PatientData.RegisterEditor(nameof(PatientDetail.Street), EditorType.TextEditor);
      this.PatientData.RegisterEditor(nameof(PatientDetail.City), EditorType.TextEditor);
      this.PatientData.RegisterEditor(nameof(PatientDetail.Country), EditorType.TextEditor);
      this.PatientData.RegisterEditor(nameof(PatientDetail.Birthday), EditorType.DateEditor);
    }
  }
}