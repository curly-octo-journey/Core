namespace Core6.Infra.Base.Http
{
    #region Mensagem HTTP base
    public class UseHttpBaseMessage
    {
        public object Content { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
    #endregion

    #region Mensagem HTTP sucesso
    public class UseHttpSuccessMessage : UseHttpBaseMessage
    {
        public UseHttpSuccessMessage()
        {
            Success = true;
        }

        public int? CodeRequest { get; set; }
    }
    #endregion

    #region Mensagem HTTP [Readers] (Grids etc)
    public class UseHttpReaderMessage : UseHttpSuccessMessage
    {
        public int Total { get; set; }
    }
    #endregion

    #region Mensagem HTTP de erro
    public class UseHttpErrorMessage : UseHttpBaseMessage
    {
        public UseHttpErrorMessage()
        {
            Success = false;
        }

        public string Type { get; set; }
    }
    #endregion
}