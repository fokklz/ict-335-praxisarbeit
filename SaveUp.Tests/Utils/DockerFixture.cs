using System.Diagnostics;

namespace SaveUp.Tests.Utils
{
    public class DockerFixture : IDisposable
    {
        private readonly HttpClient _client;
        public DockerFixture()
        {
            _client = new HttpClient();
            ExecuteCommand("docker-compose -f ..\\..\\..\\docker-compose.yml up -d");
            WaitForService().GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            ExecuteCommand("docker-compose -f ..\\..\\..\\docker-compose.yml down --volumes --remove-orphans");
        }

        private void ExecuteCommand(string command)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    //RedirectStandardOutput = true,
                    //RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            /* ways to read the output (not needed for now)
            string result = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();*/

            process.WaitForExit();
        }

        /// <summary>
        /// Wait for the Test-Environment to become available
        /// </summary>
        /// <returns>Nothing</returns>
        /// <exception cref="TimeoutException">Time span exceded</exception>
        private async Task WaitForService()
        {
            var retryCount = 30;
            var delay = TimeSpan.FromSeconds(1);

            for (int i = 0; i < retryCount; i++)
            {
                if (await IsServiceAvailable()) return;

                Console.WriteLine($"Waiting for service... attempt {i + 1}");
                await Task.Delay(delay);
            }

            throw new TimeoutException("Service did not become available in time.");
        }

        /// <summary>
        /// Check if the Test-Environment is available yet
        /// </summary>
        /// <returns>True if available</returns>
        private async Task<bool> IsServiceAvailable()
        {
            try
            {
                var response = await _client.GetAsync("http://localhost:9000");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }

    [CollectionDefinition("Docker Collection")]
    public class DockerCollection : ICollectionFixture<DockerFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
