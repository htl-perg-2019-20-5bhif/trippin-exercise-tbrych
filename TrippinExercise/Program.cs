using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TrippinExercise
{
    class Program
    {
        static HttpClient HttpClient = new HttpClient() { BaseAddress = new Uri("https://services.odata.org/TripPinRESTierService/(S(npj54vjthjj0d1nvunlzt1sf))/") };

        static async Task Main(string[] args)
        {
            var fileContent = await File.ReadAllTextAsync("users.json");
            IEnumerable<UserJSON> users = JsonSerializer.Deserialize<IEnumerable<UserJSON>>(fileContent);

            foreach (var user in users)
            {
                var result = await HttpClient.GetAsync("People('" + user.UserName + "')");
                if (!result.IsSuccessStatusCode)
                {
                    //User not found: Insert user with the POST-Request
                    var newUser = new StringContent(JsonSerializer.Serialize(new UserTripping(user)), Encoding.UTF8, "application/json");
                    var postResult = await HttpClient.PostAsync("People", newUser);
                    try
                    {
                        postResult.EnsureSuccessStatusCode();
                        Console.WriteLine("User with the UserName " + user.UserName + " successfully inserted!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error when inserting user! Exception: " + e.GetType().ToString());
                    }
                }
                else
                {
                    Console.WriteLine("User with the UserName " + user.UserName + " found!");
                }
            }
        }
    }
}
