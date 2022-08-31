namespace MergeSort
{
    public static class Sorting
    {
        public static List<T> Merge<T>(List<T> a, List<T> b) where T : IComparable=>
            a.Count == 0 ? b :
                b.Count == 0 ? a :
                    Head(a).CompareTo(Head(b)) > 0 ? Prepend(Head(a), Merge(Tail(a), b)) :
                        Prepend(Head(b), Merge(a, Tail(b)));

        public static List<T> MergeSort<T>(List<T> list) where T : IComparable =>
            Merge(MergeSort(FrontHalf(list)), MergeSort(BackHalf(list)));

        public static List<T> FrontHalf<T>(List<T> a) => a.Take(a.Count / 2).ToList();
        public static List<T> BackHalf<T>(List<T> a) => a.Skip(a.Count / 2).ToList();

        public static T Head<T>(List<T> list) => list[0];

        public static List<T> Tail<T>(List<T> list) => list.Skip(1).ToList();

        public static List<T> Prepend<T>(T newHead, List<T> list) => list.Prepend(newHead).ToList();

           
    }
} 