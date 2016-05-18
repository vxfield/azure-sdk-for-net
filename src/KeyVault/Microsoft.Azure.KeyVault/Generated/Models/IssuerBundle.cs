// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Microsoft.Azure.KeyVault.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;
    using Microsoft.Rest.Azure;

    public partial class IssuerBundle
    {
        /// <summary>
        /// Initializes a new instance of the IssuerBundle class.
        /// </summary>
        public IssuerBundle() { }

        /// <summary>
        /// Initializes a new instance of the IssuerBundle class.
        /// </summary>
        public IssuerBundle(string id = default(string), string provider = default(string), IssuerCredentials credentials = default(IssuerCredentials), OrganizationDetails organizationDetails = default(OrganizationDetails), IssuerAttributes attributes = default(IssuerAttributes))
        {
            Id = id;
            Provider = provider;
            Credentials = credentials;
            OrganizationDetails = organizationDetails;
            Attributes = attributes;
        }

        /// <summary>
        /// Identifier for the issuer object.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; private set; }

        /// <summary>
        /// The name of the issuer.
        /// </summary>
        [JsonProperty(PropertyName = "provider")]
        public string Provider { get; set; }

        /// <summary>
        /// The credentials to be used for the issuer.
        /// </summary>
        [JsonProperty(PropertyName = "credentials")]
        public IssuerCredentials Credentials { get; set; }

        /// <summary>
        /// Details of the organization as provided to the issuer.
        /// </summary>
        [JsonProperty(PropertyName = "org_details")]
        public OrganizationDetails OrganizationDetails { get; set; }

        /// <summary>
        /// Attributes of the issuer object.
        /// </summary>
        [JsonProperty(PropertyName = "attributes")]
        public IssuerAttributes Attributes { get; set; }

    }
}
