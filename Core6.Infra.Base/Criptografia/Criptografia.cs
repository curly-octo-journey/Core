using System.Text;

namespace Core6.Infra.Base.Criptografia
{
    public static class Criptografia
    {
        public static string Criptografar(string text)
        {
            string cryptoKey = "Q3JpcHRvZ3JhZmlhcyBjb20gUmluamRhZWwgLyBBRVM=";
            byte[] bIV = { 0x50, 0x08, 0xF1, 0xDD, 0xDE, 0x3C, 0xF2, 0x18, 0x44, 0x74, 0x19, 0x2C, 0x53, 0x49, 0xAB, 0xBC };
            try
            {
                // Se a string não está vazia, executa a criptografia            
                if (!string.IsNullOrEmpty(text))
                {
                    // Cria instancias de vetores de bytes com as chaves                                
                    byte[] bKey = Convert.FromBase64String(cryptoKey);
                    byte[] bText = new UTF8Encoding().GetBytes(text);
                    // Instancia a classe de criptografia Rijndael                
                    System.Security.Cryptography.Aes rijndael = new System.Security.Cryptography.AesManaged();
                    // Define o tamanho da chave "256 = 8 * 32"                                
                    // Lembre-se: chaves possíves:                                
                    // 128 (16 caracteres), 192 (24 caracteres) e 256 (32 caracteres)                                
                    rijndael.KeySize = 256;
                    // Cria o espaço de memória para guardar o valor criptografado:                                
                    MemoryStream mStream = new MemoryStream();
                    // Instancia o encriptador                                 
                    System.Security.Cryptography.CryptoStream encryptor = new System.Security.Cryptography.CryptoStream(mStream, rijndael.CreateEncryptor(bKey, bIV), System.Security.Cryptography.CryptoStreamMode.Write);
                    // Faz a escrita dos dados criptografados no espaço de memória                
                    encryptor.Write(bText, 0, bText.Length);
                    // Despeja toda a memória.                                
                    encryptor.FlushFinalBlock();
                    // Pega o vetor de bytes da memória e gera a string criptografada                                
                    return Convert.ToBase64String(mStream.ToArray());
                }
                else
                {
                    // Se a string for vazia retorna nulo                                
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Se algum erro ocorrer, dispara a exceção                        
                throw new ApplicationException("Erro ao criptografar", ex);
            }
        }

        public static string Descriptografar(string text)
        {
            string cryptoKey = "Q3JpcHRvZ3JhZmlhcyBjb20gUmluamRhZWwgLyBBRVM=";
            byte[] bIV = { 0x50, 0x08, 0xF1, 0xDD, 0xDE, 0x3C, 0xF2, 0x18, 0x44, 0x74, 0x19, 0x2C, 0x53, 0x49, 0xAB, 0xBC };
            try
            {
                // Se a string não está vazia, executa a criptografia                       
                if (!string.IsNullOrEmpty(text))
                {
                    // Cria instancias de vetores de bytes com as chaves                             
                    byte[] bKey = Convert.FromBase64String(cryptoKey);
                    byte[] bText = Convert.FromBase64String(text);
                    // Instancia a classe de criptografia Rijndael                       
                    System.Security.Cryptography.Aes rijndael = new System.Security.Cryptography.AesManaged();
                    // Define o tamanho da chave "256 = 8 * 32"                
                    // Lembre-se: chaves possíves:               
                    // 128 (16 caracteres), 192 (24 caracteres) e 256 (32 caracteres)     
                    rijndael.KeySize = 256;
                    // Cria o espaço de memória para guardar o valor DEScriptografado:        
                    MemoryStream mStream = new MemoryStream();
                    // Instancia o Decriptador                         
                    System.Security.Cryptography.CryptoStream decryptor = new System.Security.Cryptography.CryptoStream(
                        mStream,
                        rijndael.CreateDecryptor(bKey, bIV),
                        System.Security.Cryptography.CryptoStreamMode.Write);
                    // Faz a escrita dos dados criptografados no espaço de memória                   
                    decryptor.Write(bText, 0, bText.Length);
                    // Despeja toda a memória.                         
                    decryptor.FlushFinalBlock();
                    // Instancia a classe de codificação para que a string venha de forma correta    
                    UTF8Encoding utf8 = new UTF8Encoding();
                    // Com o vetor de bytes da memória, gera a string descritografada em UTF8      
                    return utf8.GetString(mStream.ToArray(), 0, (int)mStream.Length);
                }
                else
                {
                    // Se a string for vazia retorna nulo   
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Se algum erro ocorrer, dispara a exceção   
                throw new Exception("Erro ao descriptografar", ex);
            }
        }
    }
}
