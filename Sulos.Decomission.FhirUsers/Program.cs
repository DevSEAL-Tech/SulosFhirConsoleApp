//using System;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text.Json;
//using System.Threading.Tasks;
//using Newtonsoft.Json.Linq;
//using static SulosFhirConsole.Model;

using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Hl7.Fhir;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Sulos.Api.Common.Fhir;
using Sulos.Hospice.Care.Core.Common.Cqrs;
using Sulos.Hospice.Care.Core.Common.Cqrs.Commands;
using Sulos.Hospice.Care.Core.Common.Cqrs.Queries;
using Sulos.Hospice.Care.Core.Common.Fhir;
using Sulos.Hospice.Care.Core.Common.Fhir.Searching;
using Sulos.Hospice.Care.Core.Common.KeyVault;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;


namespace SulosFhirConsole
{
    internal class Program
    {
        /*private static readonly string clientId = "your-client-id";
        private static readonly string clientSecret = "your-client-secret";
        private static readonly string tenantId = "your-tenant-id";
        private static readonly string patientId = "patient-id-or-upn"; */

        static async Task Main(string[] args)
        {
            var settings = new FhirClientSettings
            {
                //Timeout = 0,
                PreferredFormat = ResourceFormat.Json,
                VerifyFhirVersion = true,
                ReturnPreference = ReturnPreference.Minimal
            };
            // Create a custom HttpMessageHandler (if needed)
            var httpMessageHandler = new HttpClientHandler();
            var client = new FhirClient("https://devsulos.azurehealthcareapis.com",settings);
            
            var searchParams = new SearchParams();
            //.LimitTo(10)   // Limit to 1 result for efficiency
            //.SummaryOnly();

            var patients = await client.SearchAsync<Patient>(searchParams);
            
            Console.WriteLine($"Total Patient :{patients.Entry.Count}");
            int patientNumber = 0;
            foreach (var patient in patients.Entry)
            {
                Console.WriteLine($"- Entry {patientNumber,3}: {patient.FullUrl}");
                if (patient.Resource != null)
                {
                    Patient patient1 = (Patient)patient.Resource;
                    System.Console.WriteLine($" - {patient1.Id,20}");
                    if (patient1.Name.Count > 0)
                    {
                        System.Console.WriteLine($" - Name : {patient1.Name[0].ToString()}");
                    }
                }

                patientNumber++;
            }




            //var fhirSearchParams = new FhirSearchParamsBuilder()

            //.Build();
            
            /*HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.Services.Configure<FhirConfigOptions>(builder.Configuration.GetSection(FhirConfigOptions.ConfigSection));
            builder.Services.Configure<KeyVaultOptions>(builder.Configuration.GetSection(KeyVaultOptions.ConfigSection));
            builder.Services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssemblies(typeof(Program).Assembly);
            });
            builder.Services.TryAddTransient<IQueryBus, QueryBus>();
            builder.Services.TryAddTransient<ICommandBus, CommandBus>();
            builder.Services.TryAddTransient<ICqrsBus>(p =>
            {
                var queryBus = p.GetRequiredService<IQueryBus>();
                var commandBus = p.GetRequiredService<ICommandBus>();
                var logger = p.GetRequiredService<ILogger<LoggingCqrsBus>>();
                var cqrsBus = new CqrsBus(queryBus, commandBus);
                return new LoggingCqrsBus(cqrsBus, logger);
            });
            builder.Services.TryAddTransient<IFhirClientOptionsFactory, FhirClientOptionsFactory>();

            using IHost host = builder.Build();

            //var processingService = host.Services.GetService<IProcessingService>();
            //processingService.Process();

            //var client = new SecretClient(vaultUri: new Uri("https://kv-dev-hospice-app-denb.vault.azure.net/"), credential: new DefaultAzureCredential());
            //KeyVaultSecret secret = client.GetSecret("mysulos-fhir-url");
            //Console.WriteLine(secret.Value);
            //*/


            //var graphClient = GetAuthenticatedGraphClient();
            //await ReadPatientDataAsync(graphClient, patientId);

        }

       /* private static GraphServiceClient GetAuthenticatedGraphClient()
        {
            var clientApp = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
                .Build();

            var authProvider = new DelegateAuthenticationProvider(async (requestMessage) =>
            {
                var result = await clientApp.AcquireTokenForClient(new[] { "https://graph.microsoft.com/.default" })
                                             .ExecuteAsync();

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            });

            return new GraphServiceClient(authProvider);
        }

        private static async Task ReadPatientDataAsync(GraphServiceClient graphClient, string patientId)
        {
            try
            {
                // Example: Reading patient data
                var patientData = await graphClient.Users.GetAsync(); 

                //// Do something with patientData
                //Console.WriteLine($"Patient Name: {patientData.DisplayName}");
                // Do something with patientData
                //Console.WriteLine($"Patient Name: {patientData.Value}");
            }
            catch (ServiceException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }*/
    }
}
