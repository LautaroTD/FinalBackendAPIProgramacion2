namespace FinalBackendAPIProgramacion2.DTO
{
    public class DTOLoginResponse
    {
        public DTOLoginResponse(string AccessToken)
        {
            this.AccessToken = AccessToken;
        }

        public string? AccessToken { get; set; }
    }
}
