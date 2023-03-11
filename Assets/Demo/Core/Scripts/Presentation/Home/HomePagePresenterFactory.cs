using Demo.Core.Scripts.Domain.FeatureFlag.MasterRepository;
using Demo.Core.Scripts.Presentation.Shared;
using Demo.Core.Scripts.View.Home;

namespace Demo.Core.Scripts.Presentation.Home
{
    public sealed class HomePagePresenterFactory
    {
        private readonly IFeatureFlagMasterRepository _featureFlagMasterRepository;

        public HomePagePresenterFactory(IFeatureFlagMasterRepository featureFlagMasterRepository)
        {
            _featureFlagMasterRepository = featureFlagMasterRepository;
        }

        public HomePagePresenter Create(HomePage view, ITransitionService transitionService)
        {
            return new HomePagePresenter(view, transitionService, _featureFlagMasterRepository);
        }
    }
}
