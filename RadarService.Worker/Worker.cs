using Microsoft.Extensions.Options;

namespace RadarService.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly WorkerOptions _workerOptions;
        public Worker(ILogger<Worker> logger, IOptions<WorkerOptions> workerOptions)
        {
            _logger = logger;
            _workerOptions = workerOptions.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //     var steps = _stepOptions.Steps;

            //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    await Task.Delay(10000, stoppingToken);
            //}

            foreach (var device in _workerOptions.Devices)
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                var client = new HttpClient(clientHandler) { BaseAddress = new Uri(device.BaseAddress) };

                var checkCommand = device.Commands.FirstOrDefault(x => x.Name.Equals("Check"));

                if (checkCommand == null) continue;

                await RunStepper(client, checkCommand.Steps, checkCommand.Steps.First());

                var foundStep = checkCommand.Steps.FirstOrDefault(x=>x.Name.Equals("CheckStatus"));

                if (foundStep == null) continue;
                
                if(foundStep.Response.Contains(foundStep.Result))



                foreach (var command in device.Commands)
                {

                    _logger.LogInformation($"device : {device} Worker Started!");
                    await RunStepper(client, command.Steps, command.Steps.First());
                    _logger.LogInformation($"device : {device} Worker Completed!");
                }
                await Task.Delay(10000, stoppingToken);
            }




        }

        private async Task RunStepper(HttpClient client, List<Step> steps, Step step)
        {
            try
            {

                if (step == null) return;

                if (step.Request.Type == "POST")
                {
                    var formData = step.Request.FormParameters.Select(x => new KeyValuePair<string, string>(x.Name, x.Value));
                    var result = await client.PostAsync(step.Request.RequestUrl, new FormUrlEncodedContent(formData));
                    if (!result.IsSuccessStatusCode) return;
                    step.Response = await result.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Step : {step.Name} Request : {step.Request.RequestUrl} Type : {step.Request.Type} response : {result.IsSuccessStatusCode}");
                    var foundStep = steps.Find(x => x.Id == step.NextStep);
                    if (foundStep == null) return;
                    await RunStepper(client, steps, foundStep);

                }
                if (step.Request.Type == "GET")
                {

                    var result = await client.GetAsync(step.Request.RequestUrl);
                    if (!result.IsSuccessStatusCode) return;
                    step.Response = await result.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Step : {step.Name} Request : {step.Request.RequestUrl} Type : {step.Request.Type} response : {result.IsSuccessStatusCode}");
                    if (step.NextStep == null) return;
                    var foundStep = steps.Find(x => x.Id == step.NextStep);
                    if (foundStep == null) return;
                    await RunStepper(client, steps, foundStep);

                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error: {message} {time}", e.Message, DateTimeOffset.Now);
                throw;
            }
        }


    }
}