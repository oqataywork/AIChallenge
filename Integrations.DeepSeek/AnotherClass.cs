// using System.Text;
//
// namespace Integrations.DeepSeek;
//
// internal static class AnotherClass
// {
//     public static async Task Main()
//     {
//         SetUpConsole();
//
//         string? apiKey = GetDeepSeekApiKey();
//
//         if (string.IsNullOrWhiteSpace(apiKey))
//         {
//             return;
//         }
//
//         var ai = new DeepSeekAiClient(apiKey);
//
//         Console.WriteLine("=== DeepSeek Console Chat ===");
//         Console.WriteLine("Type 'exit' or 'quit' to leave.\n");
//
//         while (true)
//         {
//             Console.ForegroundColor = ConsoleColor.Cyan;
//             Console.Write("You: ");
//             Console.ResetColor();
//
//             string userInput = Console.ReadLine() ?? string.Empty;
//
//             if (string.IsNullOrWhiteSpace(userInput))
//             {
//                 continue;
//             }
//
//             if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
//                 userInput.Equals("quit", StringComparison.OrdinalIgnoreCase))
//             {
//                 Console.WriteLine("Goodbye!");
//
//                 break;
//             }
//
//             Console.ForegroundColor = ConsoleColor.Yellow;
//             Console.WriteLine("AI is thinking...\n");
//             Console.ResetColor();
//
//             string response = await ai.Send(userInput);
//
//             Console.ForegroundColor = ConsoleColor.Green;
//             Console.WriteLine("DeepSeek:");
//             Console.ResetColor();
//
//             Console.WriteLine(response);
//             Console.WriteLine();
//         }
//     }
//
//     private static void SetUpConsole()
//     {
//         Console.OutputEncoding = Encoding.UTF8;
//         Console.InputEncoding = Encoding.UTF8;
//     }
//
//     private static string? GetDeepSeekApiKey()
//     {
//         string? apiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY");
//
//         if (string.IsNullOrWhiteSpace(apiKey))
//         {
//             Console.WriteLine("Enter an DeepSeek API Key:");
//             apiKey = Console.ReadLine();
//         }
//
//         return apiKey;
//     }
// }