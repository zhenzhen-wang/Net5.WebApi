namespace Hr.Resume.IService.JWT
{
   public interface IJwtAuthService
    {
        public string Authentication(string userName, string pwd);
    }
}
