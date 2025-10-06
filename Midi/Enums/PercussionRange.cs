using System;
using System.Linq;

namespace fractions
{
    public class PercussionRange : EnumRange<Percussion>
    {
        public static readonly Percussion _minPercussion = Percussion.BassDrum2;
        public static readonly Percussion _maxPercussion = Percussion.OpenTriangle;
        public static readonly Lazy<PercussionRange> _default = new Lazy<PercussionRange>(() => 
          new PercussionRange(_minPercussion, _maxPercussion)
        );

        public PercussionRange(Percussion min, Percussion max) : base(min, max)
        {
        }

        public static PercussionRange Default => _default.Value;

        public override void Assert(Percussion min, Percussion max)
        {
            if (min < _minPercussion)
                throw new ArgumentException($"{nameof(Min)} must be >= {_minPercussion}", nameof(Min));
            if (max > _maxPercussion)
                throw new ArgumentException($"{nameof(Max)} must be <= {_maxPercussion}", nameof(Max));
            if (min > max)
                throw new ArgumentException($"{nameof(Min)} must be <= {nameof(Max)}", nameof(Min));
        }
    }
}
