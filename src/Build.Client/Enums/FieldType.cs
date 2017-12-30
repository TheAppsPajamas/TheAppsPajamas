using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Enums
{
    public class FieldType : Enumeration<FieldType>
    {
        private ProjectType _projectType;
        public ProjectType ProjectType { get { return _projectType; } }

        private FieldHolderType _fieldHolderType;
        public FieldHolderType FieldHolderType { get { return _fieldHolderType; } }

        private bool _isForClient;
        public bool IsForClient { get { return _isForClient; } }

        public static readonly AppIconFieldType AppIconSharedMaster 
            = new AppIconFieldType.Shared(-1, "Shared master app icon", true, false, "shared-master.png", 1024);

        public static readonly AppIconFieldType AppIconDroidMaster
            = new AppIconFieldType.Droid(-5, "Droid master app icon", true, false, "droid-master.png", 1024);
        public static readonly AppIconFieldType AppIconDroidPlaystore
            = new AppIconFieldType.Droid(-6, "Droid playstore app icon", false, false, "droid-playstore.png", 1024);
        public static readonly AppIconFieldType AppIconDroidLdpi
            = new AppIconFieldType.Droid(-7, "Droid ldpi app icon", false, true, "mipmap-ldpi", 36);
        public static readonly AppIconFieldType AppIconDroidMdpi
            = new AppIconFieldType.Droid(-8, "Droid mdpi app icon", false, true, "mipmap-mdpi", 48);
        public static readonly AppIconFieldType AppIconDroidHdpi
            = new AppIconFieldType.Droid(-9, "Droid hdpi app icon", false, true, "mipmap-hdpi", 72);
        public static readonly AppIconFieldType AppIconDroidXhdpi
            = new AppIconFieldType.Droid(-10, "Droid xhdpi app icon", false, true, "mipmap-xhdpi", 96);
        public static readonly AppIconFieldType AppIconDroidXxhdpi
            = new AppIconFieldType.Droid(-11, "Droid xxhdpi app icon", false, true, "mipmap-xxhdpi", 144);
        public static readonly AppIconFieldType AppIconDroidXxxhdpi
            = new AppIconFieldType.Droid(-12, "Droid xxxhdpi app icon", false, true, "mipmap-xxxhdpi", 192);
        
        public static readonly AppIconFieldType AppIconIosMaster
            = new AppIconFieldType.Ios(-15, "Ios master app icon", true, false, "ios-master.png", 1024);

        public static readonly AppIconFieldType AppIconIosITunesArtwork
            = new AppIconFieldType.Ios(-16, "Ios iTunesArtwork", false, true, "iTunesArtwork", 512);
        public static readonly AppIconFieldType AppIconIosITunesArtwork_2x
            = new AppIconFieldType.Ios(-17, "Ios iTunesArtwork@2x", false, true, "iTunesArtwork@2x", 1024);

        public static readonly AppIconFieldType AppIconIosMarketingIcon
            = new AppIconFieldType.Ios(-18, "Ios marketing icon", false, true, "Icon-Marketing.png", 1024);

        public static readonly AppIconFieldType AppIconIosIcon60_2x
            = new AppIconFieldType.Ios(-19, "Ios icon-60@2x", false, true, "Icon-60@2x.png", 120);
        public static readonly AppIconFieldType AppIconIosIcon60_3x
            = new AppIconFieldType.Ios(-20, "Ios icon-60@3x", false, true, "Icon-60@3x.png", 180);

        public static readonly AppIconFieldType AppIconIosIcon76
            = new AppIconFieldType.Ios(-21, "Ios icon-76", false, true, "Icon-76.png", 76);
        public static readonly AppIconFieldType AppIconIosIcon76_2x
            = new AppIconFieldType.Ios(-22, "Ios icon-76@2x", false, true, "Icon-76@2x.png", 152);
        
        public static readonly AppIconFieldType AppIconIosIcon83_5_2x
            = new AppIconFieldType.Ios(-23, "Ios icon-83.5@2x", false, true, "Icon-83.5@2x.png", 167);

        public static readonly AppIconFieldType AppIconIosIconSmall40
            = new AppIconFieldType.Ios(-24, "Ios icon-small-40", false, true, "Icon-Small-40.png", 40);
        public static readonly AppIconFieldType AppIconIosIconSmall40_2x
            = new AppIconFieldType.Ios(-25, "Ios icon-small-40@2x", false, true, "Icon-Small-40@2x.png", 80);
        public static readonly AppIconFieldType AppIconIosIconSmall40_3x
            = new AppIconFieldType.Ios(-26, "Ios icon-small-40@3x", false, true, "Icon-Small-40@3x.png", 120);

        public static readonly AppIconFieldType AppIconIosIconSmall
            = new AppIconFieldType.Ios(-27, "Ios icon-small", false, true, "Icon-Small.png", 29);
        public static readonly AppIconFieldType AppIconIosIconSmall_2x
            = new AppIconFieldType.Ios(-28, "Ios icon-small@2x", false, true, "Icon-Small@2x.png", 58);
        public static readonly AppIconFieldType AppIconIosIconSmall_3x
            = new AppIconFieldType.Ios(-29, "Ios icon-small@3x", false, true, "Icon-Small@3x.png", 87);
        //watch
        public static readonly AppIconFieldType AppIconIosAppIcon40x40_2x
            = new AppIconFieldType.Ios(-30, "Ios icon-watch-40@2x", false, true, "AppIcon40x40@2x.png", 80);
        public static readonly AppIconFieldType AppIconIosAppIcon44x44_2x
            = new AppIconFieldType.Ios(-31, "Ios icon-watch-44@2x", false, true, "AppIcon44x44@2x.png", 88);
        public static readonly AppIconFieldType AppIconIosAppIcon86x86_2x
            = new AppIconFieldType.Ios(-32, "Ios icon-watch-86@2x", false, true, "AppIcon86x86@2x.png", 172);
        public static readonly AppIconFieldType AppIconIosAppIcon98x98_2x
            = new AppIconFieldType.Ios(-33, "Ios icon-watch-98@2x", false, true, "AppIcon98x98@2x.png", 196);
        public static readonly AppIconFieldType AppIconIosAppIcon24x24_2x
            = new AppIconFieldType.Ios(-34, "Ios icon-watch-24@2x", false, true, "AppIcon24x24@2x.png", 48);
        public static readonly AppIconFieldType AppIcon27_5x27_5_2x
            = new AppIconFieldType.Ios(-35, "Ios icon-watch-27-5@2x", false, true, "AppIcon27.5x27.5@2x.png", 55);
        public static readonly AppIconFieldType AppIcon29x29_2x
            = new AppIconFieldType.Ios(-36, "Ios icon-watch-29@2x", false, true, "AppIcon29x29@2x.png", 58);
        public static readonly AppIconFieldType AppIcon29x29_3x
            = new AppIconFieldType.Ios(-37, "Ios icon-watch-29@3x", false, true, "AppIcon29x29@3x.png", 87);


        //packaging
        public static PackagingFieldType PackagingSharedName
            = new PackagingFieldType(-50, "Shared name", ProjectType.Shared, true, false, StringFieldDisplayType.Text);
        public static PackagingFieldType PackagingSharedIdentifier
            = new PackagingFieldType(-51, "Shared identifier", ProjectType.Shared, true, false, StringFieldDisplayType.Text);
        public static PackagingFieldType PackagingSharedVersionText
            = new PackagingFieldType(-52, "Shared version", ProjectType.Shared, true, false, StringFieldDisplayType.Text);
        public static PackagingFieldType PackagingSharedVersionNumber
            = new PackagingFieldType(-53, "Shared version number", ProjectType.Shared, true, false, StringFieldDisplayType.Number);
        public static PackagingFieldType PackagingSharedAppIconName
            = new PackagingFieldType(-67, "Shared app icon name", ProjectType.Shared, true, false, StringFieldDisplayType.Text);
        public static PackagingFieldType PackagingSharedSplashName
            = new PackagingFieldType(-68, "Shared splash image name", ProjectType.Shared, true, false, StringFieldDisplayType.Text);

        public static PackagingFieldType PackagingDroidName
            = new PackagingFieldType(-54, "Droid name", ProjectType.Droid, true, true, StringFieldDisplayType.Text);
        public static PackagingFieldType PackagingDroidIdentifier
            = new PackagingFieldType(-55, "Droid identifier", ProjectType.Droid, true, true, StringFieldDisplayType.Text);
        public static PackagingFieldType PackagingDroidVersionText
            = new PackagingFieldType(-56, "Droid version", ProjectType.Droid, true, true, StringFieldDisplayType.Text);
        public static PackagingFieldType PackagingDroidVersionNumber
            = new PackagingFieldType(-57, "Droid version number", ProjectType.Droid, true, true, StringFieldDisplayType.Number);
        public static PackagingFieldType PackagingDroidAppIconName
            = new PackagingFieldType(-63, "Droid app icon name", ProjectType.Droid, true, true, StringFieldDisplayType.Text);
        public static PackagingFieldType PackagingDroidSplashName
            = new PackagingFieldType(-64, "Droid splash image name", ProjectType.Droid, true, true, StringFieldDisplayType.Text);

        public static PackagingFieldType PackagingIosName
            = new PackagingFieldType(-58, "Ios name", ProjectType.Ios, true, true, StringFieldDisplayType.Text);
        public static PackagingFieldType PackagingIosIdentifier
            = new PackagingFieldType(-59, "Ios identifier", ProjectType.Ios, true, true, StringFieldDisplayType.Text);
        public static PackagingFieldType PackagingIosVersionText
            = new PackagingFieldType(-60, "Ios version", ProjectType.Ios, true, true, StringFieldDisplayType.Text);
        public static PackagingFieldType PackagingIosVersionNumber
            = new PackagingFieldType(-61, "Ios version number", ProjectType.Ios, true, true, StringFieldDisplayType.Number);
        public static PackagingFieldType PackagingIosAppIconXcAssetsName
            = new PackagingFieldType(-65, "Ios app icon xcassets name", ProjectType.Ios, true, true, StringFieldDisplayType.Text);
        public static PackagingFieldType PackagingIosLaunchImageXcAssetsName
            = new PackagingFieldType(-66, "Ios launch image xcassets name", ProjectType.Ios, true, true, StringFieldDisplayType.Text);
        public static PackagingFieldType PackagingIosUsesNonExemptEncryption
            = new PackagingFieldType(-62, "Ios uses non exempt encryption", ProjectType.Ios, false, true, StringFieldDisplayType.Bool);


        public static BuildConfigRecordSetType BuildConfigFieldSetCompilerConstants
            = new BuildConfigRecordSetType(-70, "Compiler constants", ProjectType.Shared, true, StringFieldDisplayType.Text);

        //splash
        public static readonly SplashFieldType SplashSharedMaster
            = new SplashFieldType.Shared(-80, "Shared master splash screen", true, false, "shared-master-splash.png", 2048, 2048);

        public static readonly SplashFieldType SplashDroidMaster
            = new SplashFieldType.Droid(-85, "Droid master splash screen", true, false, "droid-master-splash.png", 2048, 2048);

        public static readonly SplashFieldType SplashIosMaster
            = new SplashFieldType.Ios(-125, "Ios master splash screen", true, false, "ios-master-splash.png", 2048, 2048);

        public static IEnumerable<AppIconFieldType> AppIcons()
        {
            var ret = GetAllOf<AppIconFieldType>().OrderByDescending(x => x.Value);
            return ret;
        }

        public static IEnumerable<PackagingFieldType> Packaging()
        {
            var ret = GetAllOf<PackagingFieldType>().OrderByDescending(x => x.Value);
            return ret;
        }

        public static IEnumerable<BuildConfigRecordSetType> BuildConfigRecordSet()
        {
            var ret = GetAllOf<BuildConfigRecordSetType>().OrderByDescending(x => x.Value);
            return ret;
        }

        public static IEnumerable<SplashFieldType> Splash()
        {
            var ret = GetAllOf<SplashFieldType>().OrderByDescending(x => x.Value);
            return ret;
        }

        protected FieldType(int value, string displayName, ProjectType projectType, FieldHolderType fieldHolderType, bool isForClient) : base(value, displayName)
        {
            _projectType = projectType;
            _fieldHolderType = fieldHolderType;
            _isForClient = isForClient;
        }
    }
}
