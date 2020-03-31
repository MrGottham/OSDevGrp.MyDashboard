using System;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;

namespace OSDevGrp.MyDashboard.Core.Infrastructure
{
    public class Randomizer : IRandomizer
    {
        #region Private variables

        private readonly Random _random;

        #endregion

        #region Constructor

        public Randomizer(ISeedGenerator seedGenerator)
        {
            if (seedGenerator == null)
            {
                throw new ArgumentNullException(nameof(seedGenerator));
            }

            _random = new Random(seedGenerator.Generate());
        }

        #endregion

        #region Methods

        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentException("The maximum value cannot be lower than the minimum value.", nameof(maxValue));
            }

            return _random.Next(minValue, maxValue);
        }

        #endregion
    }
}