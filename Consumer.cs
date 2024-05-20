using Confluent.Kafka;
using Newtonsoft.Json;

var conf = new ConsumerConfig
{
    GroupId = "sales-group",
    BootstrapServers = "localhost:9092",
    AutoOffsetReset = AutoOffsetReset.Earliest
};


using var c = new ConsumerBuilder<Ignore, string>(conf).Build();
c.Subscribe("sales");

CancellationTokenSource cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true; // prevent the process from terminating.
    cts.Cancel();
};

try
{
    while (true)
    {
        try
        {
            var cr = c.Consume(cts.Token);

            var messageInfo = new 
            {
                Offset = cr.Offset,
                Key = cr.Key,
                Value = cr.Value,
                Timestamp = cr.Timestamp.UtcDateTime,
                Headers = cr.Headers.ToDictionary(h => h.Key, h => System.Text.Encoding.UTF8.GetString(h.GetValueBytes()))
            };
            string json = JsonConvert.SerializeObject(messageInfo, Formatting.Indented);
            Console.WriteLine(json);

        }
        catch (ConsumeException e)
        {
            Console.WriteLine($"Error occured: {e.Error.Reason}");
        }
    }
}
catch (OperationCanceledException)
{
    // Ensure the consumer leaves the group cleanly and final offsets are committed.
    c.Close();
}
