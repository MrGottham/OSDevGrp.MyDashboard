using System;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;

namespace OSDevGrp.MyDashboard.Core.Infrastructure
{
    public class Randomizer : IRandomizer
    {
        #region Private variables

        private static Random _random;
        private static readonly object SyncRoot = new object();

        #endregion

        #region Properties

        private Random Random
        {
            get
            {
                lock (SyncRoot)
                {
                    return _random ?? (_random = new Random(DateTime.Now.Millisecond));
                }
            }
        }
        #endregion

        #region Methods

        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentException("The maximum value cannot be lower than the minimum value.", nameof(maxValue));
            }

            return Random.Next(minValue, maxValue);
        }

        #endregion
    }
}