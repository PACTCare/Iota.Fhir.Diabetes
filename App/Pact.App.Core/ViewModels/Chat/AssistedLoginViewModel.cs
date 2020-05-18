namespace Pact.App.Core.ViewModels.Chat
{
  using System;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows.Input;

  using Microsoft.Bot.Connector.DirectLine;

  using Pact.App.Core.Services;
  using Pact.App.Core.Views.Main;

  using Tangle.Net.Entity;
  using Tangle.Net.Utils;

  using Telerik.XamarinForms.ConversationalUI;

  using Xamarin.Essentials;
  using Xamarin.Forms;

  using ZXing.Mobile;

  using Device = Xamarin.Forms.Device;

  /// <summary>
  /// The assisted login view model.
  /// </summary>
  public class AssistedLoginViewModel : ChatBaseViewModel
  {
    private Func<string, string, Task> DisplayAlert { get; }

    private string seed;

    private bool showSeedNotice;

    /// <inheritdoc />
    public AssistedLoginViewModel(Author author, INavigation navigation, Func<string, string, Task> displayAlert)
      : base(navigation, author)
    {
      this.DisplayAlert = displayAlert;
      this.Messages.Add(new TextMessage { Author = author, Text = "Hey Linda!" });
      this.NextStep("Start");
    }

    /// <summary>
    /// The steps.
    /// </summary>
    public enum Step
    {
      Start,

      Login,

      TermsAndConditions,

      TermsAndConditionsDisplay,

      CreateNew
    }

    /// <summary>
    /// The activity set.
    /// </summary>
    public Dictionary<Step, Activity> ActivitySet =>
      new Dictionary<Step, Activity>
        {
          {
            Step.Start,
            new Activity
              {
                Text = "Hi! I'm Linda. \nI can keep track of your or your family's health data. \nIs this the first time we chat with each other?",
                SuggestedActions = new SuggestedActions
                                     {
                                       Actions = new List<CardAction> { new CardAction { Title = "Yes" }, new CardAction { Title = "No" } }
                                     }
              }
          },
          {
            Step.TermsAndConditions,
            new Activity
              {
                Text =
                  "Cool. Please note that nobody else can access your data, unless you want them to. \nBefore we can start, let me show you my terms and privacy policy.",
                SuggestedActions = new SuggestedActions { Actions = new List<CardAction> { new CardAction { Title = "Go ahead" } } }
              }
          },
          {
            Step.TermsAndConditionsDisplay,
            new Activity
              {
                SuggestedActions = new SuggestedActions { Actions = new List<CardAction> { new CardAction { Title = "Accept" } } },
                Text =
                  "Introduction \n1.1 The following text is just for testing \n1.2 By using our website, you accept these terms and conditions in full; accordingly, if you disagree with these terms and conditions or any part of these terms and conditions, you must not use our website. \n1.3 If you use any of our website services, we will ask you to expressly agree to these terms and conditions. \n1.4 You must be at least 16 years of age to use our website; by using our website or agreeing to these terms and conditions, you warrant and represent to us that you are at least 16 years of age. \n1.5 In this policy, “we”, “us” and “our” refer to PACT Care BV. For more information about us, see Section 13. \n\nLimitations \n6.1 Nothing in these terms and conditions will: (a) limit or exclude any liability for death or personal injury resulting from negligence; (b) limit or exclude any liability for fraud or fraudulent misrepresentation; (c) limit any liabilities in any way that is not permitted under applicable law; or (d) exclude any liabilities that may not be excluded under applicable law. \n6.2 The limitations and exclusions of liability set out in this Section 6 and elsewhere in these terms and conditions: ...."
              }
          },
          {
            Step.Login,
            new Activity
              {
                Text = "Please enter your Password (Seed) to continue. It will not be shared and is only stored locally in an encrypted manner.",
                SuggestedActions = new SuggestedActions { Actions = new List<CardAction> { new CardAction { Title = "Scan via QR code" } } }
              }
          }
        };

    public string Seed
    {
      get => this.seed;
      set
      {
        this.seed = value;
        this.OnPropertyChanged();
      }
    }

    /// <summary>
    /// The seed notice.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public ICommand SeedNotice =>
      new Command(
        () =>
          {
            this.ShowSeedNotice = false;
            this.Seed = string.Empty;

            Application.Current.MainPage = new MasterDetailMainPage();
          });

    public bool ShowSeedNotice
    {
      get => this.showSeedNotice;
      set
      {
        this.showSeedNotice = value;
        this.OnPropertyChanged();
      }
    }

    public void ChatItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action != NotifyCollectionChangedAction.Add)
      {
        return;
      }

      if (!(e.NewItems[0] is TextMessage message))
      {
        return;
      }

      if (message.Author == this.Author)
      {
        this.NextStep(message.Text);
      }
    }

    /// <summary>
    /// The next step.
    /// </summary>
    /// <param name="clickedAction">
    /// The clicked action.
    /// </param>
    public async void NextStep(string clickedAction)
    {
      await this.Login(clickedAction);

      switch (clickedAction)
      {
        case "Yes":
          this.PushStep(Step.TermsAndConditions);
          break;
        case "No":
          this.PushStep(Step.Login);
          break;
        case "Go ahead":
          this.PushStep(Step.TermsAndConditionsDisplay);
          break;
        case "Accept":
          Application.Current.MainPage = new TabbedMainNavigationPage();
          //this.Seed = Tangle.Net.Entity.Seed.Random().Value;
          //this.ShowSeedNotice = true;
          break;
        case "Login":
          this.PushStep(Step.Login);
          break;
        case "Scan via QR code":
          try
          {
            var scanner = new MobileBarcodeScanner();
            var result = await scanner.Scan();

            scanner.Cancel();
            await this.Login(result.Text);
          }
          catch (Exception e)
          {
            await this.DisplayAlert(e.Message, e.StackTrace);
          }

          break;
        default:
          this.PushStep(Step.Start);
          break;
      }
    }

    private async Task Login(string seedInput)
    {
      if (InputValidator.IsTrytes(seedInput, 81))
      {
        this.IsBusy = true;
        await SeedImporter.ImportSeedAsync(new Seed(seedInput));
        this.IsBusy = false;

        await SecureStorage.SetAsync("LoggedIn", "True");

        this.Messages.Clear();
        Application.Current.MainPage = new TabbedMainNavigationPage();
      }
    }

    /// <summary>
    /// The push activity.
    /// </summary>
    /// <param name="step">
    /// The step.
    /// </param>
    private void PushStep(Step step)
    {
      Device.BeginInvokeOnMainThread(() =>
        {
          this.TypingAuthors.Add(
            new Author { Name = "Linda", Avatar = "https://florenceblob.blob.core.windows.net/thumbnails/final_verysmall2.png" });
        });

      Task.Delay(1000).ContinueWith(
        t =>
          {
            this.PushActivity(this.ActivitySet.First(a => a.Key == step).Value);
            Device.BeginInvokeOnMainThread(() =>
              {
                this.TypingAuthors.Clear();
              });
          });
    }
  }
}