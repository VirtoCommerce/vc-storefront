using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace VirtoCommerce.Storefront.Model.Security
{
    public class UserActionIdentityResult
    {
        private static readonly UserActionIdentityResult _success = new UserActionIdentityResult { Succeeded = true };
        private List<IdentityError> _errors = new List<IdentityError>();

        /// <summary>
        /// Flag indicating whether if the operation succeeded or not.
        /// </summary>
        /// <value>True if the operation succeeded, otherwise false.</value>
        public bool Succeeded { get; protected set; }

        /// <summary>
        /// An <see cref="IEnumerable{T}"/> of <see cref="IdentityError"/>s containing an errors
        /// that occurred during the identity operation.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> of <see cref="IdentityError"/>s.</value>
        public IEnumerable<IdentityError> Errors => _errors;

        /// <summary>
        /// Returns an <see cref="IdentityResult"/> indicating a successful identity operation.
        /// </summary>
        /// <returns>An <see cref="IdentityResult"/> indicating a successful operation.</returns>
        public static UserActionIdentityResult Success => _success;

        public string MemberId { get; set; }

        public string ReturnUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="identityResult"></param>
        /// <returns></returns>
        public static UserActionIdentityResult Instance(IdentityResult identityResult)
        {
            var result = new UserActionIdentityResult
            {
                Succeeded = identityResult.Succeeded,
                _errors = identityResult.Errors.ToList()
            };

            return result;
        }

        /// <summary>
        /// Creates an <see cref="IdentityResult"/> indicating a failed identity operation, with a list of <paramref name="errors"/> if applicable.
        /// </summary>
        /// <param name="errors">An optional array of <see cref="IdentityError"/>s which caused the operation to fail.</param>
        /// <returns>An <see cref="IdentityResult"/> indicating a failed identity operation, with a list of <paramref name="errors"/> if applicable.</returns>
        public static UserActionIdentityResult Failed(params IdentityError[] errors)
        {
            var result = new UserActionIdentityResult { Succeeded = false };
            if (errors != null)
            {
                result._errors.AddRange(errors);
            }
            return result;
        }

        public static UserActionIdentityResult Failed(params FormError[] errors)
        {
            var result = new UserActionIdentityResult { Succeeded = false };
            if (errors != null)
            {
                result._errors.AddRange(errors.Select(x=> new IdentityError { Code = x.Code, Description = x.Description }));
            }
            return result;
        }

        /// <summary>
        /// Converts the value of the current <see cref="IdentityResult"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the current <see cref="IdentityResult"/> object.</returns>
        /// <remarks>
        /// If the operation was successful the ToString() will return "Succeeded" otherwise it returned 
        /// "Failed : " followed by a comma delimited list of error codes from its <see cref="Errors"/> collection, if any.
        /// </remarks>
        public override string ToString()
        {
            return Succeeded ?
                "Succeeded" :
                string.Format("{0} : {1}", "Failed", string.Join(",", Errors.Select(x => x.Code).ToList()));
        }
    }
}
