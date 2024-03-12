using Newtonsoft.Json;

namespace Core6.Infra.Base.Integracao.DTOs
{
    public class RetornoLoginTokenDTO
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("Conexao")]
        public string Conexao { get; set; }

        [JsonProperty("CodigoUsuario")]
        public int? CodigoUsuario { get; set; }

        [JsonProperty("CodigoEmpresa")]
        public int? CodigoEmpresa { get; set; }

        [JsonProperty("CodigoFilial")]
        public int? CodigoFilial { get; set; }

        [JsonProperty("TokenMensageria")]
        public string TokenMensageria { get; set; }
    }
}
