using Discount;
using Grpc.Core;

namespace DiscountServiceClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = new Channel("localhost:5000", ChannelCredentials.Insecure); // Assuming the server is running on localhost and port 5000
            var client = new DiscountService.DiscountServiceClient(channel);

            try
            {
                // Main loop for generating and using codes
                while (true)
                {
                    Console.WriteLine("Enter the number of codes to generate (or 'exit' to quit):");
                    var input = Console.ReadLine();

                    if (input?.ToLower() == "exit")
                    {
                        break;
                    }

                    if (int.TryParse(input, out int count))
                    {
                        // Step 1: Get the length of the discount codes from the user
                        Console.WriteLine("Enter the length of the discount code (7 or 8 characters):");
                        var lengthInput = Console.ReadLine();
                        if (int.TryParse(lengthInput, out int length) && (length == 7 || length == 8))
                        {
                            // Step 2: Generate discount codes
                            var generateResponse = await GenerateDiscountCodes(client, count, length);

                            if (generateResponse.Success)
                            {
                                Console.WriteLine("Discount codes generated successfully:");
                                foreach (var code in generateResponse.Codes)
                                {
                                    Console.WriteLine(code);
                                }

                                // Step 3: Use a generated discount code
                                Console.WriteLine("Enter a discount code to use (or 'exit' to quit):");
                                var codeToUse = Console.ReadLine();

                                if (codeToUse?.ToLower() == "exit")
                                {
                                    break;
                                }

                                var useResponse = await UseDiscountCode(client, codeToUse);

                                if (useResponse.Status == 0)
                                {
                                    Console.WriteLine($"Discount code '{codeToUse}' used successfully!");
                                }
                                else if (useResponse.Status == 1)
                                {
                                    Console.WriteLine($"Discount code '{codeToUse}' has already been used.");
                                }
                                else
                                {
                                    Console.WriteLine($"Invalid discount code '{codeToUse}'.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Failed to generate discount codes.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid length. Please enter 7 or 8 characters.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a number.");
                    }
                }
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"gRPC Error: {ex.Status.Detail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
            finally
            {
                await channel.ShutdownAsync();
            }
        }

        private static async Task<GenerateCodesResponse> GenerateDiscountCodes(DiscountService.DiscountServiceClient client, int count, int length)
        {
            try
            {
                var request = new GenerateCodeRequest
                {
                    Count = (uint)count,  // Number of codes to generate
                    Length = (uint)length // Length of the discount code
                };

                return await client.GenerateDiscountCodesAsync(request);
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error generating codes: {ex.Status.Detail}");
                return new GenerateCodesResponse { Success = false };
            }
        }

        private static async Task<UseDiscountCodeResponse> UseDiscountCode(DiscountService.DiscountServiceClient client, string code)
        {
            try
            {
                var request = new UseDiscountCodeRequest { Code = code };
                var response = await client.UseDiscountCodeAsync(request);
                return response;
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error using code: {ex.Status.Detail}");
                return new UseDiscountCodeResponse { Status = 2 };
            }
        }
    }
}