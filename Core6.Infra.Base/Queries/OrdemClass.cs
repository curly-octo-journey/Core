namespace Core6.Infra.Base.Queries
{
    public class OrdemClass
    {
        public string property { get; set; }

        public string direction { get; set; }

        public object filterValue;

        public bool ASC()
        {
            return direction.ToLower() == "asc";
        }
    }
}
