﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Allegro_Graph_CSharp_Client.AGClient.OpenRDF.Model;

namespace Allegro_Graph_CSharp_Client.AGClient.OpenRDF.RepositoryUtil
{
    public partial class RepositoryConnection
    {
        /// <summary>
        /// List the geo-spatial types registered in the store.
        /// </summary>
        /// <returns>geo-spatial types</returns>
        public string[] ListGeoTypes()
        {
            return this.GetMiniRepository().ListGeoTypes();
        }

        /// <summary>
        /// Define a new Cartesian geospatial type. Returns the type resource
        /// </summary>
        /// <param name="stripWidth">
        /// A floating-point number that determines the granularity of the type
        /// </param>
        /// <param name="xmin">
        /// Floating-point numbers that determine the min x size of the Cartesian plane that is modelled by this type
        /// </param>
        /// <param name="xmax">
        /// Floating-point numbers that determine the man x size of the Cartesian plane that is modelled by this type
        /// </param>
        /// <param name="ymin">
        /// Floating-point numbers that determine the min y size of the Cartesian plane that is modelled by this type
        /// </param>
        /// <param name="ymax">
        /// Floating-point numbers that determine the max y size of the Cartesian plane that is modelled by this type
        /// </param>
        public string SetCartesianGeoType(float stripWidth, float xmin , float xmax , float ymin , float ymax )
        {
            return this.GetMiniRepository().SetCartesianGeoType(stripWidth, xmin, xmax, ymin, ymax);
        }


        /// <summary>
        /// Add a spherical geospatial type. Returns the type resource.
        /// </summary>
        /// <param name="stripWidth">
        /// A floating-point number that determines the granularity of the type
        /// </param>
        /// <param name="unit">
        ///  Can be degree, radian, km, or mile. Determines the unit in which the stripWidth argument is given
        /// </param>
        /// <param name="latmin">
        /// Optional.
        /// Can be used to limit the size of the region modelled by this type. 
        /// Default is to span the whole sphere. 
        /// </param>
        /// <param name="latmax">
        ///  Optional.
        /// Can be used to limit the size of the region modelled by this type. 
        /// Default is to span the whole sphere. 
        /// </param>
        /// <param name="longmin">
        /// Optional.
        /// Can be used to limit the size of the region modelled by this type. 
        /// Default is to span the whole sphere.  
        /// </param>
        /// <param name="longmax">
        /// Optional.
        /// Can be used to limit the size of the region modelled by this type. 
        /// Default is to span the whole sphere.  
        /// </param>
        /// <returns></returns>
        public string SetSphericalGeoType(float stripWidth, string unit = "degree", float latmin = 361, float latmax = 361, float longmin = 361, float longmax = 361)
        {
            return this.GetMiniRepository().SetSphericalGeoType(stripWidth, unit, latmin, latmax, longmin, longmax);
        }




        public List<Statement> GetStatementsInsideBox(string type, string predicate,
                                           float xMin, float xMax, float yMin, float yMax,
                                           float limit = -1, float offset = -1, string useContext="false")
        {
            return this.GetMiniRepository().GetStatementsInsideBox(type,predicate,xMin,xMax,yMin,yMax,limit,offset,useContext);
        }

        public List<Statement> GetStatementsInsideCircle(string type, string predicate,
                                                         float x, float y, float radius,
                                                         float limit = -1, float offset = -1,string useContext="false")
        {
            return this.GetMiniRepository().GetStatementsInsideCircle(type, predicate, x, y, radius, limit, offset,useContext);
        }

        /// <summary>
        /// Fetch all triples with a given predicate whose object is a geospatial value inside the given region. 
        /// </summary>
        /// <param name="predicate">The geospatial type of the object field.</param>
        /// <param name="region">region</param>
        /// <param name="limit">Optional. Used to limit the amount of returned triples</param>
        /// <param name="offset">Optional. Used to skip a number of returned triples.</param>
        /// <returns></returns>
        public List<Statement> GetStatementsOfGeo(string predicate, GeoSpatial region,int limit = -1, int offset = -1)
        {
            string geoDataType = region.GeoDataType;
            if (region is GeoBox)
            {
                GeoBox geoBox = region as GeoBox;
                if (geoDataType == GeoSpatial.Cartesian)
                {
                    return this.GetMiniRepository().GetStatementsInsideBox(geoDataType, predicate,
                                                                    geoBox.XMin, geoBox.XMax, geoBox.YMin, geoBox.YMax,
                                                                    limit, offset);
                }
                else if (geoDataType == GeoSpatial.Spherical)
                {
                    return this.GetMiniRepository().GetStatementsInsideBox(geoDataType, predicate,
                                                                    geoBox.YMin, geoBox.YMax, geoBox.XMin, geoBox.XMax,
                                                                    limit, offset);
                }
            }
            else if (region is GeoCircle)
            {
                GeoCircle geoCircle = region as GeoCircle;
                if (geoCircle.GeoDataType == GeoSpatial.Cartesian)
                {
                    return this.GetMiniRepository().GetStatementsInsideCircle(geoCircle.GeoDataType, predicate,
                                                                              geoCircle.X, geoCircle.Y, geoCircle.Radius,
                                                                              limit, offset);
                }
                else if (geoCircle.GeoDataType == GeoSpatial.Spherical)
                {
                    return this.GetMiniRepository().GetStatementsHaversine(geoCircle.GeoDataType, predicate,
                                                                           geoCircle.X, geoCircle.Y, geoCircle.Radius,
                                                                           geoCircle.Unit, limit, offset);
                }
            }
            return null;
        }

