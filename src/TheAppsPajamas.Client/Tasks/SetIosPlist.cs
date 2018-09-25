using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheAppsPajamas.Client.Extensions;
using TheAppsPajamas.Client.Helpers;
using TheAppsPajamas.Shared.Types;
using Microsoft.Build.Framework;

namespace TheAppsPajamas.Client.Tasks
{
    public class SetIosPlist : BaseTask
    {
        public string IosPlist { get; set; }

        public ITaskItem[] PackagingFields { get; set; }

        public ITaskItem PackagingHolder { get; set; }

        public override bool Execute()
        {
            var baseResult = base.Execute();
            LogInformation("Setting Ios Plist file");

            if (PackagingHolder.IsDisabled()){
                LogInformation($"Packaging is disabled in this configuration, plist will not be set");
                return true;
            }

            if (String.IsNullOrEmpty(IosPlist))
            {
                LogInformation("Ios Plist file name empty, aborting SetIosPlist as ran successful");
                return true;
            }

            LogDebug("Plist file name '{0}'", IosPlist);
            LogDebug("Packaging fields: '{0}'", PackagingFields.Count());

            try
            {
                bool touched = false;
                var plist = (Dictionary<string, object>)Plist.readPlist(IosPlist);
                LogDebug("Plist {0} read, {1} nodes", IosPlist, plist.Count());

                touched = Name(touched, plist);

                touched = Identifier(touched, plist);

                touched = VersionText(touched, plist);

                touched = VersionNumber(touched, plist);

                var assetCatalogueNameField = PackagingFields
                    .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingIosAssetCatalogueName);

                touched = AppIconCatalogueSetName(touched, plist, assetCatalogueNameField);

                touched = LaunchCatalogueSetName(touched, plist, assetCatalogueNameField);

                //don't think we need this
                bool useLaunchStoryboard = false;
                var useLaunchStoryboardField = PackagingFields
                    .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingIosUseLaunchStoryboard);
                if (useLaunchStoryboardField != null
                    && useLaunchStoryboardField.IsEnabled()
                    && !String.IsNullOrEmpty(useLaunchStoryboardField.GetMetadata("Value")))
                {
                    useLaunchStoryboard = useLaunchStoryboardField.IsTrue(this);
                }


                touched = LaunchStoryboardName(touched, plist, useLaunchStoryboard);


                //don't need to set other image asset catalogues in plist. bonus


                touched = UsesNonExemptEncryption(touched, plist);


                if (touched)
                {
                    LogInformation("Plist touched, saving");
                    Plist.writeXml(plist, IosPlist);
                }
                else
                {
                    LogInformation("Plist untouched, skipping");
                }

            }
            catch (Exception ex){
                Log.LogErrorFromException(ex);
                return false;
            }

