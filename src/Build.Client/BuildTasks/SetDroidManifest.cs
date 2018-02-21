using System;
using System.Linq;
using Microsoft.Build.Framework;
using System.Xml;
using Build.Shared.Types;
using Build.Client.Extensions;

namespace Build.Client.BuildTasks
{
    public class SetDroidManifest : BaseTask
    {
        private const string AndroidNamespace = "http://schemas.android.com/apk/res/android";

        public string AndroidManifest { get; set; }

        public ITaskItem[] PackagingFields { get; set; }

        public ITaskItem PackagingHolder { get; set; }

        public override bool Execute()
        {
            Log.LogMessage("Setting Droid manifest file");

            if (PackagingHolder.IsDisabled())
            {
                Log.LogMessage($"Packaging is disabled in this configuration, Android Manifest will not be set");
                return true;
            }

            if (String.IsNullOrEmpty(AndroidManifest))
            {
                Log.LogMessage("Android manifest file name empty, aborting SetDroidManifest as ran successful");
                return true;
            }

            LogDebug("Manifest file name '{0}'", AndroidManifest);
            LogDebug("Packaging fields: '{0}'", PackagingFields.Count());



            try
            {
                var xml = new XmlDocument();
                bool touched = false;
                xml.Load(AndroidManifest);

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xml.NameTable);
                nsmgr.AddNamespace("android", AndroidNamespace);

                if (xml.DocumentElement == null)
                {
                    Log.LogError("Android Manifest not readable");
                    return false;
                }


                var appNode = xml.DocumentElement.SelectSingleNode("/manifest/application", nsmgr);
                if (appNode != null && appNode.Attributes != null)
                {
                    touched = BundleName(touched, appNode);
                    //icon
                    touched = LaunchIcon(touched, appNode);

                }
                else
                {
                    Log.LogWarning("Android Manifest Application node not found");
                }

                touched = BundleIdentifier(xml, touched);

                touched = VersionText(xml, touched);

                touched = VersionCode(xml, touched);

                if (touched)
                {
                    Log.LogMessage("Manifest touched, saving");
                    xml.Save(AndroidManifest);
                }
                else
                {
                    Log.LogMessage("Manifest untouched, skipping");
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
            }
            return true;
        }

        private bool VersionCode(XmlDocument xml, bool touched)
        {
            var packageVersionCodeField = PackagingFields
                .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingDroidVersionNumber);

            if (packageVersionCodeField != null && packageVersionCodeField.IsEnabled() && !String.IsNullOrEmpty(packageVersionCodeField.GetMetadata("Value")))
            {
                LogDebug("Package version code found, check against resource value {0}", packageVersionCodeField.GetMetadata("Value"));
                if (xml.DocumentElement.GetAttribute("android:versionCode") != packageVersionCodeField.GetMetadata("Value"))
                {
                    LogDebug("Package version code changed, setting Android Manifest");
                    xml.DocumentElement.SetAttribute("android:versionCode", packageVersionCodeField.GetMetadata("Value"));
                    touched = true;
                }
                else
                {
                    LogDebug("Package version code unchanged, skipping");
                }
            }
            else if (packageVersionCodeField != null && packageVersionCodeField.IsDisabled())
            {
                Log.LogWarning("Package version code is disabled, it will not be set in Android Manifest");
            }
            else
            {
                Log.LogWarning("Package version code not found in packaging fields");
            }

            return touched;
        }

        private bool VersionText(XmlDocument xml, bool touched)
        {
            var packageVersionNameField = PackagingFields
                .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingDroidVersionText);

            if (packageVersionNameField != null && packageVersionNameField.IsEnabled() && !String.IsNullOrEmpty(packageVersionNameField.GetMetadata("Value")))
            {
                LogDebug("Package version name found, check against resource value {0}", packageVersionNameField.GetMetadata("Value"));
                if (xml.DocumentElement.GetAttribute("android:versionName") != packageVersionNameField.GetMetadata("Value"))
                {
                    LogDebug("Package version name changed, setting Android Manifest");
                    xml.DocumentElement.SetAttribute("android:versionName", packageVersionNameField.GetMetadata("Value"));
                    touched = true;
                }
                else
                {
                    LogDebug("Package version name unchanged, skipping");
                }
            }
            else if (packageVersionNameField != null && packageVersionNameField.IsDisabled())
            {
                Log.LogWarning("Package version name is disabled, it will not be set in Android Manifest");
            }
            else
            {
                Log.LogWarning("Package version name not found in packaging fields");
            }

