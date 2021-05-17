namespace Application.Common.Models
{
    public class DatabaseConnection
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }

        public override string ToString() => $"User ID={UserName};Password={Password};Host={Host};Port={Port};Database={Database};Pooling=true;";
    }
}