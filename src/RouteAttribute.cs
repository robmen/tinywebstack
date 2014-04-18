namespace TinyWebStack
{
    using System;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RouteAttribute : Attribute, IComparable<RouteAttribute>
    {
        public RouteAttribute(string path)
        {
            this.Path = path;

            this.Complexity = this.Path.Split('/').Length;
        }

        public RouteAttribute(string name, string path)
            : this(path)
        {
            this.Name = name;
        }

        public string Name { get; private set; }

        public string Path { get; private set; }

        public int Complexity { get; private set; }

        public Type DefaultsAndConstraintsProviderType { get; set; }

        public int CompareTo(RouteAttribute other)
        {
            int result = 0;
            var thisSegments = this.Path.Split('/').Select(s => new { Path = s, Variable = s.StartsWith("{"), Wildcard = s.StartsWith("{*") }).ToArray();
            var otherSegments = other.Path.Split('/').Select(s => new { Path = s, Variable = s.StartsWith("{"), Wildcard = s.StartsWith("{*") }).ToArray();

            for (int i = 0; result == 0 && i < thisSegments.Length && i < otherSegments.Length; ++i)
            {
                var thisSegment = thisSegments[i];
                var otherSegment = otherSegments[i];

                // A route with a wildcard goes last.
                if (thisSegment.Wildcard ^ otherSegment.Wildcard)
                {
                    result = thisSegment.Wildcard ? 1 : -1;
                }
                else if (thisSegment.Variable ^ otherSegment.Variable) // a route with variables goes later.
                {
                    result = thisSegment.Variable ? 1 : -1;
                }
                else // otherwise routes are ordered alphabetically.
                {
                    result = thisSegment.Path.CompareTo(otherSegment.Path);
                }
            }

            // If all segments (that exist) are equal, the more specific route (the one with
            // the longest path) goes first.
            if (result == 0)
            {
                result = thisSegments.Length.CompareTo(otherSegments.Length) * -1;
            }

            return result;
        }
    }
}
