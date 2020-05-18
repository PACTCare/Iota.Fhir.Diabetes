namespace Pact.App.Core.Workers
{
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// The background worker service.
  /// </summary>
  public class BackgroundWorkerService
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="BackgroundWorkerService"/> class.
    /// </summary>
    public BackgroundWorkerService()
    {
      this.JobQueue = new Queue<object>();
      this.ObjectWorkers = new List<IObjectWorker>();
      this.TaskWorkers = new List<ITaskWorker>();

      this.BackgroundWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
      this.BackgroundWorker.DoWork += this.ProcessJobs;
      Task.Factory.StartNew(this.Poll);
    }

    /// <summary>
    /// Gets the background worker.
    /// </summary>
    private BackgroundWorker BackgroundWorker { get; }

    /// <summary>
    /// Gets the job queue.
    /// </summary>
    private Queue<object> JobQueue { get; }

    /// <summary>
    /// Gets the workers.
    /// </summary>
    private List<IObjectWorker> ObjectWorkers { get; }
    private List<ITaskWorker> TaskWorkers { get; }

    /// <summary>
    /// The add job.
    /// </summary>
    /// <param name="job">
    /// The job.
    /// </param>
    public void AddJob(object job)
    {
      this.JobQueue.Enqueue(job);

      if (!this.BackgroundWorker.IsBusy)
      {
        this.BackgroundWorker.RunWorkerAsync();
      }
    }

    /// <summary>
    /// The register worker.
    /// </summary>
    /// <param name="worker">
    /// The worker.
    /// </param>
    public void RegisterObjectWorker(IObjectWorker worker)
    {
      this.ObjectWorkers.Add(worker);
    }

    public void RegisterTaskWorker(ITaskWorker worker)
    {
      this.TaskWorkers.Add(worker);
    }

    /// <summary>
    /// The poll.
    /// </summary>
    private void Poll()
    {
      while (true)
      {
        if (!this.BackgroundWorker.IsBusy)
        {
          this.BackgroundWorker.RunWorkerAsync();
        }

        Thread.Sleep(60000);
      }
    }

    /// <summary>
    /// The process jobs.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private async void ProcessJobs(object sender, DoWorkEventArgs e)
    {
      foreach (var taskWorker in this.TaskWorkers)
      {
        taskWorker.ProcessTask();
      }

      while (this.JobQueue.Count > 0)
      {
        var job = this.JobQueue.Dequeue();
        foreach (var jobWorker in this.ObjectWorkers)
        {
          await jobWorker.ProcessObjectAsync(job);
        }
      }
    }
  }
}