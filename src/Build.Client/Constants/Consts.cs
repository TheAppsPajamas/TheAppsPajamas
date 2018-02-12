﻿using System;
namespace Build.Client.Constants
{
    public static class Consts
    {
        public const string TheAppsPajamasResourcesDir = "theappspajamas-resources";
        public const string MediaResourcesDir = "media-resources";
        public const string AssetCatalogueOutputDir = "asset-catalogue-output";
        public const string iTunesArtworkDir = "iTunesArtwork";

        public const string ModifiedProjectNameExtra = ".theappspajamas";

        public const string ProjectConfig = "project.config";

        public const string DroidResources = "Resources";
        public const string UrlBase = "http://buildapidebug.me";

        public const string ClientEndpoint = "/api/client";
        public const string TokenEndpoint = "/connect/token";

        public const string MediaEndpoint = "/api/media";

        public const string AssetCatalogueContents = "{\r\n  \"info\" : {\r\n    \"version\" : 1,\r\n    \"author\" : \"xcode\"\r\n  }\r\n}";

        public const string AppIconSetDefaultContents = "{\r\n  \"images\" : [\r\n    {\r\n      \"idiom\" : \"iphone\",\r\n      \"size\" : \"20x20\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"iphone\",\r\n      \"size\" : \"20x20\",\r\n      \"scale\" : \"3x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"iphone\",\r\n      \"size\" : \"29x29\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"iphone\",\r\n      \"size\" : \"29x29\",\r\n      \"scale\" : \"3x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"iphone\",\r\n      \"size\" : \"40x40\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"iphone\",\r\n      \"size\" : \"40x40\",\r\n      \"scale\" : \"3x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"iphone\",\r\n      \"size\" : \"60x60\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"iphone\",\r\n      \"size\" : \"60x60\",\r\n      \"scale\" : \"3x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ipad\",\r\n      \"size\" : \"20x20\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ipad\",\r\n      \"size\" : \"20x20\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ipad\",\r\n      \"size\" : \"29x29\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ipad\",\r\n      \"size\" : \"29x29\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ipad\",\r\n      \"size\" : \"40x40\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ipad\",\r\n      \"size\" : \"40x40\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ipad\",\r\n      \"size\" : \"76x76\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ipad\",\r\n      \"size\" : \"76x76\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ipad\",\r\n      \"size\" : \"83.5x83.5\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ios-marketing\",\r\n      \"size\" : \"1024x1024\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"size\" : \"24x24\",\r\n      \"idiom\" : \"watch\",\r\n      \"scale\" : \"2x\",\r\n      \"role\" : \"notificationCenter\",\r\n      \"subtype\" : \"38mm\"\r\n    },\r\n    {\r\n      \"size\" : \"27.5x27.5\",\r\n      \"idiom\" : \"watch\",\r\n      \"scale\" : \"2x\",\r\n      \"role\" : \"notificationCenter\",\r\n      \"subtype\" : \"42mm\"\r\n    },\r\n    {\r\n      \"size\" : \"29x29\",\r\n      \"idiom\" : \"watch\",\r\n      \"role\" : \"companionSettings\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"size\" : \"29x29\",\r\n      \"idiom\" : \"watch\",\r\n      \"role\" : \"companionSettings\",\r\n      \"scale\" : \"3x\"\r\n    },\r\n    {\r\n      \"size\" : \"40x40\",\r\n      \"idiom\" : \"watch\",\r\n      \"scale\" : \"2x\",\r\n      \"role\" : \"appLauncher\",\r\n      \"subtype\" : \"38mm\"\r\n    },\r\n    {\r\n      \"size\" : \"44x44\",\r\n      \"idiom\" : \"watch\",\r\n      \"scale\" : \"2x\",\r\n      \"role\" : \"longLook\",\r\n      \"subtype\" : \"42mm\"\r\n    },\r\n    {\r\n      \"size\" : \"86x86\",\r\n      \"idiom\" : \"watch\",\r\n      \"scale\" : \"2x\",\r\n      \"role\" : \"quickLook\",\r\n      \"subtype\" : \"38mm\"\r\n    },\r\n    {\r\n      \"size\" : \"98x98\",\r\n      \"idiom\" : \"watch\",\r\n      \"scale\" : \"2x\",\r\n      \"role\" : \"quickLook\",\r\n      \"subtype\" : \"42mm\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"16x16\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"16x16\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"32x32\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"32x32\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"128x128\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"128x128\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"256x256\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"256x256\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"512x512\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"512x512\",\r\n      \"scale\" : \"2x\"\r\n    }\r\n  ],\r\n  \"info\" : {\r\n    \"version\" : 1,\r\n    \"author\" : \"xcode\"\r\n  }\r\n}\r\n";

