
namespace Model.Types
{
    public class Keyword
    {
        [Hidden]
        public int Id { get; init; }

        public string WordOrPhrase { get; init; }

        public override string ToString() => WordOrPhrase;
    }
}
