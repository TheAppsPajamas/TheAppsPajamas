using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Build.Client.Extensions;
using Build.Client.Helpers;
using Build.Shared.Types;
using Microsoft.Build.Framework;

namespace Build.Client.BuildTasks
{
    public class SetIosPlist : BaseTask
    {
        public string IosPlist { get; set; }

        public ITaskItem[] PackagingFields { get; set; }

        public override bool Execute()
        {
            Log.LogMessage("Setting Ios Plist file");

            if (String.IsNullOrEmpty(IosPlist))
            {
                Log.LogMessage("Ios Plist file name empty, aborting SetIosPlist as ran successful");
                return true;
            }

            LogDebug("Plist file name '{0}'", IosPlist);
            LogDebug("Packaging fields: '{0}'", PackagingFields.Count());

            try
            {
                bool touched = false;
                var plist = (Dictionary<string, object>)Plist.readPlist(IosPlist);
                LogDebug("Plist {0} read, {1} nodes", IosPlist, plist.Count());

                var packageName = PackagingFields
                    .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingIosName);

                if (packageName != null && !String.IsNullOrEmpty(packageName.GetMetadata("Value")))
                {
                    LogDebug("Package name found, check against resource value '{0}'", packageName.GetMetadata("Value"));

                    if (plist.ContainsKey("CFBundleName")){
                        var bundleName = (string)plist["CFBundleName"];
                        if (bundleName != packageName.GetMetadata("Value")){
                            plist["CFBundleName"] = packageName.GetMetadata("Value");
                            Log.LogMessage("Package name / Bundle name changed to '{0}', setting Plist", plist["CFBundleName"]);
                            touched = true;
                        } else {
                            Log.LogMessage("Package name / Bundle name unchanged, skipping");
                        }
                    } else {
                        Log.LogWarning("Package name / Bundle name not found in Plist");
                    }

                    if (plist.ContainsKey("CFBundleDisplayName"))
                    {
                        var bundleDisplayName = (string)plist["CFBundleDisplayName"];
                        if (bundleDisplayName != packageName.GetMetadata("Value"))
                        {
                            plist["CFBundleDisplayName"] = packageName.GetMetadata("Value");
                            Log.LogMessage("Package name / Bundle display name changed to '{0}', setting Plist", plist["CFBundleDisplayName"]);
                            touched = true;
                        }
                        else
                        {
                            Log.LogMessage("Package name / Bundle display name unchanged, skipping");
                        }
                    }
                    else
                    {
                        plist.Add("CFBundleDisplayName", packageName.GetMetadata("Value"));
                        touched = true;
                        Log.LogMessage("Package name / Bundle display name not found in Plist, creating with value '{0}'", plist["CFBundleDisplayName"]);
                    }
                }
                else
                {
                    Log.LogWarning("Package name not found in packaging fields");
                }

                var packageIdentifier = PackagingFields
                    .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingIosIdentifier);

                if (packageIdentifier != null && !String.IsNullOrEmpty(packageIdentifier.GetMetadata("Value")))
                {
                    if (plist.ContainsKey("CFBundleIdentifier"))
                    {
                        var bundleIdentifier = (string)plist["CFBundleIdentifier"];
                        if (bundleIdentifier != packageIdentifier.GetMetadata("Value"))
                        {
                            plist["CFBundleIdentifier"] = packageIdentifier.GetMetadata("Value");
                            Log.LogMessage("Package identifier changed to '{0}', setting Plist", plist["CFBundleIdentifier"]);
                            touched = true;
                        }
                        else
                        {
                            Log.LogMessage("Package identifier unchanged, skipping");
                        }
                    }
                    else
                    {
                        plist.Add("CFBundleIdentifier", packageIdentifier.GetMetadata("Value"));
                        touched = true;
                        Log.LogMessage("Package identifier not found in Plist, creating with value '{0}'", plist["CFBundleIdentifier"]);
                    }
                }
                else
                {
                    Log.LogWarning("Package identifier not found in packaging fields");
                }

                var packageVersionText = PackagingFields
                        .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingIosVersionText);

