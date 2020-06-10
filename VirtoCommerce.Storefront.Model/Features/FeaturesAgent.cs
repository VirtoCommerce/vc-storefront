namespace VirtoCommerce.Storefront.Model.Features
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    using VirtoCommerce.Storefront.Model.Features.Exceptions;

    public class FeaturesAgent : IFeaturesAgent
    {
        private readonly string _featuresBranchToken;

        public FeaturesAgent(string featuresBranchToken = "features")
        {
            _featuresBranchToken = featuresBranchToken;
        }

        public bool IsActive(string featureName, JObject jObject)
        {
            var features = GetFeatures(jObject);

            var feature = features.GetFeature(featureName);

            if (!feature.IsActive)
            {
                return false;
            }

            bool result;
            var replaced = features.AnyReplacedBy(feature.Name);
            var conflict = features.AnyConflictWith(feature.Name);

            if (replaced || conflict)
            {
                result = false;
            }
            else
            {
                var referenceFeatures = feature.GetReferenceFeatures(features);
                result = !referenceFeatures.AnyInactive();
            }

            return result;
        }

        private List<Feature> GetFeatures(JObject jObject)
        {
            List<Feature> result;

            try
            {
                var featuresJson = jObject[_featuresBranchToken];

                if (featuresJson == null)
                {
                    throw new FeaturesException($"Can' find \"{_featuresBranchToken}\" section ");
                }

                result = featuresJson.ToObject<List<Feature>>();
            }
            catch (Exception exception)
            {
                throw new FeaturesException(exception.Message, exception);
            }

            return result;
        }
    }
}
