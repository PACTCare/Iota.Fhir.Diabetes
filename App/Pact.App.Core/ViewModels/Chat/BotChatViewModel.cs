namespace Pact.App.Core.ViewModels.Chat
{
  using System;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.Linq;
  using System.Threading.Tasks;

  using Microsoft.Bot.Connector.DirectLine;

  using Pact.App.Core.Chat;
  using Pact.App.Core.Models;

  using Telerik.XamarinForms.ConversationalUI;

  using Xamarin.Essentials;
  using Xamarin.Forms;

  /// <summary>
  /// The florence chat view model.
  /// </summary>
  public class BotChatViewModel : ChatBaseViewModel
  {
    public BotChatViewModel(Bot florence, INavigation navigation, Author author)
      : base(navigation, author)
    {
      this.Florence = florence;
    }

    ~BotChatViewModel()
    {
      try
      {
        this.DirectLineManager.Dispose();

      }
      catch
      {
        // ignored
      }
    }

    private DirectLineManager DirectLineManager { get; set; }

    private Bot Florence { get; }

    public async void ChatItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
        this.TypingAuthors.Add(this.Florence);
        await this.DirectLineManager.SendMessageAsync(message.Text);
      }
    }

    public async Task InitAsync()
    {
      this.IsBusy = true;
      string conversationId;

      //if (!string.IsNullOrEmpty(await SecureStorage.GetAsync("FlorenceConversation")))
      //{
      //  conversationId = await SecureStorage.GetAsync("FlorenceConversation");
      //}
      //else
      //{
        conversationId = DirectLineManager.StartConversation(this.Florence.DirectLineSecret);
      //  await SecureStorage.SetAsync("FlorenceConversation", conversationId);
      //}

      this.DirectLineManager = new DirectLineManager(this.Florence.DirectLineSecret, conversationId);
      this.DirectLineManager.ActivitiesReceived += this.OnActivitiesReceived;

      this.Messages.Add(new TextMessage { Text = "Hey Florence!", Author = this.Author });

      this.IsBusy = false;
    }

    /// <summary>
    /// The on activities received.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="activitySet">
    /// The activitySet.
    /// </param>
    private void OnActivitiesReceived(object sender, IList<Activity> activitySet)
    {
      this.TypingAuthors.Clear();

      foreach (var activity in activitySet.Where(activity => activity.From.Id == this.Florence.Name && activity.Text != "Say \"hi\" if you'd like to chat."))
      {
        this.PushActivity(activity);
      }
    }
  }
}