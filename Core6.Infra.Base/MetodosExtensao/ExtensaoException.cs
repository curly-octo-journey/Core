namespace Core6.Infra.Base.MetodosExtensao
{
    public static class ExtensaoException
    {
        public static T uExceptionSeNull<T>(this T obj, string msg) where T : class
        {
            if (obj == null)
                throw new Exception(msg);

            return obj;
        }

        public static T uExceptionSeNull<T>(this T obj, string msg, params object[] args) where T : class
        {
            if (obj == null)
                throw new Exception(string.Format(msg, args));

            return obj;
        }

        public static string uTratar(this Exception exp)
        {
            var mensagem = exp.Message;
            var inner = exp.InnerException;
            while (inner != null)
            {
                mensagem += "\nMais detalhes: " + inner.Message;
                inner = inner.InnerException;
            }
            return mensagem;
        }

        public static string uTratarUltimo(this Exception exp)
        {
            var mensagem = exp.Message;
            var inner = exp.InnerException;
            while (inner != null)
            {
                mensagem = inner.Message;
                inner = inner.InnerException;
            }
            return mensagem;
        }

        public static Exception uRecuperarUltimaExcessao(this Exception exp)
        {
            while (exp.InnerException != null)
            {
                exp = exp.InnerException;
            }

            return exp;
        }

        public static IEnumerable<T> uExceptionSeVazio<T>(this IEnumerable<T> obj, string msg, params object[] args) where T : class
        {
            if (obj.uVazio())
                throw new Exception(string.Format(msg, args));

            return obj;
        }

        public static string uMsgCompleta(this Exception e)
        {
            if (e == null)
                return null;

            var msgExp = e.Message;

            if (e.InnerException != null)
                msgExp = string.Format("{0}\n{1}", msgExp, e.InnerException.uMsgCompleta());

            return msgExp;
        }

        public static T uExceptionSeTrue<T>(this T obj, Func<T, bool> cond, string msg, params object[] args)
        {
            if (!cond.Invoke(obj)) return obj;
            throw new Exception(string.Format(msg, args));
        }

        public static T uExceptionSeFalse<T>(this T obj, Func<T, bool> cond, string msg, params object[] args)
        {
            if (cond.Invoke(obj)) return obj;
            throw new Exception(string.Format(msg, args));
        }

        public static void uExceptionSeTrue(this bool obj, string msg, params object[] args)
        {
            if (!obj) return;
            throw new Exception(string.Format(msg, args));
        }

        public static void uExceptionSeFalse(this bool obj, string msg, params object[] args)
        {
            if (obj) return;
            throw new Exception(string.Format(msg, args));
        }
    }
}
