/* Options:
Date: 2019-01-23 12:03:39
Version: 5.41
Tip: To override a DTO option, remove "//" prefix before updating
BaseUrl: https://localhost:5001

GlobalNamespace: MyApp
//MakePartial: True
//MakeVirtual: True
//MakeInternal: False
//MakeDataContractsExtensible: False
//AddReturnMarker: True
//AddDescriptionAsComments: True
//AddDataContractAttributes: False
//AddIndexesToDataMembers: False
//AddGeneratedCodeAttributes: False
//AddResponseStatus: False
//AddImplicitVersion: 
//InitializeCollections: True
//ExportValueTypes: False
//IncludeTypes: 
//ExcludeTypes: 
//AddNamespaces: 
//AddDefaultXmlNamespace: http://schemas.servicestack.net/types
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceStack;
using ServiceStack.DataAnnotations;
using MyApp;


namespace MyApp
{

    [Route("/servicestack-identity")]
    public partial class GetIdentity
        : IReturn<GetIdentityResponse>
    {
    }

    public partial class GetIdentityResponse
    {
        public GetIdentityResponse()
        {
            Claims = new List<Property>{};
        }

        public virtual List<Property> Claims { get; set; }
        public virtual AuthUserSession Session { get; set; }
    }
}