        /// <summary>
        /// Create a rectangular search region (a box) for geospatial search.
        /// This method works for both Cartesian and spherical coordinate systems.
        /// xMin, xMax may be used to input latitude. yMin, yMax may be used to input longitude.
        /// </summary>
        /// <param name="xMin"></param>
        /// <param name="xMax"></param>
        /// <param name="yMin"></param>
        /// <param name="yMax"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public GeoBox CreateBox(float xMin = 0, float xMax = 0, float yMin = 0, float yMax = 0, string unit = null)
        {
            return new GeoBox(xMin, xMax, yMin, yMax, unit);
        }

        /// <summary>
        /// Create a circular search region for geospatial search. 
        /// This method works for both Cartesian and spherical coordinate systems. 
        /// radius is the radius of the circle expressed in the designated unit,
        /// which defaults to the unit assigned to the coordinate system.
        /// x and y locate the center of the circle and may be used for latitude and longitude. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public GeoCircle CreateCircle(float x, float y, float radius, string unit = null)
        {
            return new GeoCircle(x, y, radius, unit);
        }
    }



    /// <summary>
    /// Base class for Geo-spatial queries
    /// </summary>
    public class GeoSpatial
    {
        public const string Cartesian = "CARTESIAN";
        public const string Spherical = "SPHERICAL";
        string geoDataType;

        public string GeoDataType
        {
            get { return geoDataType; }
            set { geoDataType = value; }
        }

        public GeoSpatial(string dt = null)
        {
            this.geoDataType = dt;
        }
    }
    /// <summary>
    ///  Define either a cartesian coordinate or a spherical coordinate. 
    ///  For the latter, unit can be 'km', 'mile', 'radian', or 'degree'
    /// </summary>
    public class GeoCoordinate : GeoSpatial
    {
        float xcoor;
        float ycoor;
        string unit;
        public GeoCoordinate(float x = 0, float y = 0, string unit = null, string geoDataType = null)
            : base(geoDataType)
        {
            this.xcoor = x;
            this.ycoor = y;
            this.unit = unit;
        }
        public override string ToString()
        {
            return string.Format("|COOR|({0}, {1})", this.xcoor, this.ycoor);
        }
    }

    public class GeoBox : GeoSpatial
    {
        float xMin;
        float xMax;
        float yMin;
        float yMax;
        string unit;

        public float XMin { get { return xMin; } }
        public float XMax { get { return xMax; } }
        public float YMin { get { return yMin; } }
        public float YMax { get { return yMax; } }
        public string Unit { get { return unit; } }

        public GeoBox(float xMin = 0, float xMax = 0, float yMin = 0, float yMax = 0, string unit = null)
        {
            this.xMin = xMin;
            this.xMax = xMax;
            this.yMin = yMin;
            this.yMax = yMax;
            this.unit = unit;
        }
        public override string ToString()
        {
            return string.Format("|Box|{0},{1},{2},{3}", xMin, xMax, yMin, yMax);
        }
    }

    public class GeoCircle : GeoSpatial
    {
        float x;
        float y;
        float radius;
        string unit;

        public float X { get { return x; } }
        public float Y { get { return y; } }
        public float Radius { get { return radius; } }
        public string Unit { get { return unit; } }

        public GeoCircle(float x = 0, float y = 0, float radius = 0, string unit = null)
        {
            this.x = x;
            this.y = y;
            this.radius = radius;
            this.unit = unit;
        }
        public override string ToString()
        {
            return string.Format("|Circle|{0},{1}, radius={2}", x, y, radius);
        }
    }
}
