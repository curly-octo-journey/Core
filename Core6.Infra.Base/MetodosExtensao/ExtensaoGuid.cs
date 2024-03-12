namespace Core6.Infra.Base.MetodosExtensao
{
    public static class ExtensaoGuid
    {
        public static string uToOracle(this Guid guid)
        {
            return BitConverter.ToString(guid.ToByteArray()).Replace("-", "");
        }
    }
}
