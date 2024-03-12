using System.Reflection;

namespace Core6.Infra.Base.Aspose
{
    public static class AsposeUtil
    {
        #region RecuperarLicenca
        public static MemoryStream RecuperarLicenca()
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string resourceName = "Core6.Infra.Base.Aspose.Aspose.txt";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    return null;
                }
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    var decryptedLicense = DecryptLicense(ms.ToArray(), Convert.FromBase64String("4X5EafuS0rcdJfK1XdrLW3AthKv5MuKzesMX9mpyZsSMy3slnwNUefjz2pPbT8ndiIfI3gEg451mFA5b9Q610Z9EjSL0CLkAgEyM+WDtlojwukkdKiY6wvnpSc/fxjqo8r/HgMuyNlkkKMSjUpSzHgQFuHHf3YDz8EIL2B1zFE4Z/+YsjOUmtZGeYrk3NzulUyZJN7RDDWxNwGYQ583lUd2ytfhVIHk7edA6lkQAW9rU87KjVoDP7qSF3wF9irXkPdwpmU5XeVwYbxiwvChHzWxjrDP72Otcn8gpzCAZ3MFOf7b/Z877ioO4SDHb8BtlxKpnEC5/21Hr6QjWyyQlLqlu48z/PQritzWHEechQs2p+pDcMcg9Go8hXQTAESxCbYksKFaRDMX9cEUip26l15phXHLglw/4c07F59JZE3hvveQ25gc1Ds5RcuhbhZY9BcMA8a0HbzbBiBnxLuPNwZfWsb1nq94J2CsnQ6tGRxPFC3WioO09FpFWexkEDJN178c8TgkRwAB4yoRXTCIQ/1nzSnyclpad9r4cBkZoheKIprIDHwxkfsguDKXzHO1zayPik97lvXFG8/F2GQUweOjeedrAucamZmEsiJhAe/S2EBTZReADtUEyEmqZRDhJhnkzNNPoy2jorQGg/7mpLUf08HrT1yYB4BYHFWHQcXXJvSybQiowQ+2ZwtfzE8nvsdHDnjSYELWpHD02RXyTHfEBi4fTIGU67tBVO+1QL7Iv6JbQX+0W88Gjx8hE1kaHdZQYjahjJzQ3U262itpfuUQk3e84T1jYI52ijgatbZUXaqfYN5VPf1+OzBuPSuSXuNhsoXRiCoEH22FZRVsSctsdxxFzqog3Bw0akM/YC6EGEJa/70WmDDZHTAlqkKg/36CqmswcjkdZ8oSSQoNaP6rLlIwwlT1XRjq+MuC5cndIgcL0nE2fVNrIaUAwtijj6g3+1SKhm235fq5P7QZ9XszDx8uS/jU1S/eq0+YHxtCKrmSltTqfD3TngoYfoHxGD7WErVHCXAqVuOLJ3LD5UfobwgkG/oC4BjgfohKpt3J4d5fr8EE+jk7f7jCa4v3Bc/SNDKsBTO6pW69MgOFXY9bQid1MYvpXrUHqNalaM+uhjgHVzAQvyPv58laNyvttGHX0eyZMa0c9Y9N+vixSjPC3AB2kFGDA68xQfTYpG9Ws3YstwbEDinCwQhtFFKNLv6tcmdzg6NJ62Wk81QGpFeCTgKdeY3zJcfIE+Gb1x3AsJyQ68KjmhzeqeTldgusH1kjyKruOy9itcoxo0tyTXvzwLv89T7qmKgsNTwU="));
                    return new MemoryStream(decryptedLicense);
                }
            }
        }

        private static byte[] DecryptLicense(byte[] licBytes, byte[] key)
        {
            var output = new byte[licBytes.Length];
            for (var i = 0; i < licBytes.Length; i++)
            {
                output[i] = Convert.ToByte(licBytes[i] ^ key[i]);
            }
            return output;
        }
        #endregion
    }
}
