namespace Demo.Core.Scripts.Foundation.Common
{
    public static class ResourceKey
    {
        public static class Prefabs
        {
            private const string Prefix = "prefab_";
            private const string TopFolder = "Top";
            private const string HomeFolder = "Home";
            private const string LoadingFolder = "Loading";
            private const string SettingsFolder = "Settings";
            private const string UnitShopFolder = "UnitShop";
            private const string UnitTypeInformationFolder = "UnitTypeInformation";
            private const string UnitPortraitViewerFolder = "UnitPortraitViewer";

            public const string TopPage = TopFolder + "/" + Prefix + "top_page.prefab";
            public const string HomePage = HomeFolder + "/" + Prefix + "home_page.prefab";
            public const string LoadingPage = LoadingFolder + "/" + Prefix + "loading_page.prefab";
            public const string SettingsModal = SettingsFolder + "/" + Prefix + "settings_modal.prefab";
            public const string UnitShopPage = UnitShopFolder + "/" + Prefix + "unit_shop_page.prefab";

            public const string UnitTypeInformationModal =
                UnitTypeInformationFolder + "/" + Prefix + "unit_type_information_modal.prefab";

            public const string UnitPortraitViewerModal =
                UnitPortraitViewerFolder + "/" + Prefix + "unit_portrait_viewer_modal.prefab";
        }

        public static class Textures
        {
            private const string Prefix = "tex_";

            public const string CoinIcon = Prefix + "icon_coin";

            private const string UnitPortraitFormat = Prefix + "unit_portrait_{0}_{1}";

            private const string UnitThumbnailFormat = Prefix + "unit_thumb_{0}_{1}";

            public static string GetUnitPortrait(string unitTypeMasterId, int rank)
            {
                return string.Format(UnitPortraitFormat, unitTypeMasterId, rank);
            }

            public static string GetUnitThumbnail(string unitTypeMasterId, int rank)
            {
                return string.Format(UnitThumbnailFormat, unitTypeMasterId, rank);
            }
        }
    }
}
