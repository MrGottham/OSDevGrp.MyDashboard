using System;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public abstract class ViewModelBuilderBase<TOutput, TInput> : IViewModelBuilder<TOutput, TInput> where TOutput : class, IViewModel where TInput : class 
    {
        #region Methods

        public Task<TOutput> BuildAsync(TInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return Task.Run<TOutput>(() => Build(input));
        }

        protected abstract TOutput Build(TInput input);

        #endregion
    }
}