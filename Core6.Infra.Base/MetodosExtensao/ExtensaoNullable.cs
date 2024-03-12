namespace Core6.Infra.Base.MetodosExtensao
{
    public static class ExtensaoNullable
    {
        public static bool uIsNull<T>(this T? value) where T : struct
        {
            return value == null;
        }
    }
}
