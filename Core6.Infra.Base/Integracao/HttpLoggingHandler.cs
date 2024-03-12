using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace Core6.Infra.Base.Integracao
{
    public class HttpLoggingHandler : DelegatingHandler
    {
        #region Ctor
        private readonly string[] _types = { "html", "text", "xml", "json", "txt" };

        public HttpLoggingHandler(HttpMessageHandler innerHandler = null)
            : base(innerHandler ?? new HttpClientHandler())
        {

        }
        #endregion

        #region SendAsync
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await Task.Delay(1, cancellationToken).ConfigureAwait(false);

            var stringBuilder = new StringBuilder();

            var req = request;
            var id = Guid.NewGuid().ToString();
            var msg = string.Format("[{0} -   Request]", id);

            stringBuilder.AppendLine(string.Format("{0}========Start==========", msg));
            stringBuilder.AppendLine(string.Format("{0} {1} {2} {3}/{4}", msg, req.Method, req.RequestUri.PathAndQuery, req.RequestUri.Scheme, req.Version));
            stringBuilder.AppendLine(string.Format("{0} Host: {1}", msg, req.RequestUri.Scheme));

            foreach (var header in req.Headers)
            {
                stringBuilder.AppendLine(string.Format("{0} {1}: {2}", msg, header.Key, string.Join(", ", header.Value)));
            }

            if (req.Content != null)
            {
                foreach (var header in req.Content.Headers)
                {
                    stringBuilder.AppendLine(string.Format("{0} {1}: {2}", msg, header.Key, string.Join(", ", header.Value)));
                }

                if (req.Content is StringContent || IsTextBasedContentType(req.Headers) || IsTextBasedContentType(req.Content.Headers))
                {
                    var result = await req.Content.ReadAsStringAsync();
                    stringBuilder.AppendLine(string.Format("{0} Content-Size: {1}", msg, req.Content.Headers.ContentLength));
                    stringBuilder.AppendLine(string.Format("{0} Content: ", msg));
                    stringBuilder.AppendLine(string.Format("{0} {1}", msg, result.Cast<char>()));
                }
            }

            var start = DateTime.Now;

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            var end = DateTime.Now;

            //stringBuilder.AppendLine(string.Format("{0} ", msg));
            stringBuilder.AppendLine(string.Format("{0} Duration: {1}", msg, end - start));
            stringBuilder.AppendLine(string.Format("{0} ==========End==========", msg));

            msg = string.Format("[{0} - Response]", id);
            stringBuilder.AppendLine(string.Format("{0}=========Start=========", msg));

            var resp = response;

            stringBuilder.AppendLine(string.Format("{0} {1}/{2} {3} {4} ", msg, req.RequestUri.Scheme.ToUpper(), resp.Version, (int)resp.StatusCode, resp.ReasonPhrase));

            foreach (var header in resp.Headers)
            {
                stringBuilder.AppendLine(string.Format("{0} {1}: {2}", msg, header.Key, string.Join(", ", header.Value)));
            }

            if (resp.Content != null)
            {
                foreach (var header in resp.Content.Headers)
                {
                    stringBuilder.AppendLine(string.Format("{0} {1}: {2}", msg, header.Key, string.Join(", ", header.Value)));
                }

                if (resp.Content is StringContent || IsTextBasedContentType(resp.Headers) || IsTextBasedContentType(resp.Content.Headers))
                {
                    start = DateTime.Now;
                    var result = await resp.Content.ReadAsStringAsync();
                    end = DateTime.Now;

                    stringBuilder.AppendLine(string.Format("{0} Content-Size: {1}", msg, resp.Content.Headers.ContentLength));
                    stringBuilder.AppendLine(string.Format("{0} Content: ", msg));
                    stringBuilder.AppendLine(string.Format("{0} {1}", msg, result.Cast<char>()));
                    stringBuilder.AppendLine(string.Format("{0} Duration to read: {1}", msg, end - start));
                }
            }

            stringBuilder.AppendLine(string.Format("{0}==========End==========", msg));

            Debug.WriteLine(stringBuilder.ToString());

            return response;
        }
        #endregion

        #region IsTextBasedContentType
        private bool IsTextBasedContentType(HttpHeaders headers)
        {
            IEnumerable<string> values;
            if (!headers.TryGetValues("Content-Type", out values))
            {
                return false;
            }

            var header = string.Join(" ", values).ToLowerInvariant();

            return _types.Any(t => header.Contains(t));
        }
        #endregion
    }
}
