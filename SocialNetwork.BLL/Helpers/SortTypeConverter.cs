using SocialNetwork.BLL.Models.Enums;

namespace SocialNetwork.BLL.Helpers;

public class SortTypeConverter
{
    private static readonly Dictionary<string, SortType> StringToSortTypeMap = new Dictionary<string, SortType>(StringComparer.OrdinalIgnoreCase)
    {
        { "Online", SortType.Online },
        { "Recently added", SortType.RecentlyAdded },
        { "First name", SortType.FirstName },
        { "Last name", SortType.LastName }
    };

    public static SortType ConvertFromString(string sortTypeString)
    {
        if (StringToSortTypeMap.TryGetValue(sortTypeString, out var sortType))
        {
            return sortType;
        }
        else
        {
            throw new ArgumentException($"Invalid sortTypeString: {sortTypeString}");
        }
    }
}