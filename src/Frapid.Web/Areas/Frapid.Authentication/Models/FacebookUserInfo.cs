namespace Frapid.Authentication.Models
{
    public class FacebookUserInfo : IUserInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}