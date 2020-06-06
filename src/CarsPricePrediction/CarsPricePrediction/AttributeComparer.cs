namespace CarsPricePrediction
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using AngleSharp.Dom;

    public class AttribureComparer : IEqualityComparer<IAttr>
    {
        public bool Equals([AllowNull] IAttr x, [AllowNull] IAttr y)
        {
            return x.LocalName == y.LocalName;
        }

        public int GetHashCode([DisallowNull] IAttr obj)
        {
            throw new NotImplementedException();
        }
    }
}
