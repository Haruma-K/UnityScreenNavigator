namespace Demo.Scripts
{
    public static class ResourceKey
    {
        private const string PrefabFormat = "Prefabs/prefab_demo_{0}";
        private const string TopPagePrefabName = "page_top";
        private const string HomePagePrefabName = "page_home";
        private const string HomeLoadingPagePrefabName = "page_home_loading";
        private const string ShopPagePrefabName = "page_shop";
        private const string SettingsModalPrefabName = "modal_settings";
        private const string CharacterModalPrefabName = "modal_character";
        private const string ShopItemGridSheetPrefabName = "sheet_shop_item_grid";
        private const string CharacterModalImageSheetPrefabName = "sheet_character_modal_image";
        private const string CharacterImageModalPrefabName = "modal_character_image";
        
        private const string CharacterImageFormat = "Textures/tex_character_{0:D3}_{1}";
        private const string CharacterThumbnailFormat = "Textures/tex_character_thumb_{0:D3}_{1}";

        public static string TopPagePrefab()
        {
            return string.Format(PrefabFormat, TopPagePrefabName);
        }
        
        public static string HomePagePrefab()
        {
            return string.Format(PrefabFormat, HomePagePrefabName);
        }
        
        public static string HomeLoadingPagePrefab()
        {
            return string.Format(PrefabFormat, HomeLoadingPagePrefabName);
        }
        
        public static string ShopPagePrefab()
        {
            return string.Format(PrefabFormat, ShopPagePrefabName);
        }
        
        public static string SettingsModalPrefab()
        {
            return string.Format(PrefabFormat, SettingsModalPrefabName);
        }
        
        public static string CharacterModalPrefab()
        {
            return string.Format(PrefabFormat, CharacterModalPrefabName);
        }
        
        public static string ShopItemGridSheetPrefab()
        {
            return string.Format(PrefabFormat, ShopItemGridSheetPrefabName);
        }
        
        public static string CharacterModalImageSheetPrefab()
        {
            return string.Format(PrefabFormat, CharacterModalImageSheetPrefabName);
        }
        
        public static string CharacterImageModalPrefab()
        {
            return string.Format(PrefabFormat, CharacterImageModalPrefabName);
        }

        public static string CharacterSprite(int characterId, int rank)
        {
            return string.Format(CharacterImageFormat, characterId, rank);
        }
        
        public static string CharacterThumbnailSprite(int characterId, int rank)
        {
            return string.Format(CharacterThumbnailFormat, characterId, rank);
        }
    }
}