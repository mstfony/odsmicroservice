using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using odsmicroservice.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace odsmicroservice
{
    class Program
    {
        static void Main(string[] args)
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("DArchQueue", false, false, false, null);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    Connect().Wait();

                    var body = ea.Body.ToArray();
                    var data = Encoding.UTF8.GetString(body);
                    var sensorValue = JsonConvert.DeserializeObject<List<SensorValue>>(data);

                    hubConnection.InvokeAsync("PushValues", sensorValue);
                    foreach (var sensor in sensorValue)
                    {
                        Console.WriteLine($"Sensor : {sensor.SensorId} - V: {sensor.Value}");
                    }
                 
                };

                channel.BasicConsume("DArchQueue", true, consumer);
                Console.WriteLine("[X] Press Any Key To Exit");
                Console.ReadLine();
            }

        }

        public static HubConnection hubConnection;

        public static async Task Connect()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:44376/WebAPI/sensorvaluesign")
                .Build();

            await hubConnection.StartAsync();
        }

    }
}
