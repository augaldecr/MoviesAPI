using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Shared.Herlpers
{
    public class GeometryHelper
    {
        private readonly GeometryFactory _geometryFactory;    
        public GeometryHelper()
        {
            _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        }

        public Point createPoint(double LongX, double LatY)
        {
            return _geometryFactory.CreatePoint(new Coordinate(LongX, LatY));
        }
    }
}
