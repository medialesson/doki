using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ml.Doki.Models.Grouping
{
    public class DonationsPerMonthGroup: IGrouping<int, Donation>
    {
        public int Key { get; }

        private List<Donation> _menuItemsGroup;

        public DonationsPerMonthGroup(int key, IEnumerable<Donation> items)
        {
            Key = key;
            _menuItemsGroup = items.ToList();
        }

        public IEnumerator<Donation> GetEnumerator() => _menuItemsGroup.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _menuItemsGroup.GetEnumerator();
    }
}
