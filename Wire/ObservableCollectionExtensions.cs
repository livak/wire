using System.Collections.ObjectModel;

namespace Wire
{
    public static class ObservableCollectionExtensions
    {
        public static void AddInOrder(this ObservableCollection<double> target, double item)
        {
            var indexToAdd = 0;
            for (int i = 0; i < target.Count; i++)
            {
                if (item > target[i])
                {
                    indexToAdd = i + 1;
                }
            }

            target.Insert(indexToAdd, item);
        }
    }
}
