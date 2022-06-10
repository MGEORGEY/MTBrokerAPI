namespace MTBrokerAPI.ControllerReturnTypes.UserMngt
{
    public class JwtTokenAndRolesCRT
    {
        public string JwtToken { get; set; }

        public List<string> Roles { get; set; }
    }
}
