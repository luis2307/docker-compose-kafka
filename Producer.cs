using Confluent.Kafka;


var config = new ProducerConfig
{
    BootstrapServers = "localhost:9092",
    ClientId = "KafkaProducerApp"
};

using var producer = new ProducerBuilder<string, string>(config).Build();

Console.WriteLine("EPosting messages to Kafka in the topic 'Sales'.");
Console.WriteLine("Enter the message to send or type 'exit' to finish.");

while (true)
{
    Console.Write("Enter your message:");
    var message = Console.ReadLine();

    if (message.Contains("exit"))
    {
        break;
    }

    var key = "key-" + DateTime.Now.Ticks; // Genera una clave única basada en el timestamp actual 
    var headers = new Headers
    {
        { "SentTime", System.Text.Encoding.UTF8.GetBytes(DateTime.UtcNow.AddDays(-1).ToString()) },
        { "Sender", System.Text.Encoding.UTF8.GetBytes( "KafkaProducerApp")}
    };

    try
    {
        var result = await producer.ProduceAsync("Ventas", new Message<string, string>
        {
            Key = key,
            Value = message,
            Headers = headers
        });

        Console.WriteLine($"Message sent to topic {result.Topic}, partition {result.Partition}, offset {result.Offset}");
    }
    catch (ProduceException<string, string> e)
    {
        Console.WriteLine($"Error sending message: {e.Error.Reason}");
    }
}

producer.Flush(TimeSpan.FromSeconds(10));
Console.WriteLine("All messages have been sent and confirmed.");


/*
for (int i = 0; i < 10; i++)
{
    var key = $"id-{i}";
    var value = $"Mensaje de venta {i}";

    try
    {
        var result = await producer.ProduceAsync("Ventas", new Message<string, string> { Key = key, Value = value });
        Console.WriteLine($"Mensaje enviado al topic {result.Topic}, partición {result.Partition}, offset {result.Offset}");
    }
    catch (ProduceException<string, string> e)
    {
        Console.WriteLine($"Error al enviar mensaje: {e.Error.Reason}");
    }
}

producer.Flush(TimeSpan.FromSeconds(10));
Console.WriteLine("All messages have been sent and confirmed.");*/
