namespace WormCat.Library.Models
{
    public class ErrorCodeModel
    {
        public ErrorCodeModel(int id, string message)
        {
            Id = id;
            Message = message;
        }

        public int Id { get; protected set; }
        public string Message { get; protected set; }
    }
}
