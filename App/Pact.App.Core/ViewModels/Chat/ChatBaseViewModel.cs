namespace Pact.App.Core.ViewModels.Chat
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;

  using Microsoft.Bot.Connector.DirectLine;

  using Newtonsoft.Json;

  using Pact.App.Core.Chat;

  using Telerik.XamarinForms.ConversationalUI;

  using Xamarin.Forms;

  public class ChatBaseViewModel : BaseViewModel
  {
    public ChatBaseViewModel(INavigation navigation, Author author)
      : base(navigation)
    {
      this.Author = author;
      this.Messages = new ObservableCollection<ChatItem>();
      this.TypingAuthors = new ObservableCollection<Author>();
    }

    public ObservableCollection<ChatItem> Messages { get; set; }

    public ObservableCollection<Author> TypingAuthors { get; set; }

    protected Author Author { get; }

    private CardPickerContext pickerContext;

    public CardPickerContext PickerContext
    {
      get => this.pickerContext;
      set
      {
        this.pickerContext = value;
        this.OnPropertyChanged();
      }
    }

    private bool isPickerVisible;

    public bool IsPickerVisible
    {
      get => this.isPickerVisible;
      set
      {
        this.isPickerVisible = value;
        this.OnPropertyChanged();
      }
    }

    protected void PushActivity(Activity activity)
    {
      Device.BeginInvokeOnMainThread(
        () =>
          {
            if (this.ActivityHasBeenDisplayedAsHeroCard(activity))
            {
              return;
            }

            this.Messages.Add(
              new TextMessage
                {
                  Text = activity.Text,
                  Author = new Author { Name = "Florence", Avatar = "https://florenceblob.blob.core.windows.net/thumbnails/final_verysmall2.png" }
                });

            if (activity.SuggestedActions != null && activity.SuggestedActions.Actions.Count > 0)
            {
              this.AddCardActions(activity.SuggestedActions.Actions);
            }
          });
    }

    private bool ActivityHasBeenDisplayedAsHeroCard(Activity activity)
    {
      if (activity.Attachments != null && activity.Attachments.Count > 0)
      {
        foreach (var attachment in activity.Attachments)
        {
          switch (attachment.ContentType)
          {
            case "application/vnd.microsoft.card.hero":
              var heroCard = JsonConvert.DeserializeObject<HeroCard>(attachment.Content.ToString());
              var title = string.IsNullOrEmpty(heroCard.Title) ? string.Empty : heroCard.Title + ": ";

              //this.Messages.Add(
              //  new PickerItem
              //    {
              //      Context = new CardPickerContext
              //                  {
              //                    Cards = new List<CardContext>
              //                              {
              //                                new ImageCardContext
              //                                  {
              //                                    Description = $"{title}{heroCard.Text}",
              //                                    Image = heroCard.Images != null ? heroCard.Images[0].Url : string.Empty
              //                                  }
              //                              }
              //                  }
              //    });

              this.Messages.Add(
                new PickerItem
                  {
                    Context = new MicrosoftCardHeroPickerContext
                                {
                                  Description = $"{title}{heroCard.Text}",
                                  ImageUri = heroCard.Images != null ? heroCard.Images[0].Url : string.Empty
                    }
                  });

              if (heroCard.Buttons != null && heroCard.Buttons.Count > 0)
              {
                this.AddCardActions(heroCard.Buttons);
              }

              break;
            default:
              this.Messages.Add(
                new TextMessage
                  {
                    Text = activity.Text,
                    Author = new Author { Name = "Florence", Avatar = "https://florenceblob.blob.core.windows.net/thumbnails/final_verysmall2.png" }
                  });
              break;
          }
        }

        return true;
      }

      return false;
    }

    private void AddCardActions(IEnumerable<CardAction> cardActions)
    {
      var suggestedActionPickerContext = new SuggestedActionPickerContext
                                           {
                                             Cards = cardActions.Select(
                                               a => new SuggestedActionCardContext
                                                      {
                                                        Text = a.Title,
                                                        Command = new Command(
                                                          () =>
                                                            {
                                                              this.PickerContext = null;
                                                              this.IsPickerVisible = false;

                                                              this.Messages.Add(new TextMessage { Text = a.Title, Author = this.Author });

                                                            })
                                                      }).ToList()
                                           };

      this.PickerContext = suggestedActionPickerContext;
      this.IsPickerVisible = true;
    }
  }
}