﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Commands.FrontDoor.Common;
using Microsoft.Azure.Commands.FrontDoor.Helpers;
using Microsoft.Azure.Commands.FrontDoor.Models;
using Microsoft.Azure.Commands.FrontDoor.Properties;
using Microsoft.Azure.Management.FrontDoor;
using System.Linq;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.FrontDoor.Cmdlets
{
    [Cmdlet("Set", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "FrontDoor" + "RulesEngine"), OutputType(typeof(PSRulesEngine))]
    public class SetFrontDoorRulesEngine : AzureFrontDoorCmdletBase
    {
        /// <summary>
        /// The resource group name of the Front Door.
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The resource group name that the Front Door will be created in.")]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        /// <summary>
        /// The Front Door name.
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Front Door name.")]
        [ValidateNotNullOrEmpty]
        public string FrontDoor { get; set; }

        /// <summary>
        /// The rules engine name.
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Rules engine name.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        /// <summary>
        /// A list of rules that define a particular Rules Engine Configuration.
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "A list of rules that define a particular Rules Engine Configuration.")]
        public PSRulesEngineRule[] Rules { get; set; }

        public override void ExecuteCmdlet()
        {
            var updateParameter = new Management.FrontDoor.Models.RulesEngine(
                name: Name,
                rules: Rules?.Select(x => x.ToSdkRulesEngineRule()).ToList()
                );

            try
            {
                var rulesEngine = FrontDoorManagementClient.RulesEngines.CreateOrUpdate(
                        resourceGroupName: ResourceGroupName,
                        frontDoorName: FrontDoor,
                        rulesEngineName: Name,
                        rulesEngineParameters: updateParameter
                        );
                WriteObject(rulesEngine.ToPSRulesEngine());
            }
            catch (Microsoft.Azure.Management.FrontDoor.Models.ErrorResponseException e)
            {
                throw new PSArgumentException(string.Format(Resources.Error_ErrorResponseFromServer,
                                     e.Response.Content));
            }
        }
    }
}
