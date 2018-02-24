using System;
using System.IO;
using System.Linq;
using TheAppsPajamas.Client.Constants;
using TheAppsPajamas.Client.Extensions;

namespace TheAppsPajamas.Client.Tasks
{
    public class CleanAssetCatalogues : BaseTask
    {
        public string TargetsDir { get; set; }

        public override bool Execute()
        {
            var baseResult = base.Execute();

            LogInformation("Cleaning asset catalogues");

            try
            {
                LogDebug("Targets folder {0}", TargetsDir);
                var catalogues = Directory.EnumerateDirectories(TargetsDir, "*.xcassets");

                LogDebug("Found {0} asset catalogue folders", catalogues.Count());

                foreach (var catalogue in catalogues)
                {
                    LogInformation("Deleting asset catalogue folder {0}", catalogue);
                    Directory.Delete(catalogue, true);
                }

                if (catalogues.Any() == false){
                    LogInformation("No asset catalogues found to clean");
                }
                return true;
            } catch (Exception ex){
                Log.LogErrorFromException(ex);
                return false;
            }
        }
    }
}
