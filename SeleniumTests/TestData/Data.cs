using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTests.TestData
{
    public class Data
    {
        public List<Product> Products { get; set; }
        public Card Card { get; set; }
        public ValidationMessages ValidationMessages { get; set; }
        public Customer Customer { get; set; }
    }
}