                if (packageVersionText != null && !String.IsNullOrEmpty(packageVersionText.GetMetadata("Value")))
                {
                    LogDebug("Package version text found, check against resource value {0}", packageVersionText.GetMetadata("Value"));

                    if (plist.ContainsKey("CFBundleShortVersionString"))
                    {
                        var bundleVersionNumber = (string)plist["CFBundleShortVersionString"];
                        if (bundleVersionNumber != packageVersionText.GetMetadata("Value"))
                        {
                            plist["CFBundleShortVersionString"] = packageVersionText.GetMetadata("Value");
                            Log.LogMessage("Package version text / Bundle short version string changed to '{0}', setting Plist", plist["CFBundleShortVersionString"]);
                            touched = true;
                        }
                        else
                        {
                            Log.LogMessage("Package version text / Bundle short version string unchanged, skipping");
                        }
                    }
                    else
                    {
                        plist.Add("CFBundleShortVersionString", packageVersionText.GetMetadata("Value"));
                        touched = true;
                        Log.LogMessage("Package version text / Bundle short version string not found in Plist, creating with value '{0}'", plist["CFBundleVersion"]);
                    }
                }
                else
                {
                    Log.LogWarning("Package version text not found in packaging fields");
                }

                var packageVersionNumber = PackagingFields
                    .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingIosVersionNumber);

                if (packageVersionNumber != null && !String.IsNullOrEmpty(packageVersionNumber.GetMetadata("Value")))
                {
                    LogDebug("Package version number found, check against resource value {0}", packageVersionNumber.GetMetadata("Value"));

                    if (plist.ContainsKey("CFBundleVersion"))
                    {
                        var bundleVersionNumber = (string)plist["CFBundleVersion"];
                        if (bundleVersionNumber != packageVersionNumber.GetMetadata("Value"))
                        {
                            plist["CFBundleVersion"] = packageVersionNumber.GetMetadata("Value");
                            Log.LogMessage("Package version number / Bundle version changed to '{0}', setting Plist", plist["CFBundleVersion"]);
                            touched = true;
                        }
                        else
                        {
                            Log.LogMessage("Package version number / Bundle version unchanged, skipping");
                        }
                    }
                    else
                    {
                        plist.Add("CFBundleVersion", packageVersionNumber.GetMetadata("Value"));
                        touched = true;
                        Log.LogMessage("Package version number / Bundle version not found in Plist, creating with value '{0}'", plist["CFBundleVersion"]);
                    }
                }
                else
                {
                    Log.LogWarning("Package version number not found in packaging fields");
                }

                var assetCatalogueName = PackagingFields
                    .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingIosAssetCatalogueName);

                var appIconCatalogueName = PackagingFields
                    .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingIosAppIconXcAssetsName);

                if (assetCatalogueName != null && !String.IsNullOrEmpty(assetCatalogueName.GetMetadata("Value"))
                    && appIconCatalogueName != null && !String.IsNullOrEmpty(appIconCatalogueName.GetMetadata("Value"))
                   )
                {
                    var appIconSetPath = Path.Combine(assetCatalogueName.GetMetadata("Value").ApplyXcAssetsExt()
                                                  , appIconCatalogueName.GetMetadata("Value").ApplyAppiconsetExt());

                    LogDebug("Package app icon catalogue found, check against resource value {0}", appIconSetPath);

                    if (plist.ContainsKey("XSAppIconAssets"))
                    {
                        var xsAppIconAssets = (string)plist["XSAppIconAssets"];
                        if (xsAppIconAssets != appIconSetPath)
                        {
                            plist["XSAppIconAssets"] = appIconSetPath;
                            Log.LogMessage("Package app icon catalogue changed to '{0}', setting Plist", plist["XSAppIconAssets"]);
                            touched = true;
                        }
                        else
                        {
                            Log.LogMessage("Package app icon catalogue unchanged, skipping");
                        }
                    }
                    else
                    {
                        plist.Add("XSAppIconAssets", appIconSetPath); 
                        touched = true;
                        Log.LogMessage("Package app icon catalogue not found in Plist, creating with value '{0}'", plist["XSAppIconAssets"]);
                    }
                }
                else
                {
                    Log.LogWarning("Package app icon catalogue not found in packaging fields");
                }

                if (touched)
                {
                    Log.LogMessage("Plist touched, saving");
                    Plist.writeXml(plist, IosPlist);
                }
                else
                {
                    Log.LogMessage("Plist untouched, skipping");
                }

            } catch (Exception ex){
                Log.LogErrorFromException(ex);
                return false;
            }

            return true;
        }
    }


}
