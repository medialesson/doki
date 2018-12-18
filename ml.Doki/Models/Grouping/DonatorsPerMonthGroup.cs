using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ml.Doki.Models.Grouping
{
    public class DonatorsPerMonthGroup : IGrouping<int, Donator>
    {
        public int Key { get; }

        private List<Donator> _menuItemsGroup;

        public DonatorsPerMonthGroup(int key, IEnumerable<Donator> items)
        {
            Key = key;
            _menuItemsGroup = items.ToList();
        }

        public IEnumerator<Donator> GetEnumerator() => _menuItemsGroup.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _menuItemsGroup.GetEnumerator();
    }
}
