# Discount Service gRPC client for testing the gRPC service

The client can interact with the server to:
- Generate discount codes.
- Use a discount code.

## Building and Running the gRPC client

Requirements:
- Running the gRPC DiscountService server on port 5000

Clone the Repository:
git clone cd DiscountServiceClient

Restore Dependencies:
dotnet restore

Build the Project:
dotnet build

Run the Server:
dotnet run

Using the Client:
The application will prompt the user to specify the number of code to generate followed by the number of lenght of the codes.
After valid numbers have been entered the user is prompted to to use one of the generated codes. This process repeats until the user types exit.