        public const string AppIcon = "{\r\n  \"images\" : [\r\n    {\r\n      \"idiom\" : \"iphone\",\r\n      \"size\" : \"20x20\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"iphone\",\r\n      \"size\" : \"20x20\",\r\n      \"scale\" : \"3x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"iphone\",\r\n      \"size\" : \"29x29\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"iphone\",\r\n      \"size\" : \"29x29\",\r\n      \"scale\" : \"3x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"iphone\",\r\n      \"size\" : \"40x40\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"iphone\",\r\n      \"size\" : \"40x40\",\r\n      \"scale\" : \"3x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"iphone\",\r\n      \"size\" : \"60x60\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"iphone\",\r\n      \"size\" : \"60x60\",\r\n      \"scale\" : \"3x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ipad\",\r\n      \"size\" : \"20x20\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ipad\",\r\n      \"size\" : \"20x20\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ipad\",\r\n      \"size\" : \"29x29\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ipad\",\r\n      \"size\" : \"29x29\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ipad\",\r\n      \"size\" : \"40x40\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ipad\",\r\n      \"size\" : \"40x40\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ipad\",\r\n      \"size\" : \"76x76\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ipad\",\r\n      \"size\" : \"76x76\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ipad\",\r\n      \"size\" : \"83.5x83.5\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"ios-marketing\",\r\n      \"size\" : \"1024x1024\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"size\" : \"24x24\",\r\n      \"idiom\" : \"watch\",\r\n      \"scale\" : \"2x\",\r\n      \"role\" : \"notificationCenter\",\r\n      \"subtype\" : \"38mm\"\r\n    },\r\n    {\r\n      \"size\" : \"27.5x27.5\",\r\n      \"idiom\" : \"watch\",\r\n      \"scale\" : \"2x\",\r\n      \"role\" : \"notificationCenter\",\r\n      \"subtype\" : \"42mm\"\r\n    },\r\n    {\r\n      \"size\" : \"29x29\",\r\n      \"idiom\" : \"watch\",\r\n      \"role\" : \"companionSettings\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"size\" : \"29x29\",\r\n      \"idiom\" : \"watch\",\r\n      \"role\" : \"companionSettings\",\r\n      \"scale\" : \"3x\"\r\n    },\r\n    {\r\n      \"size\" : \"40x40\",\r\n      \"idiom\" : \"watch\",\r\n      \"scale\" : \"2x\",\r\n      \"role\" : \"appLauncher\",\r\n      \"subtype\" : \"38mm\"\r\n    },\r\n    {\r\n      \"size\" : \"44x44\",\r\n      \"idiom\" : \"watch\",\r\n      \"scale\" : \"2x\",\r\n      \"role\" : \"longLook\",\r\n      \"subtype\" : \"42mm\"\r\n    },\r\n    {\r\n      \"size\" : \"86x86\",\r\n      \"idiom\" : \"watch\",\r\n      \"scale\" : \"2x\",\r\n      \"role\" : \"quickLook\",\r\n      \"subtype\" : \"38mm\"\r\n    },\r\n    {\r\n      \"size\" : \"98x98\",\r\n      \"idiom\" : \"watch\",\r\n      \"scale\" : \"2x\",\r\n      \"role\" : \"quickLook\",\r\n      \"subtype\" : \"42mm\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"16x16\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"16x16\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"32x32\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"32x32\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"128x128\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"128x128\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"256x256\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"256x256\",\r\n      \"scale\" : \"2x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"512x512\",\r\n      \"scale\" : \"1x\"\r\n    },\r\n    {\r\n      \"idiom\" : \"mac\",\r\n      \"size\" : \"512x512\",\r\n      \"scale\" : \"2x\"\r\n    }\r\n  ],\r\n  \"info\" : {\r\n    \"version\" : 1,\r\n    \"author\" : \"xcode\"\r\n  }\r\n}\r\n";
    }
}
