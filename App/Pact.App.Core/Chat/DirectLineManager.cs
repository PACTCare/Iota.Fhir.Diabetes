namespace Pact.App.Core.Chat
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Diagnostics;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;

  using Microsoft.Bot.Connector.DirectLine;

  /// <inheritdoc />
  /// <summary>
  /// The direct line manager.
  /// </summary>
  public class DirectLineManager : IDisposable
  {
    /// <summary>
    /// The default polling interval in milliseconds.
    /// </summary>
    private const int DefaultPollingIntervalInMilliseconds = 60000;

    /// <summary>
    /// The _background worker.
    /// </summary>
    private readonly BackgroundWorker backgroundWorker;

    /// <summary>
    /// The _polling interval in milliseconds.
    /// </summary>
    private int pollingIntervalInMilliseconds;

    /// <summary>
    /// The _synchronization context.
    /// </summary>
    private SynchronizationContext synchronizationContext;

    /// <summary>
    /// The _watermark.
    /// </summary>
    private string watermark;

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectLineManager"/> class. 
    /// Constructor.
    /// </summary>
    /// <param name="directLineSecret">
    /// The Direct Line secret associated with the bot.
    /// </param>
    /// <param name="conversationId">
    /// The conversation Id.
    /// </param>
    public DirectLineManager(string directLineSecret, string conversationId = null)
    {
      if (string.IsNullOrEmpty(directLineSecret))
      {
        throw new ArgumentNullException(nameof(directLineSecret));
      }

      this.backgroundWorker = new BackgroundWorker();
      this.backgroundWorker.DoWork += this.RunPollMessagesLoopAsync;
      this.backgroundWorker.RunWorkerCompleted += BackgroundWorkerDone;
      this.backgroundWorker.WorkerSupportsCancellation = true;

      this.DirectLineSecret = directLineSecret;
      this.ConversationId = conversationId;
    }

    /// <summary>
    /// An event that is fired when new messages are received.
    /// </summary>
    public event EventHandler<IList<Activity>> ActivitiesReceived;

    /// <summary>
    /// Gets the direct line secret.
    /// </summary>
    private string DirectLineSecret { get; }

    /// <summary>
    /// Gets or sets the conversation id.
    /// </summary>
    private string ConversationId { get; set; }

    public static string StartConversation(string directLineSecret)
    {
      using (var directLineClient = new DirectLineClient(directLineSecret))
      {
        return directLineClient.Conversations.StartConversation().ConversationId;
      }
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public void Dispose()
    {
      this.backgroundWorker.DoWork -= this.RunPollMessagesLoopAsync;
      this.backgroundWorker.RunWorkerCompleted -= BackgroundWorkerDone;
      this.backgroundWorker.CancelAsync();
      this.backgroundWorker.Dispose();
    }

    /// <summary>
    /// Polls for new messages (activities).
    /// </summary>
    /// <param name="conversationId">
    /// The ID of the conversation.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task PollMessagesAsync(string conversationId = null)
    {
      if (!string.IsNullOrEmpty(conversationId) || !string.IsNullOrEmpty(this.ConversationId))
      {
        conversationId = string.IsNullOrEmpty(conversationId) ? this.ConversationId : conversationId;
        ActivitySet activitySet = null;

        using (var directLineClient = new DirectLineClient(this.DirectLineSecret))
        {
          directLineClient.Conversations.ReconnectToConversation(conversationId);
          activitySet = await directLineClient.Conversations.GetActivitiesAsync(conversationId, this.watermark);
        }

        if (activitySet != null)
        {
          this.watermark = activitySet.Watermark;

          var activities = (from activity in activitySet.Activities select activity).ToList();

          if (this.synchronizationContext != null)
          {
            this.synchronizationContext.Post((o) => this.ActivitiesReceived?.Invoke(this, activities), null);
          }
          else
          {
            this.ActivitiesReceived?.Invoke(this, activities);
          }
        }
      }
    }

    /// <summary>
    /// Sends the given message to the bot.
    /// </summary>
    /// <param name="messageText">
    /// The message to send.
    /// </param>
    /// <returns>
    /// Message ID if successful. Null otherwise.
    /// </returns>
    public async Task<string> SendMessageAsync(string messageText)
    {
      ResourceResponse resourceResponse;
      using (var directLineClient = new DirectLineClient(this.DirectLineSecret))
      {
        if (string.IsNullOrEmpty(this.ConversationId))
        {
          this.ConversationId = StartConversation(this.DirectLineSecret);
        }
        else
        {
          directLineClient.Conversations.ReconnectToConversation(this.ConversationId);
        }

        resourceResponse = await directLineClient.Conversations.PostActivityAsync(
                             this.ConversationId,
                             new Activity
                               {
                                 From = new ChannelAccount(
                                   $"{this.ConversationId}_direct",
                                   $"{this.ConversationId}_direct"),
                                 Type = ActivityTypes.Message,
                                 Text = messageText
                               });
      }

      if (resourceResponse == null)
      {
        return null;
      }

      this.StartPolling();
      return resourceResponse.Id;
    }

    /// <summary>
    /// Starts polling for the messages.
    /// </summary>
    /// <param name="pollingIntervalInMilliseconds">
    /// The polling interval in milliseconds.
    /// </param>
    /// <returns>
    /// True, if polling was started. False otherwise (e.g. if already running).
    /// </returns>
    public bool StartPolling(int pollingIntervalInMilliseconds = DefaultPollingIntervalInMilliseconds)
    {
      if (this.backgroundWorker.IsBusy)
      {
        Debug.WriteLine("Already polling");
        return false;
      }

      this.synchronizationContext = SynchronizationContext.Current;
      this.pollingIntervalInMilliseconds = pollingIntervalInMilliseconds;
      this.backgroundWorker.RunWorkerAsync();
      return true;
    }

    /// <summary>
    /// Stops polling for the messages.
    /// </summary>
    public void StopPolling()
    {
      try
      {
        this.backgroundWorker.CancelAsync();
      }
      catch (InvalidOperationException e)
      {
        Debug.WriteLine($"Failed to stop polling: {e.Message}");
      }
    }

    /// <summary>
    /// The background worker done.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private static void BackgroundWorkerDone(object sender, RunWorkerCompletedEventArgs e)
    {
      Debug.WriteLine("Background worker finished");
    }

    /// <summary>
    /// The run poll messages loop async.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private async void RunPollMessagesLoopAsync(object sender, DoWorkEventArgs e)
    {
      while (!e.Cancel)
      {
        await this.PollMessagesAsync();
        Thread.Sleep(this.pollingIntervalInMilliseconds);
      }
    }
  }
}