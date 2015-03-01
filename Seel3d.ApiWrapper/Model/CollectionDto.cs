using System;
using System.Collections.Generic;

namespace Seel3d.ApiWrapper.Model
{
    public class CollectionDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Brand { get; set; }

        public DateTime Debut { get; set; }

        public DateTime Fin { get; set; }

        public List<int> ClotheList { get; set; }
    }
}
