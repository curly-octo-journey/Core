namespace Core6.Infra.Base.Criptografia
{
    public static class CriptografiaE2
    {
        public static string Criptografar(string asOriginal)
        {
            if (asOriginal == null)
            {
                return "";
            }

            string lsfinal;
            string senha;
            string lsChar;

            int liasc;
            int ncar;
            int i;

            senha = "";

            ncar = asOriginal.Length;
            for (i = 0; i < ncar; i = i + 1)
            {
                //System.out.println(asOriginal.substring(ncar - i - 1,  ncar - i));
                senha = senha + asOriginal.Substring(ncar - i - 1, 1); //subs (asOriginal,ncar - i + 1,1)
                senha = senha + asOriginal.Substring(ncar - i - 1, 1); //subs (asOriginal,ncar - i + 1,1)
            }

            lsfinal = "";
            ncar = senha.Length;
            for (i = 0; i < ncar; i++)
            {
                lsChar = senha.Substring(i, 1); //Asc( Mid( lsSenha, i, 1 ))
                liasc = lsChar[0];

                if (liasc == 90)
                {
                    liasc = 65;
                }
                else
                {
                    if (liasc == 122)
                    {
                        liasc = 97;
                    }
                    else
                    {
                        liasc = liasc + 1;
                    }
                }
                lsfinal = lsfinal + (char)liasc;
            }
            return lsfinal;
        }

        public static string Descriptografar(string asOriginal)
        {
            if (asOriginal == null)
            {
                return "";
            }

            string lsfinal;
            string lssenha;
            string lsChar;

            int liasc;
            int ncar;
            int i;

            lssenha = "";

            ncar = asOriginal.Length;
            for (i = 0; i < ncar; i = i + 2)
            {
                lssenha = lssenha + asOriginal[ncar - i - 1];//asOriginal.Substring(ncar - i - 1, 1);
            }

            ncar = lssenha.Length;
            lsfinal = "";
            for (i = 0; i < ncar; i = i + 1)
            {
                lsChar = lssenha.Substring(i, 1);
                liasc = lsChar[0];
                if (liasc == 65)
                {
                    liasc = 90;
                }
                else
                {
                    if (liasc == 97)//z
                    {
                        liasc = 122;
                    }
                    else
                    {
                        liasc = liasc - 1;
                    }
                }
                lsfinal = lsfinal + (char)liasc;
            }
            return lsfinal;
        }
    }
}