            return touched;
        }

        private bool BundleIdentifier(XmlDocument xml, bool touched)
        {
            var packageIdentifierField = PackagingFields
                .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingDroidIdentifier);

            if (packageIdentifierField != null && packageIdentifierField.IsEnabled() && !String.IsNullOrEmpty(packageIdentifierField.GetMetadata("Value")))
            {
                LogDebug("Package identifier found, check against resource value {0}", packageIdentifierField.GetMetadata("Value"));
                if (xml.DocumentElement.GetAttribute("package") != packageIdentifierField.GetMetadata("Value"))
                {
                    LogDebug("Package identified changed, setting Android Manifest");
                    xml.DocumentElement.SetAttribute("package", packageIdentifierField.GetMetadata("Value"));
                    touched = true;
                }
                else
                {
                    LogDebug("Package identifier unchanged, skipping");
                }
            }
            else if (packageIdentifierField != null && packageIdentifierField.IsDisabled())
            {
                Log.LogWarning("Package identifier is disabled, it will not be set in Android Manifest");
            }
            else
            {
                Log.LogWarning("Package identifier not found in packaging fields");
            }

            return touched;
        }

        private bool LaunchIcon(bool touched, XmlNode appNode)
        {
            var iconNameField = PackagingFields
                .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingDroidAppIconName);

            if (iconNameField != null && iconNameField.IsEnabled() && !String.IsNullOrEmpty(iconNameField.GetMetadata("Value")))
            {
                LogDebug("Package app icon found, check against resource value {0}", iconNameField.GetMetadata("Value"));

                var iconAttribute = appNode.Attributes["icon", AndroidNamespace];
                if (iconAttribute != null)
                {
                    if (iconAttribute.Value != iconNameField.GetMetadata("Value"))
                    {
                        LogDebug("Package app icon changed, setting Android Manifest");
                        iconAttribute.Value = String.Concat("@mipmap/", iconNameField.GetMetadata("Value"));
                        touched = true;
                    }
                    else
                    {
                        LogDebug("Package app icon unchanged, skipping");
                    }
                }
                else
                {
                    Log.LogWarning("Package app icon attribute not found in Android Manifest");
                }
            }
            else if (iconNameField != null && iconNameField.IsDisabled())
            {
                Log.LogWarning("Package app icon is disabled, it will not be set in Android Manifest");
            }
            else
            {
                Log.LogWarning("Package app icon not found in packaging fields");
            }

            return touched;
        }

        private bool BundleName(bool touched, XmlNode appNode)
        {
            var packageNameField = PackagingFields
                .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingDroidName);

            if (packageNameField != null && packageNameField.IsEnabled() && !String.IsNullOrEmpty(packageNameField.GetMetadata("Value")))
            {
                LogDebug("Package name found, check against resource value {0}", packageNameField.GetMetadata("Value"));

                var labelAttribute = appNode.Attributes["label", AndroidNamespace];
                if (labelAttribute != null)
                {
                    if (labelAttribute.Value != packageNameField.GetMetadata("Value"))
                    {
                        LogDebug("Package name changed, setting Android Manifest");
                        labelAttribute.Value = packageNameField.GetMetadata("Value");
                        touched = true;
                    }
                    else
                    {
                        LogDebug("Package label unchanged, skipping");
                    }
                }
                else
                {
                    Log.LogWarning("Package label attribute not found in Android Manifest");
                }
            }
            else if (packageNameField != null && packageNameField.IsDisabled())
            {
                Log.LogWarning("Package name is disabled, it will not be set in Android Manifest");
            }
            else
            {
                Log.LogWarning("Package name not found in packaging fields");
            }

            return touched;
        }
    }
}
