namespace Core6.Infra.Base.Integracao
{
    public class UseHttpResponse<T>
    {
        public T Content { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public int? CodeRequest { get; set; }
        public int? Total { get; set; }
    }

    public class UseHttpResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public int? CodeRequest { get; set; }
        public int? Total { get; set; }
    }
}
