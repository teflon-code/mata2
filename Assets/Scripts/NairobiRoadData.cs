using System;
using UnityEngine;

namespace NairobiMatatu
{
    [Serializable]
    public class RoadLine
    {
        public string name;
        public int lanes;
        public Vector2[] points;
    }

    public static class NairobiRoadData
    {
        // Coordinate pairs are sampled OpenStreetMap centerlines in [longitude, latitude].
        // The collection is intentionally a compact playable subset of Nairobi CBD.
        public static readonly RoadLine[] Roads =
        {
            Line("Uhuru Highway", 4,
                (36.81700f,-1.28500f),(36.81675f,-1.28620f),(36.81635f,-1.28755f),
                (36.81595f,-1.28920f),(36.81555f,-1.29110f),(36.81515f,-1.29320f)),
            Line("Moi Avenue", 2,
                (36.82210f,-1.28315f),(36.82205f,-1.28420f),(36.82195f,-1.28535f),
                (36.82180f,-1.28655f),(36.82165f,-1.28785f),(36.82145f,-1.28915f)),
            Line("Kenyatta Avenue", 2,
                (36.81480f,-1.28420f),(36.81615f,-1.28435f),(36.81755f,-1.28455f),
                (36.81895f,-1.28480f),(36.82035f,-1.28505f),(36.82165f,-1.28530f)),
            Line("Haile Selassie Avenue", 4,
                (36.81460f,-1.28990f),(36.81635f,-1.28955f),(36.81815f,-1.28935f),
                (36.81990f,-1.28920f),(36.82170f,-1.28910f),(36.82320f,-1.28900f)),
            Line("University Way", 2,
                (36.81380f,-1.28735f),(36.81545f,-1.28715f),(36.81715f,-1.28695f),
                (36.81890f,-1.28675f),(36.82060f,-1.28660f),(36.82200f,-1.28650f)),
            Line("Muindi Mbingu Street", 2,
                (36.81810f,-1.28345f),(36.81800f,-1.28465f),(36.81790f,-1.28585f),
                (36.81770f,-1.28715f),(36.81755f,-1.28845f),(36.81740f,-1.28960f)),
            Line("Mama Ngina Street", 2,
                (36.81610f,-1.28240f),(36.81730f,-1.28255f),(36.81855f,-1.28275f),
                (36.81985f,-1.28295f),(36.82110f,-1.28310f)),
            Line("Banda Street", 2,
                (36.81955f,-1.28345f),(36.81945f,-1.28465f),(36.81935f,-1.28585f),
                (36.81920f,-1.28710f),(36.81905f,-1.28835f)),
            Line("Accra Road", 2,
                (36.82320f,-1.29000f),(36.82215f,-1.28975f),(36.82105f,-1.28945f),
                (36.81995f,-1.28920f),(36.81880f,-1.28895f)),
        };

        private static RoadLine Line(string name, int lanes, params (float lon, float lat)[] coordinates)
        {
            var result = new RoadLine { name = name, lanes = lanes, points = new Vector2[coordinates.Length] };
            for (var i = 0; i < coordinates.Length; i++)
                result.points[i] = new Vector2(coordinates[i].lon, coordinates[i].lat);
            return result;
        }
    }
}
