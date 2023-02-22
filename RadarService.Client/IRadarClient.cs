namespace RadarService.Client
{
    public interface IRadarClient
    {
      
        public Task<bool> GetStatus();

        public Task SendCommand(bool command);
    }
}