using System;
using System.Linq;
using Microsoft.Build.Framework;
using System.Xml;
using DAL.Enums;

namespace Build.Client.BuildTasks
{
    public class SetDroidManifest : BaseTask
    {
        private const string AndroidNamespace = "http://schemas.android.com/apk/res/android";

        public string AndroidManifest { get; set; }

        public ITaskItem[] PackagingFields { get; set; }

        public override bool Execute()
        {
            Log.LogMessage("Setting Droid manifest file");

            LogDebug("Manifest file name '{0}'", AndroidManifest);
            LogDebug("Packaging fields: '{0}'", PackagingFields.Count());

            if (String.IsNullOrEmpty(AndroidManifest)){
                Log.LogMessage("Android manifest file name empty, aborting SetDroidManifest as ran successful");
                return true;
            }

            try
            {
                var xml = new XmlDocument();
                bool touched = false;
                xml.Load(AndroidManifest);

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xml.NameTable);
                nsmgr.AddNamespace("android", AndroidNamespace);

                if (xml.DocumentElement == null)
                {
                    LogDebug("Android Manifest not readable");
                    return false;
                }

                var packageName = PackagingFields
                    .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingDroidName);

                if (packageName != null && !String.IsNullOrEmpty(packageName.GetMetadata("Value")))
                {
                    LogDebug("Package name found, setting manifest to {0}", packageName.GetMetadata("Value"));

                    var appNode = xml.DocumentElement.SelectSingleNode("/manifest/application", nsmgr);
                    if (appNode != null && appNode.Attributes != null)
                    {
                        var labelAttribute = appNode.Attributes["label", AndroidNamespace];
                        if (labelAttribute != null)
                        {
                            labelAttribute.Value = packageName.GetMetadata("Value");
                            touched = true;
                        }
                        else
                        {
                            LogDebug("Package label attribute not found");
                        }
                    }
                    else
                    {
                        LogDebug("Package label attribute not found");
                    }
                }
                else
                {
                    LogDebug("Package name not found");
                }

                var packageIdentifier = PackagingFields
                    .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingDroidIdentifier);
                
                if (packageIdentifier != null && !String.IsNullOrEmpty(packageIdentifier.GetMetadata("Value"))){
                    LogDebug("Package identifier found, setting manifest to {0}", packageIdentifier.GetMetadata("Value"));
                    xml.DocumentElement.SetAttribute("package", packageIdentifier.GetMetadata("Value"));
                    touched = true;
                } else {
                    LogDebug("Package identifier not found");
                }

                var packageVersionName = PackagingFields
                    .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingDroidVersionText);

                if (packageVersionName != null && !String.IsNullOrEmpty(packageVersionName.GetMetadata("Value")))
                {
                    LogDebug("Package version name found, setting manifest to {0}", packageVersionName.GetMetadata("Value"));
                    xml.DocumentElement.SetAttribute("android:versionName", packageVersionName.GetMetadata("Value"));
                    touched = true;
                }
                else
                {
                    LogDebug("Package version name not found");
                }

                var packageVersionCode = PackagingFields
                    .FirstOrDefault(x => FieldType.FromValue(Int32.Parse(x.ItemSpec)) == FieldType.PackagingDroidVersionNumber);

                if (packageVersionCode != null && !String.IsNullOrEmpty(packageVersionCode.GetMetadata("Value")))
                {
                    LogDebug("Package version code found, setting manifest to {0}", packageVersionCode.GetMetadata("Value"));
                    xml.DocumentElement.SetAttribute("android:versionCode", packageVersionCode.GetMetadata("Value"));
                    touched = true;
                }
                else
                {
                    LogDebug("Package version code not found");
                }

                if (touched){
                    LogDebug("Manifest touched, saving");
                    xml.Save(AndroidManifest);
                } else {
                    LogDebug("Manifest untouched, not saving");
                }

                return true;
            } catch (Exception ex){
                Log.LogErrorFromException(ex);
            }
            return true;
        }
    }
}
