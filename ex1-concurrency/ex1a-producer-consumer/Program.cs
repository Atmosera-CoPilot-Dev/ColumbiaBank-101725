using System;
using System.Threading;
using System.Threading.Tasks;

// Producer class
public class Producer
{
  private readonly TaskCompletionSource<string> _taskCompletionSource;

  public Producer(TaskCompletionSource<string> taskCompletionSource)
  {
    _taskCompletionSource = taskCompletionSource;
  }

  public async Task ProduceAsync()
  {
    Console.WriteLine("Delivering message...");
    await Task.Delay(5000); // Simulate some long-ish running task
    _taskCompletionSource.SetResult("Message from producer...");
  }
}

// Consumer class
public class Consumer
{
  private readonly Task<string> _task;

  public Consumer(Task<string> task)
  {
    _task = task;
  }

  public async Task ConsumeAsync()
  {
    var message = await _task;
    Console.WriteLine(message);
  }
}

public class Program
{
  public static async Task Main(string[] args)
  {
    var taskCompletionSource = new TaskCompletionSource<string>();
    var producer = new Producer(taskCompletionSource);
    var consumer = new Consumer(taskCompletionSource.Task);

    var producerTask = producer.ProduceAsync();
    var consumerTask = consumer.ConsumeAsync();

    await Task.WhenAll(producerTask, consumerTask);
  }
}