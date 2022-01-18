namespace GameManager.Components
{
    public class TagPickerScreen
    {
        public static void Show(TagType tagType, StoreType storeType)
        {
            Console.Clear();
            UIElements.PageTitle($"Choose {tagType}");

            List<string> tags = new List<string>();

            var loader = new LoaderService();
            var library = new LibraryService();

            switch ( tagType )
            {
                case TagType.Tag:
                    tags = loader.Tags;
                    break;
                case TagType.Prefix:
                    tags = loader.Prefixes;
                    break;
            }

            var selection = UIElements.PagedMenu(tags, tagType.ToString(), "Blank to cancel");

            if ( selection != null )
            {
                switch ( storeType )
                {
                    case StoreType.Library:
                        LibraryMenu.Show();
                        break;
                    case StoreType.Metadata:
                        MetadataSearchResultsScreen.Show(library.GetMetadataByTag(tags[(int) selection], tagType), $"Games with [{tags[(int) selection]}] {tagType}");
                        break;
                }
            }
        }
    }
}