            return true;
        }

        private bool Name(bool touched, Dictionary<string, object> plist)
        {
            var packageNameField = PackagingFields
                .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingIosName);

            if (packageNameField != null && packageNameField.IsEnabled() && !String.IsNullOrEmpty(packageNameField.GetMetadata("Value")))
            {
                LogDebug("Package name found, check against resource value '{0}'", packageNameField.GetMetadata("Value"));

                if (plist.ContainsKey("CFBundleName"))
                {
                    var bundleName = (string)plist["CFBundleName"];
                    if (bundleName != packageNameField.GetMetadata("Value"))
                    {
                        plist["CFBundleName"] = packageNameField.GetMetadata("Value");
                        LogInformation("Package name / Bundle name changed to '{0}', setting Plist", plist["CFBundleName"]);
                        touched = true;
                    }
                    else
                    {
                        LogInformation("Package name / Bundle name unchanged, skipping");
                    }
                }
                else
                {
                    LogWarning("Package name / Bundle name not found in Plist");
                }

                if (plist.ContainsKey("CFBundleDisplayName"))
                {
                    var bundleDisplayName = (string)plist["CFBundleDisplayName"];
                    if (bundleDisplayName != packageNameField.GetMetadata("Value"))
                    {
                        plist["CFBundleDisplayName"] = packageNameField.GetMetadata("Value");
                        LogInformation("Package name / Bundle display name changed to '{0}', setting Plist", plist["CFBundleDisplayName"]);
                        touched = true;
                    }
                    else
                    {
                        LogInformation("Package name / Bundle display name unchanged, skipping");
                    }
                }
                else
                {
                    plist.Add("CFBundleDisplayName", packageNameField.GetMetadata("Value"));
                    touched = true;
                    LogInformation("Package name / Bundle display name not found in Plist, creating with value '{0}'", plist["CFBundleDisplayName"]);
                }
            }
            else if (packageNameField != null && packageNameField.IsDisabled())
            {
                LogWarning("Package name is disabled, it will not be set in plist");
            }
            else
            {
                LogWarning("Package name not found in packaging fields");
            }

            return touched;
        }

        private bool Identifier(bool touched, Dictionary<string, object> plist)
        {
            var packageIdentifierField = PackagingFields
                .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingIosIdentifier);

            if (packageIdentifierField != null && packageIdentifierField.IsEnabled() && !String.IsNullOrEmpty(packageIdentifierField.GetMetadata("Value")))
            {
                if (plist.ContainsKey("CFBundleIdentifier"))
                {
                    var bundleIdentifier = (string)plist["CFBundleIdentifier"];
                    if (bundleIdentifier != packageIdentifierField.GetMetadata("Value"))
                    {
                        plist["CFBundleIdentifier"] = packageIdentifierField.GetMetadata("Value");
                        LogInformation("Package identifier changed to '{0}', setting Plist", plist["CFBundleIdentifier"]);
                        touched = true;
                    }
                    else
                    {
                        LogInformation("Package identifier unchanged, skipping");
                    }
                }
                else
                {
                    plist.Add("CFBundleIdentifier", packageIdentifierField.GetMetadata("Value"));
                    touched = true;
                    LogInformation("Package identifier not found in Plist, creating with value '{0}'", plist["CFBundleIdentifier"]);
                }
            }
            else if (packageIdentifierField != null && packageIdentifierField.IsDisabled())
            {
                LogWarning("Package identifier is disabled, it will not be set in plist");
            }
            else
            {
                LogWarning("Package identifier not found in packaging fields");
            }

            return touched;
        }

        private bool VersionText(bool touched, Dictionary<string, object> plist)
        {
            var packageVersionTextField = PackagingFields
                    .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingIosVersionText);

            if (packageVersionTextField != null && packageVersionTextField.IsEnabled() && !String.IsNullOrEmpty(packageVersionTextField.GetMetadata("Value")))
            {
                LogDebug("Package version text found, check against resource value {0}", packageVersionTextField.GetMetadata("Value"));

                if (plist.ContainsKey("CFBundleShortVersionString"))
                {
                    var bundleVersionNumber = (string)plist["CFBundleShortVersionString"];
                    if (bundleVersionNumber != packageVersionTextField.GetMetadata("Value"))
                    {
                        plist["CFBundleShortVersionString"] = packageVersionTextField.GetMetadata("Value");
                        LogInformation("Package version text / Bundle short version string changed to '{0}', setting Plist", plist["CFBundleShortVersionString"]);
                        touched = true;
                    }
                    else
                    {
                        LogInformation("Package version text / Bundle short version string unchanged, skipping");
                    }
                }
                else
                {
                    plist.Add("CFBundleShortVersionString", packageVersionTextField.GetMetadata("Value"));
                    touched = true;
                    LogInformation("Package version text / Bundle short version string not found in Plist, creating with value '{0}'", plist["CFBundleVersion"]);
                }
            }

            else if (packageVersionTextField != null && packageVersionTextField.IsDisabled())
            {
                LogWarning("Package version text is disabled, it will not be set in plist");
            }
            else
            {
                LogWarning("Package version text not found in packaging fields");
            }

            return touched;
        }

        private bool VersionNumber(bool touched, Dictionary<string, object> plist)
        {
            var packageVersionNumberField = PackagingFields
                .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingIosVersionNumber);

            if (packageVersionNumberField != null && packageVersionNumberField.IsEnabled() && !String.IsNullOrEmpty(packageVersionNumberField.GetMetadata("Value")))
            {
                LogDebug("Package version number found, check against resource value {0}", packageVersionNumberField.GetMetadata("Value"));

                if (plist.ContainsKey("CFBundleVersion"))
                {
                    var bundleVersionNumber = (string)plist["CFBundleVersion"];
                    if (bundleVersionNumber != packageVersionNumberField.GetMetadata("Value"))
                    {
                        plist["CFBundleVersion"] = packageVersionNumberField.GetMetadata("Value");
                        LogInformation("Package version number / Bundle version changed to '{0}', setting Plist", plist["CFBundleVersion"]);
                        touched = true;
                    }
                    else
                    {
                        LogInformation("Package version number / Bundle version unchanged, skipping");
                    }
                }
                else
                {
                    plist.Add("CFBundleVersion", packageVersionNumberField.GetMetadata("Value"));
                    touched = true;
                    LogInformation("Package version number / Bundle version not found in Plist, creating with value '{0}'", plist["CFBundleVersion"]);
                }
            }
            else if (packageVersionNumberField != null && packageVersionNumberField.IsDisabled())
            {
                LogWarning("Package name is disabled, it will not be set in plist");
            }
            else
            {
                LogWarning("Package version number not found in packaging fields");
            }

            return touched;
        }

        private bool AppIconCatalogueSetName(bool touched, Dictionary<string, object> plist, ITaskItem assetCatalogueNameField)
        {
            var appIconCatalogueNameField = PackagingFields
                .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingIosAppIconXcAssetsName);

            if (assetCatalogueNameField != null
                && assetCatalogueNameField.IsEnabled()
                && !String.IsNullOrEmpty(assetCatalogueNameField.GetMetadata("Value"))
                && appIconCatalogueNameField != null
                && appIconCatalogueNameField.IsEnabled()
                && !String.IsNullOrEmpty(appIconCatalogueNameField.GetMetadata("Value"))
               )
            {
                var appIconSetPath = Path.Combine(assetCatalogueNameField.GetMetadata("Value").ApplyXcAssetsExt()
                                              , appIconCatalogueNameField.GetMetadata("Value").ApplyAppiconsetExt());

                LogDebug("Package app icon catalogue found, check against resource value {0}", appIconSetPath);

                if (plist.ContainsKey("XSAppIconAssets"))
                {
                    var xsAppIconAssets = (string)plist["XSAppIconAssets"];
                    if (xsAppIconAssets != appIconSetPath)
                    {
                        plist["XSAppIconAssets"] = appIconSetPath;
                        LogInformation("Package app icon catalogue changed to '{0}', setting Plist", plist["XSAppIconAssets"]);
                        touched = true;
                    }
                    else
                    {
                        LogInformation("Package app icon catalogue unchanged, skipping");
                    }
                }
                else
                {
                    plist.Add("XSAppIconAssets", appIconSetPath);
                    touched = true;
                    LogInformation("Package app icon catalogue not found in Plist, creating with value '{0}'", plist["XSAppIconAssets"]);
                }
            }

            else if (assetCatalogueNameField != null && assetCatalogueNameField.IsDisabled())
            {
                LogWarning("Asset catalogue name is disabled, cannot set app icon catalogue set name in plist");
            }
            else if (appIconCatalogueNameField != null && appIconCatalogueNameField.IsDisabled())
            {
                LogWarning("AppIcon catalogue set name is disabled, cannot set app icon catalogue set name in plist");
            }
            else
            {
                LogWarning("Package app icon catalogue not found in packaging fields");
            }

            return touched;
        }

        private bool LaunchCatalogueSetName(bool touched, Dictionary<string, object> plist, ITaskItem assetCatalogueNameField)
        {
            var launchCatalogueNameField = PackagingFields
                .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingIosLaunchImageXcAssetsName);

            if (assetCatalogueNameField != null
                && assetCatalogueNameField.IsEnabled()
                && !String.IsNullOrEmpty(assetCatalogueNameField.GetMetadata("Value"))
                && launchCatalogueNameField != null
                && launchCatalogueNameField.IsEnabled()
                && !String.IsNullOrEmpty(launchCatalogueNameField.GetMetadata("Value"))
               )
            {
                var setPath = Path.Combine(assetCatalogueNameField.GetMetadata("Value").ApplyXcAssetsExt()
                                                  , launchCatalogueNameField.GetMetadata("Value").ApplyLaunchimageExt());

                LogDebug("Package image catalogue found, check against resource value {0}", setPath);

                if (plist.ContainsKey("XSLaunchImageAssets"))
                {
                    var plistKey = (string)plist["XSLaunchImageAssets"];
                    if (plistKey != setPath)
                    {
                        plist["XSLaunchImageAssets"] = setPath;
                        LogInformation("Package image catalogue changed to '{0}', setting Plist", plist["XSLaunchImageAssets"]);
                        touched = true;
                    }
                    else
                    {
                        LogInformation("Package app icon catalogue unchanged, skipping");
                    }
                }
                else
                {
                    plist.Add("XSLaunchImageAssets", setPath);
                    touched = true;
                    LogInformation("Package image catalogue not found in Plist, creating with value '{0}'", plist["XSLaunchImageAssets"]);
                }
            }

            else if (assetCatalogueNameField != null && assetCatalogueNameField.IsDisabled())
            {
                LogWarning("Asset catalogue name is disabled, cannot set launch image catalogue set name in plist");
            }
            //remove key for launch image catalogue if using storyboard - maybe, or maybe just make sure it's set, and
            //if a launchstoryboard name is set it'll overwrite it. then we could drop that field
            else if ((launchCatalogueNameField != null && launchCatalogueNameField.IsDisabled()))
            {
                if (plist.ContainsKey("XSLaunchImageAssets"))
                {
                    plist.Remove("XSLaunchImageAssets");
                    touched = true;
                    LogWarning("Launch image catalogue set name is disabled, removing from plist");
                }
                else
                {

                    LogWarning("Launch image catalogue set name is disabled, but not set in plist, no changing");
                }
            }
            else
            {
                LogWarning("Package launch image catalogue not found in packaging fields");
            }

            return touched;
        }

        private bool LaunchStoryboardName(bool touched, Dictionary<string, object> plist, bool useLaunchStoryboard)
        {
            var launchStoryboardNameField = PackagingFields
                .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingIosLaunchStoryboardName);

            if (launchStoryboardNameField != null
                && launchStoryboardNameField.IsEnabled()
                && !String.IsNullOrEmpty(launchStoryboardNameField.GetMetadata("Value"))
               )
            {
                var launchStoryboardName = launchStoryboardNameField.GetMetadata("Value");

                LogDebug("Package launch storyboard found, check against resource value {0}", launchStoryboardName);

                if (plist.ContainsKey("UILaunchStoryboardName"))
                {
                    if (useLaunchStoryboard)
                    {
                        var plistKey = (string)plist["UILaunchStoryboardName"];
                        if (plistKey != launchStoryboardName)
                        {
                            plist["UILaunchStoryboardName"] = launchStoryboardName;
                            LogInformation("Package launch storyboard changed to '{0}', setting Plist", plist["UILaunchStoryboardName"]);
                            touched = true;
                        }
                        else
                        {
                            LogInformation("Package launch storyboard unchanged, skipping");
                        }
                    } else {
                        plist.Remove("UILaunchStoryboardName");
                        touched = true;
                        LogInformation("Package launch storyboard useLanuchStoryboard false, but key exists, removing from plist");
                    }
                }
                else
                {
                    if (useLaunchStoryboard)
                    {
                        plist.Add("UILaunchStoryboardName", launchStoryboardName);
                        touched = true;
                        LogInformation("Package launch storyboard not found in Plist, creating with value '{0}'", plist["UILaunchStoryboardName"]);
                    } else {
                        LogInformation("Package launch storyboard useLanuchStoryboard false, not adding to plist");
                    }
                }
            }

            else if (launchStoryboardNameField != null && launchStoryboardNameField.IsDisabled())
            {
                LogWarning("Launch launch storyboard is disabled, but not set in plist, no changing");

            }
            else
            {
                LogWarning("Package launch storyboard not found in packaging fields");
            }

            return touched;
        }

        private bool UsesNonExemptEncryption(bool touched, Dictionary<string, object> plist)
        {
            var usesNonExemptEncryptionField = PackagingFields
                .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingIosUsesNonExemptEncryption);

            if (usesNonExemptEncryptionField != null
                && usesNonExemptEncryptionField.IsEnabled()
                && !String.IsNullOrEmpty(usesNonExemptEncryptionField.GetMetadata("Value"))
               )
            {
                var usesNonExemptEncryption = usesNonExemptEncryptionField.IsTrue(this);

                LogDebug("Package uses non exempt encryption found, check against resource value {0}", usesNonExemptEncryption);

                if (plist.ContainsKey("ITSAppUsesNonExemptEncryption"))
                {
                    var plisyValue = (bool)plist["ITSAppUsesNonExemptEncryption"];
                    if (plisyValue != usesNonExemptEncryption)
                    {
                        plist["ITSAppUsesNonExemptEncryption"] = usesNonExemptEncryption;
                        LogInformation("Package uses non exempt encryption changed to '{0}', setting Plist", plist["ITSAppUsesNonExemptEncryption"]);
                        touched = true;
                    }
                    else
                    {
                        LogInformation("Package uses non exempt encryption unchanged, skipping");
                    }
                }
                else
                {
                    plist.Add("ITSAppUsesNonExemptEncryption", usesNonExemptEncryption);
                    touched = true;
                    LogInformation("Package uses non exempt encryption not found in Plist, creating with value '{0}'", plist["ITSAppUsesNonExemptEncryption"]);
                }
            }

            else if ((usesNonExemptEncryptionField != null && usesNonExemptEncryptionField.IsDisabled()))
            {
                LogWarning("Launch uses non exempt encryption is disabled, but not set in plist, no changing");

            }
            else
            {
                LogWarning("Package luses non exempt encryption not found in packaging fields");
            }

            return touched;
        }
    }


}
