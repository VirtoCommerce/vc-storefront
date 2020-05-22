namespace VirtoCommerce.Storefront.Model.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using VirtoCommerce.Storefront.Model.Features.Exceptions;

    public static class FeatureExtensions
    {
        public static bool AnyConflictWith(this IReadOnlyCollection<Feature> features, string featureName)
        {
            return features.Any(feature => feature.IsActive && feature.Conflicts.Contains(featureName));
        }

        public static bool AnyInactive(this IEnumerable<Feature> features)
        {
            return features.Any(feature => !feature.IsActive);
        }

        public static bool AnyReplacedBy(this IReadOnlyCollection<Feature> features, string featureName)
        {
            return features.Any(feature => feature.IsActive && feature.Replaces.Contains(featureName));
        }

        public static Feature GetFeature(this IReadOnlyCollection<Feature> features, string featureName)
        {
            var result = features.FirstOrDefault(feature => feature.Name == featureName);

            if (result == null)
            {
                throw new FeaturesException($"Can't find the future \"{featureName}\"");
            }

            return result;
        }

        public static IEnumerable<Feature> GetReferenceFeatures(
            this Feature targetFeature,
            IReadOnlyCollection<Feature> features)
        {
            return features.Where(feature => feature.Requires.Contains(targetFeature.Name));
        }
    }
}
